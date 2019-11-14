namespace POEditor
{
    using System;
    using System.Threading.Tasks;

    using MarkdownLog;

    using McMaster.Extensions.CommandLineUtils;

    [Command("list")]
    class LanguageListCommand : POEditorCommandBase
    {
        [Option(CommandOptionType.SingleValue, Description = "Project id")]
        public int Id { get; set; }

        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var result = await this.API.Languages.List(this.Id);
            Console.Write(result.ToMarkdownTable());
            return 0;
        }
    }
}