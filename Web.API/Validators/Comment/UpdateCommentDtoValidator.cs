using FluentValidation;
using Web.API.Dtos.Comment;

namespace Web.API.Validators.Comment
{
    internal sealed class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title must contain a value")
                .MinimumLength(5).WithMessage("Title must be longer than 5 symbols")
                .MaximumLength(280).WithMessage("Title must be shorter than 280 symbols");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content must contain a value")
                .MinimumLength(5).WithMessage("Content must be longer than 5 symbols")
                .MaximumLength(280).WithMessage("Content must be shorter than 280 symbols");
        }
    }
}
