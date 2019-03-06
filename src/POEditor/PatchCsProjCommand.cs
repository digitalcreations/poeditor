namespace POEditor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using McMaster.Extensions.CommandLineUtils;

    using Microsoft.Build.BuildEngine;

    [Command("patch", Description = "Patch .csproj file to include .resx-files correctly")]
    class PatchCsProjCommand
    {

        [Option(CommandOptionType.SingleValue, Description = "Path to project file (.csproj). Will look for .resx files and add it to the project file.", ShortName = "p")]
        public string Project { get; set; }

        public Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (!File.Exists(this.Project))
            {
                Console.WriteLine("Project file not specified.");

                return Task.FromResult<int>(-1);
            }

            if (!Path.GetExtension(this.Project).Equals(".csproj", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine($"File '{this.Project}' is not a .csproj file");
                return Task.FromResult<int>(-1);
            }

            var proj = new Project();
            proj.Load(this.Project);

            var itemGroups = proj.ItemGroups
                .Cast<BuildItemGroup>()
                .Where(g => g.Cast<BuildItem>().Any(i => i.Include.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase)));

            Console.WriteLine($"Found {itemGroups.Count()} ItemGroups with .resx files. Removing any .resx files (and generated files dependent upon them) from these ItemGroups.");
            var removedItems = 0;
            foreach (var itemGroup in itemGroups)
            {
                var items = itemGroup
                    .Cast<BuildItem>()
                    .Where(i => IsResourceFile(i) || IsGeneratedFileDependentUponResourceFile(i, itemGroup))
                    .ToList();

                foreach (var item in items)
                {
                    itemGroup.RemoveItem(item);
                    removedItems++;
                }
            }

            Console.WriteLine($"Removed {removedItems} items.");

            var emptyItemGroups = proj.ItemGroups.Cast<BuildItemGroup>().Where(g => g.Count == 0).ToList();
            Console.WriteLine($"Left with {emptyItemGroups.Count} empty item groups. Removing them.");
            foreach (var itemGroup in emptyItemGroups)
            {
                proj.RemoveItemGroup(itemGroup);
            }

            var resourceItemGroup = proj.AddNewItemGroup();
            var directory = Path.GetDirectoryName(this.Project);
            var resourceFiles = Directory.GetFiles(directory, "*.resx", SearchOption.AllDirectories)
                .Select(f => f.Replace(directory, "").TrimStart(Path.DirectorySeparatorChar))
                .OrderBy(f => Path.GetFileNameWithoutExtension(f))
                .GroupBy(f => Path.GetDirectoryName(f) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f)));

            Console.WriteLine($"Found {resourceFiles.Count()} default language resource files ({resourceFiles.Sum(g => g.Count())} including localized resource files).");

            foreach (var group in resourceFiles)
            {
                var owner = group.First();
                var resource = resourceItemGroup.AddNewItem("EmbeddedResource", owner);
                resource.SetMetadata("Generator", "PublicResXFileCodeGenerator");
                resource.SetMetadata("LastGenOutput", Path.GetFileNameWithoutExtension(owner) + ".Designer.cs");

                var designerResource = resourceItemGroup.AddNewItem("Compile", Path.ChangeExtension(owner, ".Designer.cs"));
                designerResource.SetMetadata("DependentUpon", Path.GetFileName(owner), true);
                designerResource.SetMetadata("AutoGen", "True", true);
                designerResource.SetMetadata("DesignTime", "True", true);
                foreach (var item in group.Skip(1))
                {
                    var x = resourceItemGroup.AddNewItem("EmbeddedResource", item);
                    x.SetMetadata("DependentUpon", Path.GetFileName(owner), true);
                }
            }

            Console.WriteLine("Saving project file.");

            proj.Save(this.Project);

            return Task.FromResult(0);
        }

        private static bool IsGeneratedFileDependentUponResourceFile(BuildItem parentResourceFile, BuildItemGroup itemGroup)
        {
            return parentResourceFile.Include.EndsWith(".designer.cs", StringComparison.InvariantCultureIgnoreCase) && itemGroup.Cast<BuildItem>().Any(i2 => i2.GetMetadata("DependentUpon").EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool IsResourceFile(BuildItem buildItem)
        {
            return buildItem.Include.EndsWith(".resx", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}