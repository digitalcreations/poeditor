namespace POEditor
{
    using System;
    using System.Threading.Tasks;

    using McMaster.Extensions.CommandLineUtils;

    [Command("poeditor")]
    [Subcommand("projects", typeof(ProjectCommand))]
    [Subcommand("terms", typeof(TermsCommand))]
    [Subcommand("languages", typeof(LanguagesCommand))]
    [Subcommand("folder", typeof(FolderCommand))]
    [Subcommand("csproj", typeof(CsProjCommand))]
    class Program : POEditorCommandBase
    {
        static async Task Main(string[] args)
        {
            try
            {
                Environment.ExitCode = await CommandLineApplication.ExecuteAsync<Program>(args);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.GetType().FullName);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
                Environment.ExitCode = -1;
            }
        }
    }
}
