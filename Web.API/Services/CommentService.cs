using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Web.API.Dtos.Comment;
using Web.API.Dtos.Stock;
using Web.API.Exceptions;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Mappers;
using Web.API.Models;

namespace Web.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentsRepository _commentsRepo;
        private readonly IStockRepository _stockRepo;
        private readonly ILogger<CommentService> _logger;
        private readonly IStockService _stockService;
        private readonly IAccountService _accountService;

        public CommentService(ICommentsRepository commentsRepo, IStockRepository stockRepo,
             ILogger<CommentService> logger, IStockService stockService, IAccountService accountService)
        {
            _commentsRepo = commentsRepo;
            _stockRepo = stockRepo;
            _logger = logger;
            _stockService = stockService;
            _accountService = accountService;
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


        public async Task<CommentDto> CreateComment(string symbol, string AppUserId, CreateCommentDto commentDto, CancellationToken ct)
        {
            var stockId = await EnsureStockExists(symbol, ct);
            return await SaveComment(stockId, AppUserId, commentDto, ct);
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



        //private methods
        private async Task<int> EnsureStockExists(string symbol, CancellationToken ct)
        {
            var symbolUpper = symbol.ToUpper();

            var stock = await _stockRepo.GetBySymbol(symbolUpper, ct);

            if (stock != null)
            {
                return stock.ID;
            }

            var stockDto = await _stockService.GetOrCreateStockAsync(symbolUpper, ct);

            if (stockDto == null)
            {
                throw new NotFoundException("Can't create comment for unexisting stock");
            }
            return stockDto.ID;
        }

        private async Task<CommentDto> SaveComment(int stockId, string UserId, CreateCommentDto commentDto, CancellationToken ct)
        {
            var comment = commentDto.ToCommentFromCreate(stockId);
            comment.AppUser = await _accountService.GetById(UserId);

            var createdComment = await _commentsRepo.CreateCommentAsync(comment, ct);

            if (createdComment == null)
            {
                throw new InvalidOperationException($"Failed to create comment for stock ID {stockId}");
            }
            _logger.LogInformation("User {UserId} created comment for stock {StockId}", UserId, stockId);
            return createdComment.ToCommentDto();
        }
    }
}