using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.API.Dtos.Comment;
using Web.API.Extensions;
using Web.API.Interfaces;
using Web.API.Interfaces.IServices;
using Web.API.Mappers;
using Web.API.Models;

namespace Web.API.Controllers
{
    [Route("api/comments")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, CancellationToken ct)
        {
            var comments = await _commentService.GetAll(page, ct);

            return Ok(comments);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var commentDto = await _commentService.GetById(id, ct);

            return Ok(commentDto);
        }

        [HttpPost("{symbol}")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromRoute] string symbol, CreateCommentDto commentDto, CancellationToken ct)
        {
            var AppUserId = User.GetUserID();

            var createdComment = await _commentService.CreateComment(symbol, AppUserId, commentDto, ct);
            return CreatedAtAction(nameof(GetById), new { id = createdComment.ID }, createdComment);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, UpdateCommentDto updateDto, CancellationToken ct)
        {
            var commentDto = await _commentService.UpdateComment(id, updateDto, ct);
            return Ok(commentDto);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id, CancellationToken ct)
        {
            await _commentService.DeleteComment(id, ct);

            return NoContent();
        }

    }
}
