namespace POEditor
{
    public class Translation
    {
        public Translation(string name, string language, string text, string comment)
        {
            this.Name = name;
            this.Language = language;
            this.Text = text;
            this.Comment = comment;
        }

        public string Name { get; }
        public string Language { get; }
        public string Text { get; }
        public string Comment { get; }
    }
}