namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("projects")]
    [Subcommand(typeof(ProjectListCommand))]
    [Subcommand(typeof(ProjectViewCommand))]
    class ProjectCommand : POEditorCommandBase
    {

    }
}