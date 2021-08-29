using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
         //public static async Task SeedUsers(DataContext context) {
         public static async Task SeedUsers(
             UserManager<AppUser> userManager , // tạo user thay cho context.User.Add
             RoleManager<AppRole> roleManager // thêm role
             ) {
             // AnyAsync xac  dinh xem 1 chuỗi co chua phan tu hay khong ? 
             if(await userManager.Users.AnyAsync()) return;              
             // neu khong co thi doc file Json
             var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
             var users =  JsonSerializer.Deserialize<IList<AppUser>>(userData) ;
             if(users == null)  return;

            // create roles and get by use function  _userManager.getRoleAsynce()
             var roles = new List<AppRole> {
                 new AppRole {Name = "Member"}, 
                 new AppRole {Name = "Admin"}, 
                 new AppRole {Name = "Moderator"}             
             }; 
             // add role in RoleManager
             foreach (var role in roles)
             {
                 await roleManager.CreateAsync(role);
             }

             foreach (var user in users)
             {
                // using var hmac = new HMACSHA512();
                 user.UserName = user.UserName.ToLower();
                //  user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                //  user.PasswordSalt = hmac.Key;
               //  context.Users.Add(user);
               //  await userManager.CreateAsync(user , "Pa$$w0rd" );
               await userManager.CreateAsync(user , "123456Asd" );
                // Add user in Role Member
                await userManager.AddToRoleAsync(user , "Member");                          
             }
            //set Admin
            var admin = new AppUser {
    // viet thuong , no se tham 1 username ten Admin vao ben trong AppUser de login quyen admin
                UserName = "admin"  // UserName viet hoa vi trong AppUser viet nhu the
            };
            //await userManager.CreateAsync(admin , "Pa$$w0rd");
            await userManager.CreateAsync(admin , "123456Asd");
            await userManager.AddToRolesAsync(admin , new[] {"Admin" , "Moderator"} );   
                            
            // await userManager.AddToRolesAsync(admin , new List<string>{"Admin" , "Moderator"} );
            // await context.SaveChangesAsync();
         }
    }
}