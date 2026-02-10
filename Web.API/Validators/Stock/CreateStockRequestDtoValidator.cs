using FluentValidation;
using Web.API.Dtos.Stock;


namespace Web.API.Validators.Stock
{
    internal sealed class CreateStockRequestDtoValidator : AbstractValidator<CreateStockRequestDto>
    {
        public CreateStockRequestDtoValidator()
        {
            RuleFor(x => x.CompanyName)
               .NotEmpty().WithMessage("Company name must contain a value")
               .Length(2, 40).WithMessage("Company name must be in range from 2 to 40 symbols");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Symbol must contain a value")
                .Length(2, 10).WithMessage("Symbol must be in range from 2 to 10 symbols");

            RuleFor(x => x.Purchase)
                .GreaterThan(0).WithMessage("Purchase must be greater than 0");

            RuleFor(x => x.LastDiv)
                .GreaterThanOrEqualTo(0).WithMessage("LastDiv must be greater than or equal to 0");

            RuleFor(x => x.Industy)
                .NotEmpty().WithMessage("Industry must contain a value")
                .Length(2, 50).WithMessage("Industry must be in range from 2 to 50 symbols");

            RuleFor(x => x.MarketCap)
                .GreaterThan(0).WithMessage("MarketCap must be greater than 0");
        }
    }

}
}
