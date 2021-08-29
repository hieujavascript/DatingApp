namespace API.Entities
{
    public class Connection // để sử dụng phải tạo Constructor truyền tham số vào
    {
        // Entity Framework cần Constructor rỗng nếu ko sẽ báo lỗi
        public Connection()
        {
        }
        // sử dụng tạo Connection
        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }
        public string Username { get; set; }
    }
}