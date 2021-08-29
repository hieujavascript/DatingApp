using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
namespace API.Controllers
{
    [Authorize]
    public class LikesController : BaseAPIController
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikesController(IUnitOfWork unitOfWork )
        {
            this._unitOfWork = unitOfWork;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username) {
            // user login
            var sourceUserId = User.getUserId();
            // lay user khi user khi login
            var likeUser = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            // tra ve User co like
            var sourceUser = await this._unitOfWork.LikeRepository.GetUserWithLikes(sourceUserId);

            if(likeUser == null) return NotFound();// khong tim thay user ma ho muôn like
            if(sourceUser.UserName == username) return BadRequest("you can't like your self");
                      
            // nếu user like người mình đã like==========================================
            var userLike = await this._unitOfWork.LikeRepository.GetUserLike(sourceUserId , likeUser.Id);
            if(userLike != null) return BadRequest("you already like this user");
            // ==========================================================================
            // neu khong co thi tao moi
            userLike = new UserLike {
                SourceUserId =  sourceUserId , 
                LikedUserId = likeUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if(await this._unitOfWork.Complete()) return Ok();

            return BadRequest("Fail to like user");
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery]LikeParams likeParams)
        {
            // sử dụng 2 tham số nen tao 2 property trong likeParam la predicate , UserId
            likeParams.UserId = User.getUserId();
            // tra ve 1 page list cho user
            var users = await this._unitOfWork.LikeRepository.GetUserLikes(likeParams);
            // add vài Header là một Extension để client lấy ra
            Response.AddPaginationHeader(users.CurrentPage , 
                                        users.PageSize , 
                                        users.TotalCount , 
                                        users.TotalPages);
            return Ok(users);
        }
        // public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes(string predicate)
        // {
        //     // sử dụng 2 tham số nen tao 2 property trong likeParam la predicate , UserId
        //     var users = await this._unitOfWork.LikeRepository.GetUserLikes(predicate , User.getUserId());
        //     return Ok(users);
        // }
    }
}