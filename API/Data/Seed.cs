using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
         public static async Task SeedUsers(DataContext context) {
             
             // neu co phan tu thi tra ve moi phan tu
             if(await context.User.AnyAsync()) return; 
             
             // neu khong co thi doc file Json
             var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
             var users =  JsonSerializer.Deserialize<IList<AppUser>>(userData) ;
             foreach (var user in users)
             {
                 using var hmac = new HMACSHA512();
                 user.UserName = user.UserName.ToLower();
                 user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                 user.PasswordSalt = hmac.Key;
                 context.User.Add(user);
             }
             await context.SaveChangesAsync();
         }
    }
}