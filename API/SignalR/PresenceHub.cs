using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using API.Extensions;
using System;
using Microsoft.AspNetCore.Authorization;
using API.SignalR;
namespace API.SignalR
{
    [Authorize]
    public class PresenceHub: Hub // Một người vừa Online thì hệ thống Toast cho biết người đo đang Online
    {
        private readonly PresenceTracker _presenceTrack;

        public PresenceHub(PresenceTracker presenceTrack)
        {
            this._presenceTrack = presenceTrack;           
        }

        public override async Task OnConnectedAsync()
        {
           Boolean isOnline =  await this._presenceTrack.UserConnected(Context.User.getUserName() , Context.ConnectionId);
            //System.Security.Claims.ClaimsPrincipal extension lay tu token
           // var username =  Context.User.getUserName();
           // dùng để hiện Username thông báo bằng Toast với Other (tất cả mọi kết nối trừ kết nối của người đó)
           //  rằng người nào đang online 
           if(isOnline)
           {
            // send userOnline tới tất cả những người đang online Apart from caller
            await Clients.Others.SendAsync("UserIsOnline", Context.User.getUserName());
           }
            // lay tat ca user dang online
            var currentUser = await this._presenceTrack.GetOnlineUsers();

            // gửi về cho Client 1 mảng Các User đang online
            // không muốn gọi tất cả nên thay thế All bằng Call
            // await Clients.All.SendAsync("GetOnlineUsers" , currentUser);            
            await Clients.Caller.SendAsync("GetOnlineUsers" , currentUser);

        }
         public override async Task OnDisconnectedAsync(Exception exception){
             Boolean isOffline =  await this._presenceTrack.DisUserConnected(Context.User.getUserName() , Context.ConnectionId);
            
            // nếu isOffline là true , nghĩa là một người trong OnlineUser Dictionary đã thoát kết nối
            // ta sẽ lấy username của người đó và cập nhật remove khỏi danh sách online Dictionary
            if(isOffline) // thì ta sẽ remove họ khỏi danh sách đang online
            await Clients.Others.SendAsync("UserIsOffline", Context.User.getUserName());
            
            await base.OnDisconnectedAsync(exception);

            
        // === chúng ta  ko muốn send username ko onlien về client =======

            // // đây là danh sách cac User đang online
            //   var currentUser = await this._presenceTrack.GetOnlineUsers();
            // // gửi về cho Client , để client gọi hàm 
            // // this.hubConnection.on("UserIsOnline" , currentUser) trong Angular
            // await Clients.All.SendAsync("GetOnlineUsers" , currentUser);
            
         }
    }
}