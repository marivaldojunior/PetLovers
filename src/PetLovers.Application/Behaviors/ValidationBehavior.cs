using FluentValidation;
using MediatR;
using PetLovers.Application.Interfaces;

namespace PetLovers.Application.Behaviors;

/// <summary>
/// Pipeline behavior for validating commands before they are handled.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            var errors = failures.Select(f => f.ErrorMessage).ToList();
            
            // Handle CommandResult types
            if (typeof(TResponse).IsGenericType && 
                typeof(TResponse).GetGenericTypeDefinition() == typeof(CommandResult<>))
            {
                var resultType = typeof(TResponse);
                var validationFailureMethod = resultType.GetMethod("ValidationFailure", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                if (validationFailureMethod is not null)
                {
                    return (TResponse)validationFailureMethod.Invoke(null, new object[] { errors.AsReadOnly() })!;
                }
            }
            else if (typeof(TResponse) == typeof(CommandResult))
            {
                return (TResponse)(object)CommandResult.ValidationFailure(errors.AsReadOnly());
            }

            throw new ValidationException(failures);
        }

        return await next();
    }
}
