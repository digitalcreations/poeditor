namespace POEditor
{
    using System;
    using System.Threading.Tasks;

    using MarkdownLog;

    using McMaster.Extensions.CommandLineUtils;

    [Command("list", Description = "List projects")]
    class ProjectListCommand : POEditorCommandBase
    {
        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var projects = await this.API.Projects.List();
            Console.Write(projects.ToMarkdownTable());
            return 0;
        }
    }
}