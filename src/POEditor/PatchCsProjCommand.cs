namespace POEditor
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

            var proj = new Project(this.Project, globalProperties: null, toolsVersion: null, projectCollection: ProjectCollection.GlobalProjectCollection, loadSettings: ProjectLoadSettings.IgnoreMissingImports);

            proj.RemoveItems(proj.Items.Where(i => i.IsResourceFile() || i.isDependentOnResourceFile()));

            var directory = Path.GetDirectoryName(this.Project);
            var resourceFiles = Directory.GetFiles(directory, "*.resx", SearchOption.AllDirectories)
                .Select(f => f.Replace(directory, "").TrimStart(Path.DirectorySeparatorChar))
                .OrderBy(f => Path.GetFileNameWithoutExtension(f))
                .GroupBy(f => Path.GetDirectoryName(f) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f)));

            Console.WriteLine($"Found {resourceFiles.Count()} default language resource files ({resourceFiles.Sum(g => g.Count())} including localized resource files).");

            foreach (var group in resourceFiles)
            {
                var owner = group.First();
                proj.AddItem("EmbeddedResource", owner, new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("Generator", "PublicResXFileCodeGenerator"),
                    new KeyValuePair<string, string>("LastGenOutput", Path.GetFileNameWithoutExtension(owner) + ".Designer.cs") });

                proj.AddItem("Compile", Path.ChangeExtension(owner, ".Designer.cs"), new List<KeyValuePair<string, string>> {
                    new KeyValuePair<string, string>("DependentUpon", Path.GetFileName(owner)),
                    new KeyValuePair<string, string>("AutoGen", "True"),
                    new KeyValuePair<string, string>("DesignTime", "True")
                });

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

            return Task.FromResult(0);
        }


    }

    static class ItemExtensions
    {
        public static bool IsResourceFile(this ProjectItem projectItem) => projectItem.UnevaluatedInclude.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase);
        public static bool isDependentOnResourceFile(this ProjectItem projectItem) => projectItem.HasMetadata("DependentUpon") && projectItem.GetMetadataValue("DependentUpon").EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase);
    }
}