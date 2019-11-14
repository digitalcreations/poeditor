namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("terms")]
    [Subcommand(typeof(TermsListCommand))]
    [Subcommand(typeof(TermsAddCommand))]
    class TermsCommand : POEditorCommandBase
    {

    }
}