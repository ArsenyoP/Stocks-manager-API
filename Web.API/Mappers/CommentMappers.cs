using System.Runtime.CompilerServices;
using Web.API.Dtos.Comment;
using Web.API.Models;

namespace Web.API.Mappers
{
    public static class CommentMappers
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                ID = commentModel.ID,
                StockID = commentModel.StockID,
                Title = commentModel.Title,
                CreatedBy = commentModel.AppUser.UserName
            };
        }

        public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int stockId)
        {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content,
                StockID = stockId
            };
        }

        public static Comment ToCommentFromUpdate(this UpdateCommentDto commentDto)
        {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content
            };
        }

    }
}
