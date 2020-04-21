using FluentValidation;
using JetBrains.Annotations;
using OrderBooks.WebApi.Models.OrderBooks;

namespace OrderBooks.WebApi.Validators
{
    [UsedImplicitly]
    public class OrderBookRequestManyValidator : AbstractValidator<OrderBookRequestMany>
    {
        public OrderBookRequestManyValidator()
        {
            RuleFor(o => o.Limit)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Limit must be greater or equal then 0.")
                .LessThanOrEqualTo(1000)
                .WithMessage("Limit must be less or equal to 1000.");
        }
    }
}
