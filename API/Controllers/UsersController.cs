using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Dtos;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using API.Entities;
using API.Extensions;
using System.Linq;
using API.Helpers;
using API.extensions;

namespace API.Controllers
{
    //=============  IMPORT in BaseAPIController ===========
    // [ApiController]
    // [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseAPIController
    {
        //  private readonly DataContext _context;
        // private readonly IUserRepository _unitOfWork.UserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUnitOfWork unitOfWork , IMapper mapper , IPhotoService photoService)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            this._photoService = photoService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
            // var users = await this._unitOfWork.UserRepository.GetUsersAsync();
            // var usersReturn = this._mapper.Map<IEnumerable<MemberDto>>(users);
            // return Ok(usersReturn);
            // return await this._context.User.ToListAsync();
           
            var username = User.getUserName();
            var gender = await this._unitOfWork.UserRepository.GetUserGender(username);
            userParams.CurrentUsername = User.getUserName() ; // lấy từ token
            // neu router trinh duyet ma Null thi gan nguoc gender 
            // lisa is female thi router se la male
            if(string.IsNullOrEmpty(userParams.Gender))
            {
               userParams.Gender = gender == "male" ? "female" : "male";
              // userParams.Gender = gender; //  set giá trị Gender cho UserParam
            }
            var users = await this._unitOfWork.UserRepository.GetMembersAsync(userParams);
            // cập nhật lại header cho client sau khi thay doi Tham so Query o Router trinh duyet
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize,
                users.TotalCount, users.TotalPages);
            return Ok(users);
        }
       // [Authorize(Roles = "Member") ]
        [HttpGet("{username}" , Name = "getUser")]
        public async Task<ActionResult<MemberDto>> getUser(string username)
        {
            //  var user = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            //  var userReturn = this._mapper.Map<MemberDto>(user);
            // return userReturn;

            var user = await this._unitOfWork.UserRepository.GetMemberAsync(username);
            return user;
        }
        [HttpPut]
        public async Task<ActionResult<MemberDto>> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // lay ra username login 
            var username = User.getUserName(); //.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // var usernames = User.FindAll(ClaimTypes.NameIdentifier)?.GetEnumerator();
            var user = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            // neu ko map kieu  this._mapper.Map(memberUpdateDto , user); nay phai nhap thu cong nhu vay 
            // user.city =  memberUpdateDto.city lam tung cai mot
            this._mapper.Map(memberUpdateDto, user); // tu dong map va gán TẤT CẢ value tu DTO qua user
            this._unitOfWork.UserRepository.Update(user);
            bool status = await this._unitOfWork.Complete();
            if (status == true) return NoContent();
            return BadRequest("can not update member dto");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            
            var username = User.getUserName();
            var user = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            // upload len Icloud
            var result = await this._photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            // photo vao photos trong user
            user.Photos.Add(photo);
            if (await this._unitOfWork.Complete()) // neu save vao database successful
            {
                var photo_dto = this._mapper.Map<PhotoDto>(photo);
                return CreatedAtRoute("getUser" , new {username = user.UserName} , photo_dto);
                // return this._mapper.Map<PhotoDto>(photo); tra ve truc tiep ko co trong than Body
            }
            return BadRequest("Update faild");
        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var username = User.getUserName();
            var user = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            // current photo
            var photo = user.Photos.FirstOrDefault(x =>x.Id == photoId );
            // khong cap nhat isMain = true khi ban than no la isMain = true
            if(photo.IsMain == true) return BadRequest("This is already your main Photo");
            // neu IsMain la false
            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
            // neu cai IsMain hien tai = true set no la false             
            if(currentMain != null) currentMain.IsMain = false;
            // cập nhạt cai IsMain mới mà ta nhap vao tu trinh duyet là true
            photo.IsMain = true;
            if(await this._unitOfWork.Complete()) return NoContent();
            // neu có lỗi xảy ra
            return BadRequest("Can't Update IsMain Photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId) {
            
            var username = User.getUserName();
            var user = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(username);
             // tim photo can delete
            var photo =  user.Photos.FirstOrDefault(x =>x.Id == photoId);
            if(photo == null) return NotFound();
            if(photo.IsMain) return BadRequest("you can't delete your main photo");
            if(photo.PublicId != null) {
                // Delete tren Icloud
                var result = await this._photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error !=null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);            
            if(await this._unitOfWork.Complete()) return NoContent();
            // neu có lỗi xảy ra
            return BadRequest("Can't Update IsMain Photo");


        }
    }

}