using System.Threading.Tasks;

namespace Api.Infra.Crud
{
    public class Delete
    {
        public class Command<TEntity> : IRequest<CommandResult>
            where TEntity : class, IDeleteable
        {
            public object Id { get; set; }
        }

        public class Handler<TEntity> : IRequestHandler<Command<TEntity>, CommandResult>
            where TEntity : class, IDeleteable
        {
            private readonly ApiDbContext _context;

            public Handler(ApiDbContext context)
            {
                _context = context;
            }

            public async Task<CommandResult> Handle(Command<TEntity> request, CancellationToken cancellationToken)
            {
                var entity = _context.Set<TEntity>().FindById(request.Id);
                if (entity == null) throw new NotFoundEntityException();

                entity.IsDeleted = true;

                await _context.SaveChangesAsync();

                return new CommandResult { Id = request.Id };
            }
        }
    }

}
