namespace POEditor
{
    using System;
    using System.Threading.Tasks;

    using McMaster.Extensions.CommandLineUtils;

    [Command("add")]
    class LanguageAddCommand : POEditorCommandBase
    {
        [Option(CommandOptionType.SingleValue, Description = "Project id")]
        public int Id { get; set; }

        [Option(CommandOptionType.SingleValue, Description = "The language code")]
        public string Language { get; set; }

        public override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            await this.API.Languages.Add(this.Id, this.Language);
            Console.WriteLine($"Added {this.Language} to project #{this.Id}");
            return 0;
        }
    }
}