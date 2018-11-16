﻿namespace POEditor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MarkdownLog;

    using McMaster.Extensions.CommandLineUtils;

    using POEditorAPI;

    class FolderUploadCommand : POEditorCommandBase
    {
        [Option(CommandOptionType.SingleValue, Description = "Project id")]
        public int Id { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "f", Description = "File processor to use (supported: resx)")]
        public FolderType Type { get; set; } = FolderType.ResX;

        [Option(CommandOptionType.SingleValue)]
        public string Path { get; set; }

        [Option(CommandOptionType.MultipleValue, ShortName = "l", Description = "Language(s) to upload")]
        public string[] Language { get; set; }

        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var manager = ManagerFromType(this.Type);

            this.Path = System.IO.Path.GetFullPath(this.Path);
            var initialDirectory = Directory.GetCurrentDirectory();
            var translations = Directory.EnumerateFiles(this.Path, manager.SearchPattern, SearchOption.AllDirectories)
                .SelectMany(p => manager.GetResources(p, this.Path))
                .ToList();
            Directory.SetCurrentDirectory(initialDirectory);

            var termNames = translations.Select(t => t.Name).Distinct().ToList();
            var languages = translations.Select(t => t.Language).Distinct().ToList();
            Console.WriteLine($"Found {termNames.Count} distinct terms in {languages.Count} languages...");

            var syncableTerms = translations.GroupBy(t => t.Name)
                .Select(g => new Term { Name = g.Key, Comment = g.FirstOrDefault(t => !string.IsNullOrEmpty(t.Comment))?.Comment })
                .ToList();
            var syncResults = await this.API.Projects.Sync(this.Id, syncableTerms);
            Console.WriteLine($"Synced terms. {syncResults.Parsed} parsed, {syncResults.Added} added, {syncResults.Deleted} deleted, {syncResults.Updated} updated.");

            if (Language != null)
                languages = languages.Where(l => Language.Contains(l)).ToList();

            foreach (var language in languages)
            {
                var languageTranslations = translations.Where(t => t.Language == language)
                    .Select(t => new Term
                                     {
                                        Name = t.Name,
                                        Translation = new global::POEditorAPI.Translation()
                                                          {
                                                              Content = t.Text
                                                          }
                                     })
                    .ToList();
                var response = await this.API.Languages.Update(this.Id, language, false, languageTranslations);

                Console.WriteLine($"Updated {languageTranslations.Count} translations in language {language}. {response.Parsed} parsed, {response.Added} added, {response.Updated} updated.");
            }

            return 0;
        }

        public static IFolderManager ManagerFromType(FolderType type)
        {
            return new ResXFolderManager();
        }
    }

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
            languages = languages.Where(l => l.Percentage > (decimal)this.CompletionPercentage).ToList();
            Console.WriteLine($"{languages.Count} languages are at least {this.CompletionPercentage:N2}% complete.");

            var translations = new List<Translation>();
            foreach (var language in languages)
            {
                var terms = await this.API.Terms.List(this.Id, language.Code);
                Console.WriteLine($"Found {terms.Count} terms in {language.Name}.");

                translations.AddRange(terms.Select(t => new Translation(t.Name, language.Code, t.Translation.Content, t.Comment)));
            }

            manager.SetResources(this.Path, translations, project.ReferenceLanguage);

            return 0;
        }
    }
}