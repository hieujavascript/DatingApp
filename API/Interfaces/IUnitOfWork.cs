using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
         public IUserRepository UserRepository { get; }
         public ILikeRepository LikeRepository { get; }
         public IMessagesRepository MessagesRepository { get;}
        Task<bool> Complete();
        bool HasChange();
    }
}