using System;

namespace API.Dtos
{
    public class MessageDto
    {
        public int Id { get; set; } // Value se tu dong sinh ra trong database
        public int SenderId { get; set; } // tu dong tro thanh khgoa ngoai
        // relationship giua nguoi gui va message
        public string  SenderUsername { get; set; }
        // public AppUser Sender { get; set; }
        public string SenderPhotoUrl { get; set; }
        //=======================================
        public int RecipientId { get; set; }// tu dong tro thanh khgoa ngoai
        // relationship giua nguoi nhan va message
        public string RecipientUsername { get; set; }
        // public AppUser Recipient { get; set; }
        public string RecipientPhotoUrl { get; set; }
        // =======================================
        public string Content { get; set; }
        public DateTime? DateRead { get; set; } // co the null
        //  public DateTime MessageSend { get; set; } = DateTime.Now; // ngay hien hanh
        public DateTime MessageSend { get; set; }
        // public bool SenderDelete { get; set; }
        // public bool RecipientDelete { get; set; }
    }
}