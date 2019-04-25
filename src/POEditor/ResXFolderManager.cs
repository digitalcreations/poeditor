namespace POEditor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Resources;

    class ResXFolderManager : IFolderManager
    {
        public string SearchPattern { get; } = "*.resx";
        private readonly HashSet<string> locales;

        public ResXFolderManager()
        {
            locales = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(c => c.Name).Where(c => !string.IsNullOrWhiteSpace(c)).ToHashSet();
        }

        public string NameFromFilePath(string path, string entryName)
        {
            // Trim ".resx" from filename
            path = path[0..^5];

            if (path.Contains('.'))
            {
                var lastPart = Path.GetFileName(path).Split('.').Last();
                if (locales.Contains(lastPart))
                {
                    path = path[0..^(lastPart.Length + 1)];
                }
            }

            return path + "::" + entryName;
        }

        public IEnumerable<Translation> GetResources(string path, string basePath)
        {
            Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(path));
            var pathParts = Path.GetFileNameWithoutExtension(path).Split('.');
            var language = pathParts.Length > 1 ? pathParts.Last() : "en-US";

            var relativePath = path.Replace(basePath, "").Trim('\\');

            using (var reader = new ResXResourceReader(path))
            {
                reader.UseResXDataNodes = true;

                var dict = reader.GetEnumerator();

                while (dict.MoveNext())
                {
                    var entry = (ResXDataNode)dict.Value;
                    var value = entry.GetValue((ITypeResolutionService)null);

                    if (value is string str)
                    {
                        yield return new Translation(this.NameFromFilePath(relativePath, entry.Name), language, str, entry.Comment);
                    }
                }
            }
        }

        public Tuple<string, string> ParseName(string name, string language)
        {
            var parts = name.Split(new[] { "::" }, 2, StringSplitOptions.None);
            if (!string.IsNullOrWhiteSpace(language))
            {
                language = "." + language;
            }

            var relativePath = $"{parts[0]}{language}.resx";
            return new Tuple<string, string>(relativePath, parts[1]);
        }

        public void SetResources(string rootPath, ICollection<Translation> translations, string defaultLanguage)
        {
            var files = translations.GroupBy(t => this.ParseName(t.Name, t.Language).Item1);

            foreach (var file in files)
            {
                var language = string.Equals(file.First().Language, defaultLanguage, StringComparison.InvariantCultureIgnoreCase) ? string.Empty : file.First().Language;
                var (relativePath, _) = this.ParseName(file.First().Name, language);

                var absolutePath = Path.Combine(rootPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath));

                using (var writer = new ResXResourceWriter(absolutePath))
                {
                    foreach (var translation in file)
                    {
                        var (__, name) = this.ParseName(translation.Name, language);
                        writer.AddResource(new ResXDataNode(name, translation.Text) { Comment = translation.Comment });
                    }
                }
            }
        }
    }
}