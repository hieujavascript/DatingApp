using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;
namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByUserNameAsync(string username);
        Task<AppUser> GetUserByIdAsync(int id);
        //Task<IEnumerable<MemberDto>> GetMembersAsync();
        
        // khai bao pagelist cùng với query trên router UserParams la tham số
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams); 
        Task<MemberDto> GetMemberAsync(string username);
    }
}