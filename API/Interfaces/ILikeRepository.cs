using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikeRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId , int likeUserId); // chinh xac user dc like
        Task<AppUser> GetUserWithLikes(int userId); // lay ra user đã like
        
        // lấy ra danh sách nguoi da like
        // useID = 1 da like cho bao nhieu nguoi
        // Task<IEnumerable<LikeDto>> GetUserLikes(string predicate , int userId); 
        Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams); 
    }
}