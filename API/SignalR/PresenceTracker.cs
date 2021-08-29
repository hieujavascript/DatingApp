using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
   
    public class PresenceTracker
    {
        // Dictionary gồm Key (username) và  Value (conectionID)
         private static readonly Dictionary<string,List<string>>  OnlineUser
                            = new();// Dictionary<string, List<string>>();
        // nhung user dang có Connection (tức đang online) thì add nó vào Dictionary                             
        public Task<bool> UserConnected(string username , string conectionId) {       
            // biến này cho ta biết một người vừa Login vào hay đã login từ trước
            
            Boolean isOnline = false; 
            lock(OnlineUser) {
                if(OnlineUser.ContainsKey(username))
                    {
                        //Add(conectionId) vao key [username]
                        //OnlineUser[username] sẽ trả về 1 list và ta sẽ dùng Method.Add 
                        OnlineUser[username].Add(conectionId);                        
                    }
                else 
                   {
                       //vì username chưa có trong Dictionary OnlineUser 
                       //khi isOnline là True thì có nghĩa một người mới vừa login vào 
                       // và ta sẽ cập nhật nó vào Onlineuser của Hub truyền về cho Client                     
                        isOnline = true;
                        OnlineUser.Add(username , new List<string> {conectionId});
                   } 
            }
            // return Task.CompletedTask;
            return Task.FromResult(isOnline);
        }
        // nhung user không có Connection (tức đang Offline) thì remove nó khỏi Dictionary 
        public Task<bool> DisUserConnected(string username , string conectionId) {
            Boolean isOffline = false;

            lock(OnlineUser) {
                //không có Username trong OnlineUser Dictionary  họ đã thoát
                if(!OnlineUser.ContainsKey(username)) 
                {
                    return Task.FromResult(isOffline);
                }
                //remove conection ID VỚI key Username
                OnlineUser[username].Remove(conectionId);
                if(OnlineUser[username].Count ==0)
                {
                //
                    isOffline = true;
                    OnlineUser.Remove(username);
                }

            }
            return Task.FromResult(isOffline);
        }
        public Task<string[]> GetOnlineUsers() {
            string[] onlineUsers;
            lock(OnlineUser) {
                onlineUsers = OnlineUser.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }            
            return  Task.FromResult(onlineUsers);
        }
        public  Task<List<string>> getConnectionForUser(string username) {
            List<string> connectionIds;
            lock(OnlineUser) {
                // nếu OnlineUser không chứa  username thì trả về null
                // có chưa thì trả về Value ConnectionId
                connectionIds = OnlineUser.GetValueOrDefault(username);
            }
             return Task.FromResult(connectionIds);
        }
    }
}