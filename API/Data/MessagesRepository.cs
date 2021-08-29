using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MessagesRepository(DataContext context , IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public void AddMessage(Message message)
        {
            this._context.Messages.Add(message);
            //throw new System.NotImplementedException();
        }

        public void DeleteMessage(Message message)
        {
            this._context.Messages.Remove(message);
           // throw new System.NotImplementedException();
        }

        public async Task<Message> GetMessage(int id)
        {
            //return await this._context.Messages.FindAsync(id); find id ko the Join
            var message = await this._context.Messages
                                    .Include(s => s.Sender)
                                    .Include(r => r.Recipient)
                                    .SingleOrDefaultAsync(m => m.Id == id);
            return message;
           // throw new System.NotImplementedException();
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, 
                                                                string recipientIsername)
        {            
            // Where trong bộ nhớ không thực hiện ở database
            var messages = await  this._context.Messages.AsSplitQuery()
            // Load Photo cho User
                         // .Include(u => u.Sender).ThenInclude(p => p.Photos)
                        //  .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            // ==================  
                          .Where(
                            m => 
                                m.Recipient.UserName == currentUsername 
                                && m.Sender.UserName == recipientIsername
                                && m.RecipientDelete == false
                                || 
                                m.Recipient.UserName == recipientIsername
                                && m.Sender.UserName == currentUsername
                                && m.SenderDelete == false
                          ).OrderBy(m => m.MessageSend)
                          // trả về MessageDto cho TolistAsync();
                          .ProjectTo<MessageDto>(this._mapper.ConfigurationProvider)
                          .ToListAsync(); //IEnumerable<MessageDto>
                          
            // cần lặp qua 1 ít nên phải tách ra để không duyệt nhiều trong vòng lặp
            // Thư lisa gửi thì khi Todd Login thư của Lisa gửi sẽ bị đánh dấu đã xem và ngược lại
            
            var unreadMessage = messages.Where(
                                            m => m.DateRead == null 
                                            && m.RecipientUsername == currentUsername            
                                            ).ToList();
            // xác định xem 1 CHUỖI có chưa PHẦN TỬ không ?                                            
            if(unreadMessage.Any())
            {
                foreach ( var message in unreadMessage) {
                    //message.DateRead = DateTime.Now;
                    message.DateRead = DateTime.UtcNow;
                }
            }
            
            //await this._context.SaveChangesAsync(); // save va cap nhat lai trong database
            return messages; 
            //da dùng  .ProjectTo<MessageDto>(this._mapper.ConfigurationProvider) nên Return Message là đc
            // return this._mapper.Map<IEnumerable<MessageDto>>(messages); 

            // Queryable Extensions
            // return  messages.ProjectTo<MessageDto>(this._mapper.ConfigurationProvider);  

        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
           var query = this._context.Messages
                           .OrderByDescending(d => d.MessageSend)
                           .ProjectTo<MessageDto>(this._mapper.ConfigurationProvider)
                           .AsQueryable();
            query = messageParams.Container switch {
                "Inbox" =>  query.Where(u => u.RecipientUsername == messageParams.Username 
                                             && u.RecipientDelete == false)  ,
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
                                             && u.SenderDelete == false ) ,
                // mac dinh hien thi mess voi dataRead la null                                              
                       _ => query.Where(u => u.RecipientUsername == messageParams.Username 
                                             && u.RecipientDelete == false 
                                             && u.DateRead == null)
            };
            // ta dùng ProjectTo trả về từ câu Query cho tiện
           // var message = query.ProjectTo<MessageDto>(this._mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAysnc(query , 
                                                           messageParams.PageNumber ,
                                                           messageParams.PageSize);
        }

        // thay abng Unit Of Work
        // public async Task<bool> SaveAllAsync()
        // {
        //     return await this._context.SaveChangesAsync() > 0;      
        //     //throw new System.NotImplementedException();
        // }

        public void RemoveConnection(Connection connection)
        {
            this._context.Connections.Remove(connection);
           // throw new NotImplementedException();
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await this._context.Connections.FindAsync(connectionId);
           // throw new NotImplementedException();
        }
        public void AddGroup(Group group)
        {
            this._context.Groups.Add(group);
           // throw new NotImplementedException();
        }
        public async Task<Group> GetMessGroup(string groupName)
        {
            return await this._context.Groups
                    .Include(c => c.connections)                     
                    .FirstOrDefaultAsync(g => g.Name == groupName); 
                    //  faultAsyncko co se tra ve null chu ko throw error nhu SingleOrDefaultAsync
           // throw new NotImplementedException();
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            var Group =  await this._context.Groups
                                 .Include(c => c.connections)
                                 .Where(c => c.connections.Any(x => x.ConnectionId == connectionId ))
                                 .FirstOrDefaultAsync();
            return Group;
        }
    }
}