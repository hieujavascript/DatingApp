using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context; 
        public UsersController(DataContext context)
        {
            this._context = context;
        }
         // get data from client : https://localhost:5001/api/users
        [HttpGet]
        // public ActionResult<IEnumerable<AppUser>> getUsers() {
        //    var user = this._context.User.ToList();
        //    return user;
        // }
        public async Task<ActionResult<IEnumerable<AppUser>>> getUsers() {
            return await this._context.User.ToListAsync();
        }

         [HttpGet("{id}")]
         public async Task<ActionResult<AppUser>> getUser(int id) {
             return await this._context.User.FindAsync(id);
         }
        // public ActionResult<AppUser> getUser(int id) {
        //    var user = this._context.User.Find(id);
        //    return user;
        // }
    }

}