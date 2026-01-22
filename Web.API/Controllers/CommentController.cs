using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.API.Dtos.Comment;
using Web.API.Extensions;
using Web.API.Interfaces;
using Web.API.Mappers;
using Web.API.Models;

namespace Web.API.Controllers
{
    [Route("api/comments")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentsRepository _commentsRepo;
        private readonly IStockRepository _stockRepo;
        private readonly UserManager<AppUser> _userManager;
        public CommentController(ICommentsRepository commentsRepo, IStockRepository stockRepo,
            UserManager<AppUser> userManager)
        {
            _commentsRepo = commentsRepo;
            _stockRepo = stockRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {

            var comments = await _commentsRepo.GetAllAsync(ct);

            var commentsDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentsDto);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken ct)
        {
            var comment = await _commentsRepo.GetById(id, ct);

            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment.ToCommentDto());
        }

        [HttpPost("{stockId:int}")]
        [Authorize]
        public async Task<IActionResult> CreateComment([FromRoute] int stockId, CreateCommentDto commentDto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!await _stockRepo.StockExists(stockId, ct))
            {
                return BadRequest("Stock does not exists");
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            var commentModel = commentDto.ToCommentFromCreate(stockId);

            commentModel.AppUserId = appUser.Id;

            await _commentsRepo.CreateCommentAsync(commentModel, ct);

            return CreatedAtAction(nameof(GetById), new { id = commentModel.ID }, commentModel.ToCommentDto());
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateComment([FromRoute] int id, UpdateCommentDto updateDto, CancellationToken ct)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentsRepo.UpdateCommentAsync(id, updateDto.ToCommentFromUpdate(), ct);

            if (comment == null)
            {
                return NotFound("Comment was not found");
            }

            return Ok(comment.ToCommentDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteComment([FromRoute] int id, CancellationToken ct)
        {
            var comment = await _commentsRepo.DeleteAsync(id, ct);

            if (comment == null)
            {
                return NotFound("Couldn't find comment");
            }

            return NoContent();
        }

    }
}
