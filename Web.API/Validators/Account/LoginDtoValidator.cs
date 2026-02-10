using FluentValidation;
using Web.API.Dtos.Account;

namespace Web.API.Validators.Account
{
    internal sealed class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName must contain a value");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password must contain a value");
        }
    }
}

