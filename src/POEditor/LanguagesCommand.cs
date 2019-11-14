namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("languages")]
    [Subcommand(typeof(LanguageListCommand))]
    [Subcommand(typeof(LanguageAvailableCommand))]
    [Subcommand(typeof(LanguageAddCommand))]
    class LanguagesCommand : POEditorCommandBase
    {

    }
}