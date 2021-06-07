using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikeRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public LikesRepository(DataContext context , IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
        {            
             return await this._context.Likes.FindAsync(sourceUserId , likeUserId);
            // throw new System.NotImplementedException();
        }
        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = this._context.User.OrderBy(u => u.UserName).AsQueryable();          
            var likes = this._context.Likes.AsQueryable();
            
            if(likeParams.Predicate == "liked")  { 
                
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId);

                users = likes.Select(like => like.LikedUser); // chi lay cot likeUser
            }
             if(likeParams.Predicate == "likedBy")  { // lay danh sach like ma user đang log like
                likes = likes.Where(like => like.LikedUserId == likeParams.UserId);
                users = likes.Select(like => like.SourceUser);
             }            
            //  return await users.Select(user => new LikeDto{
            //      Username = user.UserName,
            //      KnownAs = user.KnownAs,
            //      Age = user.DateOfBirth.CalculateAge(),
            //      PhotoUrl = user.Photos.SingleOrDefault(p => p.IsMain).Url,// select photo in AppUser
            //      City = user.City,
            //      Id = user.Id
            //  }).ToListAsync<LikeDto>();
              var likeUsers =  users.ProjectTo<LikeDto>(this._mapper.ConfigurationProvider);
              return await PagedList<LikeDto>.CreateAysnc(likeUsers , 
                                                          likeParams.PageNumber , 
                                                          likeParams.PageSize);
        }
         public async Task<AppUser> GetUserWithLikes(int userId)
        {
            // tra ve 1 user có like
            return await this._context.User
                        .Include(x => x.LikedUsers)
                        .SingleOrDefaultAsync(x => x.Id == userId);            
        }
        // public async Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId)
        // {
        //     var users = this._context.User.OrderBy(u => u.UserName).AsQueryable();          
        //     var likes = this._context.Likes.AsQueryable();
            
        //     if(predicate == "liked")  { 
                
        //         likes = likes.Where(like => like.SourceUserId == userId);

        //         users = likes.Select(like => like.LikedUser); // chi lay cot likeUser
        //     }
        //      if(predicate == "likedBy")  { // lay danh sach like ma user đang log like
        //         likes = likes.Where(like => like.LikedUserId == userId);
        //         users = likes.Select(like => like.SourceUser);
        //      }            
        //     //  return await users.Select(user => new LikeDto{
        //     //      Username = user.UserName,
        //     //      KnownAs = user.KnownAs,
        //     //      Age = user.DateOfBirth.CalculateAge(),
        //     //      PhotoUrl = user.Photos.SingleOrDefault(p => p.IsMain).Url,// select photo in AppUser
        //     //      City = user.City,
        //     //      Id = user.Id
        //     //  }).ToListAsync<LikeDto>();
        //       var like_dto =  await users.ProjectTo<LikeDto>(this._mapper.ConfigurationProvider).ToListAsync();
        //       return like_dto;           
        // }
       
    }
}