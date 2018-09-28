namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("languages")]
    [Subcommand("list", typeof(LanguageListCommand))]
    [Subcommand("available", typeof(LanguageAvailableCommand))]
    [Subcommand("add", typeof(LanguageAddCommand))]
    class LanguagesCommand : POEditorCommandBase
    {

    }
}