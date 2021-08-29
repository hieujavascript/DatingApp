
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class AdminController : BaseAPIController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            this._userManager = userManager;         
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUserWithRoles()
        {
            var user = await _userManager.Users
                      .Include(u => u.UserRoles)
                      .ThenInclude(u => u.Role) // danh sach ten role 
                      .OrderBy(u => u.UserName)
                      .Select(u => new { // tra ve 1 userDOT vo danh
                          u.Id,
                          Username = u.UserName,
                          Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                      })
                      .ToListAsync();

            return Ok(user);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModerations()
        {
            return Ok("Admin or Moderate can see it");
        }
        // [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> EditResult(string username , [FromQuery] string roles) {
            var selectRoles = roles.Split(",").ToArray();
            // tìm ra đối tượng user
            var user = await this._userManager.FindByNameAsync(username);
            // trả về danh sách role name mà user đó đang nằm trong đó
            var userRoles = await this._userManager.GetRolesAsync(user); 
            // dd user vào role , neu trung ten thi Except se ko lay ten trugn lap do 
            var result =  await this._userManager.AddToRolesAsync(user , selectRoles.Except(userRoles));
            if(!result.Succeeded) return BadRequest("Faile add roles");
            // [FromQuery] = (1 , 3 , 5) userRoles (1,2,3 ,4,5,6)
            // tìm ra role name co ten (2 4 6) sau đó remove sẽ còn 1 , 3 , 5
            result = await this._userManager.RemoveFromRolesAsync(user , userRoles.Except(selectRoles));
            // if(!result.Succeeded) return BadRequest("Fail remove from role");

            return Ok(await this._userManager.GetRolesAsync(user));
        }
    }
}
