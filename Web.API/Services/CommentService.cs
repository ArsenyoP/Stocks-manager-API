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
        private readonly ILogger<CommentService> _logger;

        public CommentService(ICommentsRepository commentsRepo, IStockRepository stockRepo,
            ILogger<CommentService> logger)
        {
            _commentsRepo = commentsRepo;
            _stockRepo = stockRepo;
            _logger = logger;
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
                _logger.LogWarning("User with ID {UserID} tried to create comment for non-existent" +
                    "stock with ID {StockId}", AppUserId, stockId);
                throw new KeyNotFoundException("Stock doesn't exists");
            }


            var commentModel = commentDto.ToCommentFromCreate(stockId);

            commentModel.AppUserId = AppUserId;

            var createdCommentModel = await _commentsRepo.CreateCommentAsync(commentModel, ct);

            if (createdCommentModel == null)
            {
                throw new KeyNotFoundException("Error while creating comment");
                _logger.LogError("Error while creating comment: {commentDto}",
                    commentDto);
            }

            _logger.LogInformation("User with ID {UserIDd} created comment for stock with ID {StockId}",
                AppUserId, stockId);

            return createdCommentModel.ToCommentDto();
        }

        public async Task<CommentDto> UpdateComment(int id, UpdateCommentDto updateDto, CancellationToken ct)
        {
            var existingComment = await _commentsRepo.GetById(id);

            if (existingComment == null)
            {
                _logger.LogWarning("Attemp to modify comment  with ID {CommentID}",
                    id);
                throw new KeyNotFoundException("Comment doesn't exists");
            }

            existingComment = await _commentsRepo.UpdateCommentAsync(existingComment, updateDto, ct);
            _logger.LogInformation("Updated comment with ID {CommentId}", id);

            return existingComment.ToCommentDto();

        }


        public async Task DeleteComment(int id, CancellationToken ct)
        {
            var commentModel = await _commentsRepo.GetById(id);

            if (commentModel == null)
            {
                _logger.LogWarning("Attemp to delete non-existent comment with ID {CommentId}",
                    id);
                throw new KeyNotFoundException("Stock doesn't exists");
            }

            _logger.LogInformation("Deleted comment with ID {CommentId}",
                    id);
            await _commentsRepo.DeleteAsync(commentModel, ct);
        }


    }
}
