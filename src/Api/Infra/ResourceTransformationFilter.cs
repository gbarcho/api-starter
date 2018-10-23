using System;
using System.Linq;

namespace Api
{
    public class ResourceTransformationFilter : IActionFilter
    {
        private readonly IMapper _mapper;

        public ResourceTransformationFilter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var destinationType = context.Filters.OfType<ProducesAttribute>().FirstOrDefault()?.Type;

            if (destinationType == null) return;

            if (!context.ModelState.IsValid) return;

            if (context.Result is BadRequestResult) return;


            if (!(context.Result is ObjectResult result)) return;

            var sourceType = result.Value.GetType();

            if (IsAssignableToGenericType(sourceType, typeof(QueryResult<>)))
            {
                result.Value = _mapper.Map(result.Value, sourceType, destinationType);
            }
            if (IsAssignableToGenericType(sourceType, typeof(SingleResult<>)))
            {
                result.Value = _mapper.Map(result.Value, sourceType, destinationType);
            }


        }

        private bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }

}
