﻿namespace POEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using McMaster.Extensions.CommandLineUtils;

    using Microsoft.Build.Evaluation;

    [Command("patch", Description = "Patch .csproj file to include .resx-files correctly")]
    class PatchCsProjCommand
    {

        [Option(CommandOptionType.SingleValue, Description = "Path to project file (.csproj). Will look for .resx files and add it to the project file.", ShortName = "p")]
        public string Project { get; set; }

        public Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (!File.Exists(this.Project))
            {
                Console.WriteLine("Project file not found.");

                return Task.FromResult<int>(-1);
            }

            if (!Path.GetExtension(this.Project).Equals(".csproj", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"File '{this.Project}' is not a .csproj file");
                return Task.FromResult<int>(-1);
            }

            var originalWorkingDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(this.Project));

            var proj = new Project(this.Project, globalProperties: null, toolsVersion: null, projectCollection: ProjectCollection.GlobalProjectCollection, loadSettings: ProjectLoadSettings.IgnoreMissingImports);

            proj.RemoveItems(proj.Items.Where(i => i.IsResourceFile() || i.IsDependentOnResourceFile()));

            var directory = Path.GetDirectoryName(this.Project);
            var resourceFiles = Directory.GetFiles(directory, "*.resx", SearchOption.AllDirectories)
                .Select(f => f.Replace(directory, "").TrimStart(Path.DirectorySeparatorChar))
                .OrderBy(f => Path.GetFileNameWithoutExtension(f))
                .GroupBy(f => Path.GetDirectoryName(f) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f)));

            Console.WriteLine($"Found {resourceFiles.Count()} default language resource files ({resourceFiles.Sum(g => g.Count())} including localized resource files).");

            foreach (var group in resourceFiles)
            {
                var owner = group.First();

                var designerFilename = Path.Combine(Path.GetDirectoryName(owner), Path.GetFileNameWithoutExtension(owner) + ".Designer.cs");

                var embeddedResourceMetadata = new List<KeyValuePair<string, string>>();
                if (File.Exists(designerFilename))
                {
                    embeddedResourceMetadata.Add(new KeyValuePair<string, string>("Generator", "PublicResXFileCodeGenerator"));
                    embeddedResourceMetadata.Add(new KeyValuePair<string, string>("LastGenOutput", designerFilename));

                    proj.AddItem("Compile", designerFilename, new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("DependentUpon", Path.GetFileName(owner)),
                    new KeyValuePair<string, string>("AutoGen", "True"),
                    new KeyValuePair<string, string>("DesignTime", "True")
                });
                }

                proj.AddItem("EmbeddedResource", owner, embeddedResourceMetadata);

                foreach (var item in group.Skip(1))
                {
                    proj.AddItem("EmbeddedResource", item, new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("DependentUpon", Path.GetFileName(owner))
                    });
                }
            }

            Console.WriteLine("Saving project file.");
            proj.Save();
            Directory.SetCurrentDirectory(originalWorkingDirectory);

            return Task.FromResult(0);
        }


    }

    static class ItemExtensions
    {
        public static bool IsResourceFile(this ProjectItem projectItem) => projectItem.UnevaluatedInclude.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase);
        public static bool IsDependentOnResourceFile(this ProjectItem projectItem) => projectItem.HasMetadata("DependentUpon") && projectItem.GetMetadataValue("DependentUpon").EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase);
    }
}