using Microsoft.AspNetCore.Mvc;
using Web.API.Interfaces;
using Web.API.Mappers;

namespace Web.API.Controllers
{
    [Route("api/comments")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentsRepository _commentsRepo;
        public CommentController(ICommentsRepository commentsRepo)
        {
            _commentsRepo = commentsRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _commentsRepo.GetAllAsync();

            var commentsDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentsDto);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var comment = await _commentsRepo.GetById(id);

            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }
    }
}
