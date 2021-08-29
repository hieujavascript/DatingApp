using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using API.Dtos;
namespace API.Entities
{
    public class Group  // tạo 1 Group và lưu giữ thông tin Username qua ConnectionId
    {
        // constructor rỗng cần cho EntityFramework
        public Group()
        {
        }

        public Group(string name)
        {
            Name = name;           
        }

        [Key]
        public string Name {get ; set;} // thêm  Key vì ta muốn Tên Group Name là Primary key duy nhất
        // khởi tạo new vì khi chúng ta tạo 1 Group nó se tự động tạo mới 1 list Connection
        // và chúng ta chỉ việc thêm Connection vào
        
        // Connection nó sẽ đc tữ động tạo ở cái class Connection , 
        //khi tạo group nó sẽ new  mới list<connection> thay vì phải khởi tạo trong contructor public Group(string name , connection)
        public ICollection<Connection> connections {get ; set ;} = new List<Connection>();
        
       
    }
}