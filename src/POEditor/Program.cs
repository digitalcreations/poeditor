namespace POEditor
{
    using System;
    using System.Threading.Tasks;

    using McMaster.Extensions.CommandLineUtils;

    [Command("poeditor")]
    [Subcommand(typeof(ProjectCommand))]
    [Subcommand(typeof(TermsCommand))]
    [Subcommand(typeof(LanguagesCommand))]
    [Subcommand(typeof(FolderCommand))]
    [Subcommand(typeof(CsProjCommand))]
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
