using System.ComponentModel.DataAnnotations;

namespace Web.API.Dtos.Comment
{
    public class CreateCommentDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title must be longer than 5 symbols")]
        [MaxLength(280, ErrorMessage = "Title must be shorter than 280 symbols")]

        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(5, ErrorMessage = "Content must be longer than 5 symbols")]
        [MaxLength(280, ErrorMessage = "Content must be shorter than 280 symbols")]
        public string Content { get; set; } = string.Empty;
    }
}
