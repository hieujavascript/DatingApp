using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using API.Dtos;
using API.Interfaces;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, 
        ITokenService tokenService,
        IMapper mapper)
        {
            this._tokenService = tokenService;
            this._mapper = mapper;
            this._context = context;
        }
        [HttpPost("register")]
        // public async Task<ActionResult<AppUser>> Register(string username , string password) {
        //  public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto) {
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // var user = new AppUser // ko dùng nữa vì đã lấy ra từ mapper
            // {
            //     UserName = registerDto.Username,
            //     PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
            //     PasswordSalt = hmac.Key
            // };
            using var hmac = new HMACSHA512(); // keywork using nghia la lấy xong rồi hủy;            
            // lay doi tuong AppUser tu Auto Mapper
            var user = this._mapper.Map<AppUser>(registerDto);
           // var cc = user.UserName;
            user.UserName = registerDto.Username;
            user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            // tracking user vao Entity Framework
            this._context.User.Add(user);
            // save user in database 
            await this._context.SaveChangesAsync();
           
            return new UserDto {
                Username = user.UserName , 
                Token = this._tokenService.CreateToken(user),
                knownAs = user.KnownAs , 
                Gender = user.Gender              
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
        {
            // kiem tra user co ton tai hay ko
            var user = await this._context.User
                                 .Include(p => p.Photos) // bao gom bang photos
                                 .SingleOrDefaultAsync(x => x.UserName == logindto.Username);
            if (user == null) return Unauthorized("Invalid UserName");
            // neu user ton tai thi lay PasswordHash so sanh
            using var hmac = new HMACSHA512(user.PasswordSalt);
            // password cua nguoi dung nhap vao
            var computerhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(logindto.Password));
            for (int i = 0; i < user.PasswordHash.Length; i++)
            {
                if (user.PasswordHash[i] != computerhash[i])
                    return Unauthorized("Password Invalid");
            }
            return  new UserDto {
                Username = user.UserName , 
                Token = this._tokenService.CreateToken(user),
                photoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                knownAs = user.KnownAs,
                Gender = user.Gender
            }; 
        }

    }
}