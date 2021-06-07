
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            // chuoi Serect Key
                this._key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));                
           // _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // tao Claim , de biet day la ai , nam giu thong tin gi
            var claims = new List<Claim>
            {
                // new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
                   new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                   new Claim(JwtRegisteredClaimNames.NameId , user.Id.ToString())

            };  
            // duoc su dung khi chung ta Write Token
            // var creds = new SigningCredentials(_key , SecurityAlgorithms.HmacSha512Signature);
               var creds = new SigningCredentials(this._key, SecurityAlgorithms.HmacSha512Signature);
            // mo ta mot token
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