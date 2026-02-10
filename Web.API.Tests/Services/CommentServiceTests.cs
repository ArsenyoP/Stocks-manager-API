using FluentAssertions;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.API.Dtos.Comment;
using Web.API.Interfaces;
using Web.API.Models;
using Web.API.Services;

namespace Web.API.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentsRepository> _commentsRepoMock;
        private readonly Mock<IStockRepository> _stockRepoMock;
        private readonly Mock<ILogger<CommentService>> _loggerMock;
        private readonly CommentService _commentService;

        public CommentServiceTests()
        {
            _commentsRepoMock = new Mock<ICommentsRepository>();
            _stockRepoMock = new Mock<IStockRepository>();
            _loggerMock = new Mock<ILogger<CommentService>>();
            _commentService = new CommentService(_commentsRepoMock.Object, _stockRepoMock.Object, _loggerMock.Object);
        }

        private List<Comment> GetTestComments()
        {
            return Enumerable.Range(0, 25).Select(i => new Comment
            {
                ID = i,
                Title = $"Title {i}",
                Content = $"Content for comment {i}",
                CreatedOn = DateTime.UtcNow.AddDays(i),
                StockID = i,
                AppUserId = $"User {i}",
                AppUser = new AppUser { UserName = $"User {i}" }
            }).ToList();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(0)]
        public async Task GetAll_NotEmpty_ReturnsListComments(int page)
        {
            var comments = GetTestComments();
            var mockQueryable = comments.BuildMock();

            _commentsRepoMock.Setup(x => x.GetAllQuery(It.IsAny<CancellationToken>()))
                .Returns(mockQueryable);


            var result = await _commentService.GetAll(page, CancellationToken.None);


            if (page == 1 && page == 0)
            {
                result.Should().HaveCount(20);

                result[0].Title.Should().Be("Title 25");
                result[0].Content.Should().Be("Content for comment 25");
                result[0].CreatedBy.Should().Be("User 25");
                result[0].ID.Should().Be(25);

            }
            if (page == 2)
            {
                result.Should().HaveCount(5);

                result[4].Title.Should().Be("Title 0");
                result[4].Content.Should().Be("Content for comment 0");
                result[4].CreatedBy.Should().Be("User 0");
                result[4].ID.Should().Be(0);
            }
        }

        [Fact]
        public async Task GetAll_EmptyDB_ReturnsListComments()
        {
            var comments = new List<Comment> { };
            var mockQueryable = comments.BuildMock();
            var page = 1;

            _commentsRepoMock.Setup(x => x.GetAllQuery(It.IsAny<CancellationToken>()))
                .Returns(mockQueryable);


            var result = await _commentService.GetAll(page, CancellationToken.None);


            result.Should().HaveCount(0);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetById_ExistingComment_ReturnsCommentDto()
        {
            var commentId = 1;
            var comment = new Comment { ID = commentId, Title = "Comment 1" };

            _commentsRepoMock.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(comment);


            var result = await _commentService.GetById(commentId, CancellationToken.None);


            result.ID.Should().Be(commentId);
            result.Title.Should().Be("Comment 1");
        }

        [Fact]
        public async Task GetById_NonExistingComment_ThrowsKeyNotFoundException()
        {
            var commentId = 1;

            _commentsRepoMock.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Comment?)null);


            Func<Task> act = async () => await _commentService.GetById(commentId, CancellationToken.None);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Can't find comment with ID: 1");
        }

        [Fact]
        public async Task CreateComment_ValidData_ReturnsCommentDto()
        {
            var createDto = new CreateCommentDto
            {
                Title = "Test title 1",
                Content = "Test content 1"
            };
            var commentModel = new Comment
            {
                ID = 1,
                Title = "Title 1",
                Content = "Content for comment 1",
                CreatedOn = DateTime.UtcNow.AddDays(1),
                StockID = 1,
                AppUserId = "User_1_id",
                AppUser = new AppUser { UserName = "User 1" }
            };
            var stockId = 1;
            var AppUserId = "user_id_1";

            _stockRepoMock.Setup(x => x.StockExists(stockId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _commentsRepoMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(commentModel);


            var result = await _commentService.CreateComment(stockId, AppUserId, createDto, CancellationToken.None);


            result.ID.Should().Be(1);
            result.Title.Should().Be("Title 1");
            result.Content.Should().Be("Content for comment 1");
            result.CreatedBy.Should().Be("User 1");

            _stockRepoMock.Verify(x => x.StockExists(stockId, It.IsAny<CancellationToken>()),
                Times.Once);

            _commentsRepoMock.Verify(x => x.CreateCommentAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateComment_StockDoesntExist_ThrowsKeyNotFoundException()
        {
            var createDto = new CreateCommentDto
            {
                Title = "Test title 1",
                Content = "Test content 1"
            };
            var stockId = 1;
            var appUserId = "user_id_1";

            _stockRepoMock.Setup(x => x.StockExists(stockId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);


            Func<Task> act = async () => await _commentService.CreateComment(stockId, appUserId, createDto, CancellationToken.None);


            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Stock doesn't exists");

            _commentsRepoMock.Verify(x => x.CreateCommentAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateComment_CreatedCommentNull_ThrowsKeyNotFoundException()
        {
            var createDto = new CreateCommentDto
            {
                Title = "Test title 1",
                Content = "Test content 1"
            };
            var stockId = 1;
            var appUserId = "user_id_1";

            _stockRepoMock.Setup(x => x.StockExists(stockId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _commentsRepoMock.Setup(x => x.CreateCommentAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Comment?)null);


            Func<Task> act = async () => await _commentService.CreateComment(stockId, appUserId, createDto, CancellationToken.None);


            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Error while creating comment");
        }

        [Fact]
        public async Task UpdateComment_WithValidData_ReturnsUpdatedComment()
        {
            var commentId = 1;
            var updateDto = new UpdateCommentDto
            {
                Title = "Updated title",
                Content = "Updated content"
            };
            var existingComment = new Comment
            {
                ID = commentId,
                Title = "Old title",
                Content = "Old content"
            };
            var updatedComment = new Comment
            {
                ID = commentId,
                Title = "Updated title",
                Content = "Updated content",
                AppUser = new AppUser { UserName = "User 1" }
            };

            _commentsRepoMock.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingComment);

            _commentsRepoMock.Setup(x => x.UpdateCommentAsync(existingComment, updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedComment);


            var result = await _commentService.UpdateComment(commentId, updateDto, CancellationToken.None);


            result.ID.Should().Be(commentId);
            result.Title.Should().Be("Updated title");
            result.Content.Should().Be("Updated content");

            _commentsRepoMock.Verify(x => x.GetById(commentId, It.IsAny<CancellationToken>()),
                Times.Once);
            _commentsRepoMock.Verify(x => x.UpdateCommentAsync(existingComment, updateDto, It.IsAny<CancellationToken>()), 
                Times.Once);
        }

        [Fact]
        public async Task UpdateComment_NotExistingComment_ThrowsKeyNotFoundException()
        {
            var commentId = 1;
            var updateDto = new UpdateCommentDto
            {
                Title = "Updated title",
                Content = "Updated content"
            };

            _commentsRepoMock.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Comment?)null);


            Func<Task> act = async () => await _commentService.UpdateComment(commentId, updateDto, CancellationToken.None);


            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Comment doesn't exists");

            _commentsRepoMock.Verify(x => x.UpdateCommentAsync(It.IsAny<Comment>(), It.IsAny<UpdateCommentDto>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task DeleteComment_ExistingComment_ExecutesDeleteAsync()
        {
            var commentId = 1;
            var comment = new Comment
            {
                ID = commentId,
                Title = "Comment 1"
            };

            _commentsRepoMock.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(comment);

            _commentsRepoMock.Setup(x => x.DeleteAsync(comment, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);


            await _commentService.DeleteComment(commentId, CancellationToken.None);


            _commentsRepoMock.Verify(x => x.GetById(commentId, It.IsAny<CancellationToken>()),
                Times.Once);
            _commentsRepoMock.Verify(x => x.DeleteAsync(comment, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeleteComment_NotExistingComment_ThrowsKeyNotFoundException()
        {
            var commentId = 1;

            _commentsRepoMock.Setup(x => x.GetById(commentId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Comment?)null);


            Func<Task> act = async () => await _commentService.DeleteComment(commentId, CancellationToken.None);


            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Stock doesn't exists");

            _commentsRepoMock.Verify(x => x.DeleteAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
