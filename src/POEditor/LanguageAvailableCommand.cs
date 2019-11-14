namespace POEditor
{
    using System;
    using System.Threading.Tasks;

    using MarkdownLog;

    using McMaster.Extensions.CommandLineUtils;

    [Command("available")]
    class LanguageAvailableCommand : POEditorCommandBase
    {
        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var result = await this.API.Languages.Available();
            Console.Write(result.ToMarkdownTable());
            return 0;
        }
    }
}