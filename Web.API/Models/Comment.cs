using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Web.API.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public int? StockID { get; set; }
        public Stock? Stock { get; set; }
    }
}
