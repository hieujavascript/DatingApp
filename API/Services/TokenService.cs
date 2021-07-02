
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(
            IConfiguration config,
            UserManager<AppUser> userManager)
        { // 
            // chuoi Serect Key
                this._key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
            this._userManager = userManager;
            // _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }
    
        public async Task<string> CreateToken(AppUser user )
        {
            // tao Claim , de biet day la ai , nam giu thong tin gi                   
            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.NameId , user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName , user.UserName)
            };

            // duoc su dung khi chung ta Write Token
               var creds = new SigningCredentials(this._key, SecurityAlgorithms.HmacSha512Signature);
            // mo ta mot token
           
            // lay danh sach cac roles (Admin ,member , moderate)
            var roles = await this._userManager.GetRolesAsync(user);
            // add role vao ben trong Claim
            // select roles từ 1 list roles va TẠO MỚI 1 CLAIM để thêm  nó vào token bên dưới
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role , role)));
            
            // thêm Claim vào token
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims) , // xac nhan doi tuong Username duoc luuu trong claim
                Expires = DateTime.Now.AddDays(7) , // ngay  het han
                SigningCredentials = creds // chu ky hop le
            };
            // khoi tao doi tuong
            var tokenHandler = new JwtSecurityTokenHandler();
            // tao token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); // write token
        }

  
    }
}