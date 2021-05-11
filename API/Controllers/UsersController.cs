using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.Dtos;
using AutoMapper;

namespace API.Controllers
{
    //=============  IMPORT in BaseAPIController ===========
    // [ApiController]
    // [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseAPIController
    {
      //  private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository , IMapper mapper)
        {
            this._userRepository = userRepository;
            this._mapper = mapper;
        }
        // get data from client : https://localhost:5001/api/users
        // public ActionResult<IEnumerable<AppUser>> getUsers() {
        //    var user = this._context.User.ToList();
        //    return user;
        // }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> getUsers()
        {
            // var users = await this._userRepository.GetUsersAsync();
            // var usersReturn = this._mapper.Map<IEnumerable<MemberDto>>(users);
            // return Ok(usersReturn);
            // return await this._context.User.ToListAsync();
            var users = await this._userRepository.GetMembersAsync();
            return Ok(users);
        }
        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> getUser(string username)
        {
            //  var user = await this._userRepository.GetUserByUserNameAsync(username);
            //  var userReturn = this._mapper.Map<MemberDto>(user);
            // return userReturn;

            var user = await this._userRepository.GetMemberAsync(username);
            return user;
       
        }
    }

}