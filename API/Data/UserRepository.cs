using System.Collections.Generic;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using API.Dtos;
using AutoMapper.QueryableExtensions;
using System.Linq;
using AutoMapper;
using API.Helpers;
using System;

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
            var member = await this._context.Users
                        .Where(x => x.UserName == username)
                        // hoac dùng select(user = new MemBerDot( Id = user.Id . UserName = user.UserName  ) ) Mapper thủ công
                        .ProjectTo<MemberDto>(this._mapper.ConfigurationProvider) // mapper tự động
                        .SingleOrDefaultAsync();
                        return member;
           // throw new System.NotImplementedException();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            // AsQueryable tra ve IQueryable co thể next trong Router
            var query =  this._context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername);
            query = query.Where(u => u.Gender == userParams.Gender);
            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob );
            // query ket hop voi Parametter
            query = userParams.OrderBy switch {
                // sap xep theo giam dan truong nam trong AppUser
                "created" => query.OrderByDescending(u => u.Created), // created la ten tham so trong router
                _ => query.OrderByDescending(u => u.LastActive) // day la truong hop mac dinh
            };

            return await PagedList<MemberDto>.CreateAysnc(
            query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking() , 
            userParams.PageNumber , userParams.PageSize);
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            var user = await this._context.Users.FindAsync(id);
            return user;
        }

        public async Task<AppUser> GetUserByUserNameAsync(string username)
        {
            var user = await this._context.Users
                                .Include(p => p.Photos) // Join
                                .SingleOrDefaultAsync(
                x => x.UserName == username);
            return user;

                //  var query = await this._context.User
                //             .Include(p => p.Photos)
                //             .Where(u => u.UserName == username)
                //             .SingleOrDefaultAsync();
                // return query;

            
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            var users = await this._context.Users
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