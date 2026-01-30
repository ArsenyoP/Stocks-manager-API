using Microsoft.EntityFrameworkCore;
using Web.API.Dtos.Comment;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Mappers;

namespace Web.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentsRepository _commentsRepo;
        private readonly IStockRepository _stockRepo;

        public CommentService(ICommentsRepository commentsRepo, IStockRepository stockRepo)
        {
            _commentsRepo = commentsRepo;
            _stockRepo = stockRepo;
        }

        public async Task<List<CommentDto>> GetAll(int page, CancellationToken ct)
        {
            int pageSize = 20;

            if (0 >= page)
            {
                page = 1;
            }


            var skipNumber = (page - 1) * pageSize;

            return await _commentsRepo.GetAllQuery()
                .OrderByDescending(c => c.CreatedOn)
                .Skip(skipNumber)
                .Take(pageSize)
                .Select(c => new CommentDto
                {
                    ID = c.ID,
                    Title = c.Title,
                    Content = c.Content,
                    CreatedOn = c.CreatedOn,
                    CreatedBy = c.AppUser.UserName
                }).ToListAsync(ct);
        }

        public async Task<CommentDto> GetById(int id, CancellationToken ct)
        {
            var commentModel = await _commentsRepo.GetById(id, ct);

            if (commentModel == null)
            {
                throw new KeyNotFoundException($"Can't find comment with ID: {id}");
            }

            return commentModel.ToCommentDto();
        }


        public async Task<CommentDto> CreateComment(int stockId, string AppUserId, CreateCommentDto commentDto, CancellationToken ct)
        {
            if (!await _stockRepo.StockExists(stockId, ct))
            {
                throw new KeyNotFoundException("Stock doesn't exists");
            }


            var commentModel = commentDto.ToCommentFromCreate(stockId);

            commentModel.AppUserId = AppUserId;

            await _commentsRepo.CreateCommentAsync(commentModel, ct);
            return commentModel.ToCommentDto();
        }

        public async Task<CommentDto> UpdateComment(int id, UpdateCommentDto updateDto, CancellationToken ct)
        {
            var existingComment = await _commentsRepo.GetById(id);

            if (existingComment == null)
            {
                throw new KeyNotFoundException("Stock doesn't exists");
            }

            existingComment = await _commentsRepo.UpdateCommentAsync(existingComment, updateDto, ct);
            return existingComment.ToCommentDto();

        }


        public async Task DeleteComment(int id, CancellationToken ct)
        {
            var commentModel = await _commentsRepo.GetById(id);

            if (commentModel == null)
            {
                throw new KeyNotFoundException("Stock doesn't exists");
            }

            await _commentsRepo.DeleteAsync(commentModel, ct);
        }
    }
}
