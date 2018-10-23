using System.Threading.Tasks;

namespace Api.Infra.Crud
{

    public class CreateHandler<TCommand, TEntity> : IRequestHandler<TCommand, CommandResult>
            where TCommand : IRequest<CommandResult>
            where TEntity : class, IEntity
    {
        private readonly IMapper _mapper;
        private readonly ApiDbContext _context;

        public CreateHandler(IMapper mapper, ApiDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<CommandResult> Handle(TCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<TEntity>(request);

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            return _mapper.Map<CommandResult>(entity);
        }
    }

}
