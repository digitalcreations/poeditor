namespace POEditor
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using MarkdownLog;

    using McMaster.Extensions.CommandLineUtils;

    class TermsListCommand : POEditorCommandBase
    {
        [Option(CommandOptionType.SingleValue, Description = "Project id")]
        public int Id { get; set; }

        [Option(CommandOptionType.SingleOrNoValue, Description = "Language code")]
        public string Language { get; set; }

        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var terms = await this.API.Terms.List(this.Id, this.Language);
            Console.Write(terms.Select(t => new
                                                {
                                                    t.Name,
                                                    t.Created,
                                                    t.Updated,
                                                    t.Context,
                                                    t.Comment,
                                                    t.Plural,
                                                    Tags = string.Join(", ", t.Tags)
                                                }).ToMarkdownTable());
            return 0;
        }
    }
}