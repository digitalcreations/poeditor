namespace POEditor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using McMaster.Extensions.CommandLineUtils;

    [Command("download")]
    class FolderDownloadCommand : POEditorCommandBase
    {
        [Option(CommandOptionType.SingleValue, Description = "Project id")]
        public int Id { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "f", Description = "File processor to use (supported: resx)")]
        public FolderType Type { get; set; } = FolderType.ResX;

        [Option(CommandOptionType.SingleValue)]
        public string Path { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "Require at least this language completion percentage to download")]
        [Range(0, 100)]
        public float CompletionPercentage { get; set; } = 25;

        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            this.Path = System.IO.Path.GetFullPath(this.Path);
            var manager = FolderUploadCommand.ManagerFromType(this.Type);

            var project = await this.API.Projects.View(this.Id);
            Console.WriteLine($"Project reference language is {project.ReferenceLanguage}.");

            var languages = await this.API.Languages.List(this.Id);
            Console.WriteLine($"Found {languages.Count} languages.");
            languages = languages
                .Where(l => l.Percentage > (decimal)this.CompletionPercentage).ToList();
            Console.WriteLine($"{languages.Count} languages are at least {this.CompletionPercentage:N2}% complete.");

            var translations = new List<Translation>();
            foreach (var language in languages)
            {
                var terms = await this.API.Terms.List(this.Id, language.Code);
                Console.WriteLine($"Found {terms.Count} terms in {language.Name}.");

                translations.AddRange(terms.Where(t => !string.IsNullOrEmpty(t.Translation.Content)).Select(t => new Translation(t.Name, language.Code, t.Translation.Content, t.Comment)));
            }

            manager.SetResources(this.Path, translations, project.ReferenceLanguage);

            return 0;
        }
    }
}