namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("terms")]
    [Subcommand("list", typeof(TermsListCommand))]
    [Subcommand("add", typeof(TermsAddCommand))]
    class TermsCommand : POEditorCommandBase
    {

    }
}