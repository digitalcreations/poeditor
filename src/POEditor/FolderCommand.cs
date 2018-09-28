namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("folder")]
    [Subcommand("upload", typeof(FolderUploadCommand))]
    [Subcommand("download", typeof(FolderDownloadCommand))]
    class FolderCommand : POEditorCommandBase
    {

    }
}