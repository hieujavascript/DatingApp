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
using Microsoft.AspNetCore.Identity;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        // private readonly DataContext _context;
        private readonly ITokenService _tokenService; // dependency tokenservice
        private readonly IMapper _mapper;

        public AccountController(
        // DataContext context;
        UserManager<AppUser> userManager , 
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IMapper mapper)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._tokenService = tokenService;
            this._mapper = mapper;
           // this._context = context;
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
           // using var hmac = new HMACSHA512(); // keywork using nghia la lấy xong rồi hủy;            
            // lay doi tuong AppUser tu Auto Mapper
            
            if (await this.UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user = this._mapper.Map<AppUser>(registerDto);
           // var cc = user.UserName;
            user.UserName = registerDto.Username;
            // user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password));
            // user.PasswordSalt = hmac.Key;

            // tracking user vao Entity Framework
            // this._context.Users.Add(user);
            // // save user in database 
            // await this._context.SaveChangesAsync();

            var result = await this._userManager.CreateAsync(user , registerDto.Password );            
            if(!result.Succeeded) return BadRequest(result.Errors);

            // add user vao role
            var role = await this._userManager.AddToRoleAsync(user , "Member");
            if(!role.Succeeded) return BadRequest(result.Errors);
            
            return new UserDto {
                Username = user.UserName , 
                Token = await this._tokenService.CreateToken(user),
                knownAs = user.KnownAs , 
                Gender = user.Gender              
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
        {
            // kiem tra user co ton tai hay ko
            // var user = await this._context.Users
            //                      .Include(p => p.Photos) // bao gom bang photos
            //                      .SingleOrDefaultAsync(x => x.UserName == logindto.Username);
            // if (user == null) return Unauthorized("Invalid UserName");
            // neu user ton tai thi lay PasswordHash so sanh
           // using var hmac = new HMACSHA512(user.PasswordSalt);
            // password cua nguoi dung nhap vao
           // var computerhash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(logindto.Password));
            // for (int i = 0; i < user.PasswordHash.Length; i++)
            // {
            //     if (user.PasswordHash[i] != computerhash[i])
            //         return Unauthorized("Password Invalid");
            // }
            // kiem tra username
            var user = await this._userManager.Users
                        .Include(p => p.Photos)
                        .SingleOrDefaultAsync(x => x.UserName == logindto.Username.ToLower());
            if(user == null) return Unauthorized("Invalid Username");
            // kiem tra password 
            var result = await this._signInManager.CheckPasswordSignInAsync(user , logindto.Password , false);
            if(! result.Succeeded) return Unauthorized();

            return  new UserDto {
                Username = user.UserName , 
                Token = await this._tokenService.CreateToken(user),
                photoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                knownAs = user.KnownAs,
                Gender = user.Gender
            }; 
        }

        private async Task<bool> UserExists(string username) {
            return await this._userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

    }
}