namespace POEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using MarkdownLog;

    using McMaster.Extensions.CommandLineUtils;

    using POEditorAPI;

    [Command("add")]
    class TermsAddCommand : POEditorCommandBase
    {
        [Option(CommandOptionType.SingleValue, Description = "Project id")]
        public int Id { get; set; }

        [Option(CommandOptionType.SingleValue)]
        public string Name { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "C")]
        public string Context { get; set; }

        [Option(CommandOptionType.SingleValue)]
        public string Plural { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "c")]
        public string Comment { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "Tags, separated by comma.", ShortName = "T")]
        public string Tags { get; set; }

        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var term = new Term()
                           {
                               Name = this.Name,
                               Comment = this.Comment,
                               Context = this.Context,
                               Plural = this.Plural,
                               Tags = this.Tags.Split(',').Select(s => s.Trim()).ToArray()
                           };
            var response = await this.API.Terms.Add(this.Id, new List<Term> { term }).ConfigureAwait(false);

            Console.Write(response.ToPropertyValues().ToMarkdownTable());

            return 0;

        }
    }
}