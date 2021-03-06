using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    //public class AppUser 
     public class AppUser  :  IdentityUser<int>
    {
        // public int Id { get; set; }
        // public string UserName { get; set; }
        // public byte[] PasswordHash {get; set;}
        // public byte[] PasswordSalt {get ; set;}
        public DateTime DateOfBirth {get ; set;}
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string  Introduction { get; set; }
        public string LookingFor { get; set; }
        public string  Interests { get; set; }
        public string  City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; }
        
        // danh sach nhung user da like 
        //user dc like boi nhung user nay
        public ICollection<UserLike> LikedByUsers { get; set; }
        
        // danh sach nhung user dc like
        public ICollection<UserLike> LikedUsers { get; set; } 
        public ICollection<Message> MessageSent { get; set; } // gửi
        public ICollection<Message> MessageReceived { get; set; } // nhận

        // tao quan he voi AppUserRole , noi do JOIN AppUser và AppRole
       public ICollection<AppUserRole> UserRoles { get; set; }
        // public int GetAge() {
        //     return DateOfBirth.CalculateAge();
        // }
    }
}