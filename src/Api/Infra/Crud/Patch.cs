using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Infra.Crud
{
    public class Patch
    {
        public class Command<TEntity> : JsonPatchDocument<TEntity>, IRequest<CommandResult>
            where TEntity : class, IEntity
        {
            public Command()
            {
            }

            public Command(List<Operation<TEntity>> operations, IContractResolver contractResolver) : base(operations, contractResolver)
            {
            }
            public object Id { get; set; }
        }

        public class Handler<TEntity> : IRequestHandler<Command<TEntity>, CommandResult>
            where TEntity : class, IEntity
        {
            private readonly ApiDbContext _context;
            private readonly IMapper _mapper;

            public Handler(ApiDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CommandResult> Handle(Command<TEntity> request, CancellationToken cancellationToken)
            {
                var patched = _mapper.Map<JsonPatchDocument<TEntity>>(request);
                var entity = _context.Set<TEntity>().FindById(request.Id);
                if (entity == null)
                {
                    throw new NotFoundEntityException();
                }

                patched.ApplyTo(entity);
                await _context.SaveChangesAsync();
                return _mapper.Map<CommandResult>(entity);
            }
        }
    }

}
