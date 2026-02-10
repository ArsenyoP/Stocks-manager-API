using FluentValidation;
using Web.API.Dtos.Stock;

namespace Web.API.Validators.Stock
{
    internal sealed class UpdateStockRequestDtoValidator : AbstractValidator<UpdateStockRequestDto>
    {
        public UpdateStockRequestDtoValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name must contain a value")
                .MaximumLength(40).WithMessage("Company name cannot be over 40 characters");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Symbol must contain a value")
                .MaximumLength(10).WithMessage("Symbol cannot be over 10 characters");

            RuleFor(x => x.Purchase)
                .GreaterThanOrEqualTo(1).WithMessage("Purchase must be at least 1")
                .LessThanOrEqualTo(1000000000).WithMessage("Purchase cannot exceed 1,000,000,000");

            RuleFor(x => x.LastDiv)
                .GreaterThanOrEqualTo(0.001m).WithMessage("LastDiv must be at least 0.001")
                .LessThanOrEqualTo(1000).WithMessage("LastDiv cannot be higher than 1000");

            RuleFor(x => x.Industy)
                .NotEmpty().WithMessage("Industry must contain a value")
                .MaximumLength(15).WithMessage("Industry cannot be over 15 characters");

            RuleFor(x => x.MarketCap)
                .GreaterThanOrEqualTo(1).WithMessage("MarketCap must be at least 1")
                .LessThanOrEqualTo(5000000000).WithMessage("MarketCap cannot exceed 5,000,000,000");
        }
    }
}
