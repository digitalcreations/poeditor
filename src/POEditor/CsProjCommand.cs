namespace POEditor
{
    using McMaster.Extensions.CommandLineUtils;

    [Command("csproj")]
    [HelpOption("--help")]
    [Subcommand(typeof(PatchCsProjCommand))]
    class CsProjCommand 
    {

    }
}