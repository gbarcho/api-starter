using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class ValidationPreProcessor<TRequest> : IRequestPreProcessor<TRequest>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;


        public ValidationPreProcessor(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var failures = _validators
                .Select(async x => await x.ValidateAsync(request, cancellationToken))
                .Select(x => x.Result)
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Count > 0)
            {
                throw new ValidationException(failures);
            }

            return Task.CompletedTask;

        }
    }

}
