using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UnitOfWork(DataContext context , IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(this._context , this._mapper);

        public ILikeRepository LikeRepository => new LikesRepository(this._context , this._mapper);
        public IMessagesRepository MessagesRepository => new MessagesRepository(this._context , this._mapper);

        public async Task<bool> Complete()
        {
            return await this._context.SaveChangesAsync() >0;
        }

        public bool HasChange()
        {
            return this._context.ChangeTracker.HasChanges();
        }
    }
}