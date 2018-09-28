namespace POEditor
{
    using System;
    using System.Threading.Tasks;

    using MarkdownLog;

    using McMaster.Extensions.CommandLineUtils;

    [Command("view", Description = "Get project details")]
    class ProjectViewCommand : POEditorCommandBase
    {
        [Option(CommandOptionType.SingleValue, Description = "Project id")]
        public int Id { get; set; }

        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var project = await this.API.Projects.View(this.Id);
            Console.Write(project.ToPropertyValues().ToMarkdownTable());
            return 0;
        }
    }
}