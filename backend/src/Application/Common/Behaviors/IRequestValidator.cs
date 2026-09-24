using Application.Common.ErrorHandling;

namespace Application.Common.Behaviors;

public interface IRequestValidator<in TRequest>
{
    IEnumerable<Error> Validate(TRequest request);
}
