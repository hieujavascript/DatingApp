using System;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class MessageHub : Hub // nhớ add nó vào startup.cs end point
    {
        private readonly IUnitOfWork _unitOfWork;

        // private readonly IMessagesRepository _unitOfWork.MessagesRepository;
        private readonly IMapper _mapper;
      //  private readonly IUserRepository _unitOfWork.UserRepository;
        private readonly IHubContext<PresenceHub> _presencehub;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(IUnitOfWork unitOfWork ,
                        //IMessagesRepository messageRepository, 
                          IMapper mapper , 
                         // IUserRepository userRepository ,
                          IHubContext<PresenceHub> presencehub,
                          PresenceTracker presenceTracker
                          )
        {
          //  this._unitOfWork.MessagesRepository = messageRepository;
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
           // this._unitOfWork.UserRepository = userRepository;
            this._presencehub = presencehub;
            this._presenceTracker = presenceTracker;
        }
        public override async Task OnConnectedAsync() {
            // lấy otherUser từ Request Querystring
            var httpContext = Context.GetHttpContext();
            // tra về từ Service Angular gọi Hub trong phần Connec
            var otherUser = httpContext.Request.Query["user"].ToString();   
            
            var groupName = this.getGroupName(Context.User.getUserName() , otherUser);
            // Add GroupName vào Group của Hub
            await Groups.AddToGroupAsync(Context.ConnectionId , groupName);
            // Add GroupName vào Group trong Database            
            var Group =  await this.AddToGroup(groupName);          
            await Clients.Group(groupName).SendAsync("UpdateGroup" , Group);
            var messages = await this._unitOfWork.MessagesRepository.GetMessageThread(Context.User.getUserName() , otherUser);            
            
            if(this._unitOfWork.HasChange()) await this._unitOfWork.Complete();

            // send message ReceiveMessageThread cho người vừa Join vào kết nối của Hub này
            await Clients.Caller.SendAsync("ReceiveMessageThread" , messages);
        //  await Clients.Group(groupName).SendAsync("ReceiveMessageThread" , messages);
        }
        public override async Task OnDisconnectedAsync(Exception exception){
            // Remove Connection từ Database
           var Group = await this.RemoveConnectionFromMessageGroup();
            await Clients.Group(Group.Name).SendAsync("UpdateGroup" , Group);
          //  var connec =  this._unitOfWork.MessagesRepository.GetConnection(Context.ConnectionId);
            // khi họ thoát thì Group sẽ tự động remove họ khỏi nhóm            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task CreateMessage(CreateMessageDto createMessage) {                        
            var username = Context.User.getUserName();
            // nguoi gui
            var sender = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(username);
            // neu nguoi gui = nguoi nhan
            if(username == createMessage.RecipientUsername.ToLower()) 
            throw new HubException ("you can't send message to your self");
            // nguoi nhan
            var recipient = await this._unitOfWork.UserRepository.GetUserByUserNameAsync(createMessage.RecipientUsername);
            if(recipient == null) throw new HubException("not found");

            // tạo message

            // gán những thứ cần thiết cho message
            var messageEntity = new Message {
                Sender = sender , 
                Recipient = recipient,
                Content = createMessage.Content,
                SenderUsername = username,
                RecipientUsername = recipient.UserName
            };
            this._unitOfWork.MessagesRepository.AddMessage(messageEntity);
            
            // nếu người nhận message họ có đang kết nối (OnConnectedAsync)
             var groupName = getGroupName(sender.UserName , recipient.UserName);
             // lấy ra Group(vì trong Group có Connection chứa Username người Nhận)
             // kiểm tra xem họ đang có nằm trong hàm OnConnectedAsync , do chúng ta có AddToGroup thì họ có Username
             var group = await this._unitOfWork.MessagesRepository.GetMessGroup(groupName);
            if(group.connections.Any(u => u.Username == recipient.UserName)) {
                messageEntity.DateRead = DateTime.UtcNow;// set ngay tháng là hiện tại cùng thời gian
            }
            else { 
                // user đang khong có connection trogn Group nhưng đã đăng nhập và đang online trên presencehub 
                // connection của người nhân
                var connections = await this._presenceTracker.getConnectionForUser(recipient.UserName);
                if(connections != null) {
                // sẽ gọi ON của connection trên Client ở bất cứ đâu khi họ kết nối với presencehub
                // điều này cho ta biết một người hiện đang Online và ko có connection trong GROUP NAME
                await this._presencehub.Clients.Clients(connections)
                                               .SendAsync("NewMessageRecived" , new {username = sender.UserName , knownAs = sender.KnownAs} );
                }
            }


            // có add vào Entity thi có SavwAllChange để lưu vào Database
            if(await this._unitOfWork.Complete()) 
            {
                //var groupName = getGroupName(sender.UserName , recipient.UserName);
                // client gọi hàm NewMessage thì sẽ nhận đc MessageDto
                await Clients.Group(groupName).SendAsync("NewMessage" , this._mapper.Map<MessageDto>(messageEntity));
            }


            // return BadRequest("Failed to send message");
        }

        // sắp xếp tên sẽ theo thứ tự Alphabet
        private string getGroupName(string call , string other) {
            // var stringCompare = string.CompareOrdinal(call , other) < 0;
            // return stringCompare ? $"{call}-{other}" : $"{other}-{call}";
            int stringCompare = call.CompareTo(other);
            var str = "";
            if(stringCompare <= 0) str = $"{call}-{other}";
            else str=  $"{other}-{call}";
            return str;            
        }

        public async Task NewMessage(Message msg)  
        {  
            await Clients.All.SendAsync("MessageReceived", msg);  
        }  

        // Add GroupName , cONNECTION, USERNAME vào trong Group
        private async Task<Group> AddToGroup(string groupName)
        {            
            
            // lay Group từ database  xem có hay không ?
            var group = await  this._unitOfWork.MessagesRepository.GetMessGroup(groupName);
           // Nếu chưa có Group với cái groupName đó trong database thì tạo mới
            if(group == null)
            {
                // tạo mới class Group truyền dữ liệu vào Constructor để gán giá trị và sử dụng
                group = new Group(groupName); 
                // add group của Hub vào trong  Entity Database Context
                this._unitOfWork.MessagesRepository.AddGroup(group); // lấy ra bằng GetMessGroup  (1)             
            }            
            // còn nếu đã tồn tại Group với groupName trong database rồi
            //thì mỗi khi 1 người vào ta chỉ việc add ConnectionId của họ vào Group Name đó
            var connection = new Connection(Context.ConnectionId , Context.User.getUserName());

            group.connections.Add(connection); // add connection vào class Group Hub

            if (await this._unitOfWork.Complete())  // save vào database   (2)
            return group;

            throw new HubException("Failed to join Group");
             
        }
        // khi không connec thì xóa dữ liệu trong bảng Connection
        private async Task<Group> RemoveConnectionFromMessageGroup() {
            var group = await this._unitOfWork.MessagesRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            this._unitOfWork.MessagesRepository.RemoveConnection(connection);
            if ( await this._unitOfWork.Complete())
            return group;

            throw new HubException("fail remove connection to group");  
        }

    }
}