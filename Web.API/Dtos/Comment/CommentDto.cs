namespace Web.API.Dtos.Comment
{
    public class CommentDto
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int? StockID { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
