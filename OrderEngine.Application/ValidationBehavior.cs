using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace OrderEngine.Application;

public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .SelectMany(validator => validator.Validate(context).Errors)
                .Where(error => error is not null)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Cliente é obrigatório.");

        RuleFor(x => x.Items)
            .NotNull()
            .WithMessage("Itens são obrigatórios.");

        RuleFor(x => x.Items)
            .Must(items => items is not null && items.Any())
            .WithMessage("Um pedido deve ter pelo menos 1 item.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .NotEmpty()
                    .WithMessage("Produto é obrigatório.");

                item.RuleFor(x => x.ProductName)
                    .NotEmpty()
                    .WithMessage("Nome do produto é obrigatório.");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantidade deve ser maior que zero.");

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Preço unitário deve ser maior que zero.");
            });
    }
}

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O código do pedido é obrigatório.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("O status deve ser um status de pedido válido.");
    }
}
