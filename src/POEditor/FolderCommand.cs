namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("folder")]
    [Subcommand(typeof(FolderUploadCommand))]
    [Subcommand(typeof(FolderDownloadCommand))]
    class FolderCommand : POEditorCommandBase
    {

    }
}