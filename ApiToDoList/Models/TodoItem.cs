namespace TodoApi.Models
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
