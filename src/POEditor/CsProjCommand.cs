namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("csproj")]
    [HelpOption("--help")]
    [Subcommand("patch", typeof(PatchCsProjCommand))]
    class CsProjCommand 
    {

    }
}