using FluentValidation;

namespace Ecommerce.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderCommandValidator
    : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotNull()
            .NotEmpty();

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.ProductName)
                    .NotEmpty();

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0);

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0);
            });
    }
}