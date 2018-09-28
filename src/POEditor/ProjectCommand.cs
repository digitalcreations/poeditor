namespace POEditor
{
    using System.Security.Cryptography;

    using McMaster.Extensions.CommandLineUtils;

    [Command("projects")]
    [Subcommand("list", typeof(ProjectListCommand))]
    [Subcommand("view", typeof(ProjectViewCommand))]
    class ProjectCommand : POEditorCommandBase
    {

    }
}