namespace Finals_Q1.Models
{
    public class Todo
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool Completed { get; set; }

        public string PreviousHash { get; set; } = "GENESIS";
        public string Hash { get; set; } = string.Empty;
    }
}