namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } // user Hien tai đã like
        public int SourceUserId { get; set; } // user Like

        public AppUser LikedUser { get; set; } // những user đã like
        public int LikedUserId { get; set; } // user được like
        
    }
}