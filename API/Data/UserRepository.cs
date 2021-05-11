using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.Dtos;
using AutoMapper.QueryableExtensions;
using System.Linq;
using AutoMapper;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            var member = await this._context.User
                        .Where(x => x.UserName == username)
                        // hoac dùng select(user = new MemBerDot( Id = user.Id . UserName = user.UserName  ) ) Mapper thủ công
                        .ProjectTo<MemberDto>(this._mapper.ConfigurationProvider) // mapper tự động
                        .SingleOrDefaultAsync();
                        return member;
           // throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            var members = await this._context.User
                          .ProjectTo<MemberDto>(this._mapper.ConfigurationProvider)
                          .ToListAsync();
                          return members;
          //  throw new System.NotImplementedException();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user = await this._context.User.FindAsync(id);
            return user;
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            var user = await this._context.User
                                .Include(p => p.Photos)
                                .SingleOrDefaultAsync(
                x => x.UserName == username);
            return user;
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var users = await this._context.User
                                    .Include(p => p.Photos)
                                    .ToListAsync();
            return users;
            // throw new System.NotImplementedException();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await this._context.SaveChangesAsync() > 0;
            // throw new System.NotImplementedException();
        }

        public void Update(AppUser user)
        {
            this._context.Entry(user).State = EntityState.Modified;
            // throw new System.NotImplementedException();
        }
    }
}