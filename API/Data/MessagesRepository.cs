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
                          .Include(u => u.Sender).ThenInclude(p => p.Photos)
                          .Include(u => u.Recipient).ThenInclude(p => p.Photos)
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
                          .ToListAsync();
                          
            // cần lặp qua 1 ít nên phải tách ra để không duyệt nhiều trong vòng lặp
            // Thư lisa gửi thì khi Todd Login thư của Lisa gửi sẽ bị đánh dấu đã xem và ngược lại
            
            var unreadMessage = messages.Where(
                                            m => m.DateRead == null 
                                            && m.Recipient.UserName == currentUsername            
                                            ).ToList();
            // xác định xem 1 CHUỖI có chưa PHẦN TỬ không ?                                            
            if(unreadMessage.Any())
            {
                foreach ( var message in unreadMessage) {
                    message.DateRead = DateTime.Now;
                }
            }
            await this._context.SaveChangesAsync(); // save va cap nhat lai trong database
            
            return this._mapper.Map<IEnumerable<MessageDto>>(messages);
            // Queryable Extensions
            // return  messages.ProjectTo<MessageDto>(this._mapper.ConfigurationProvider);  

        }

        public async Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams)
        {
           var query = this._context.Messages
                           .OrderByDescending(d => d.MessageSend)
                           .AsQueryable();
            query = messageParams.Container switch {
                "Inbox" =>  query.Where(u => u.Recipient.UserName == messageParams.Username 
                                             && u.RecipientDelete == false)  ,
                "Outbox" => query.Where(u => u.Sender.UserName == messageParams.Username
                                             && u.SenderDelete == false ) ,
                // mac dinh hien thi mess voi dataRead la null                                              
                       _ => query.Where(u => u.Recipient.UserName == messageParams.Username 
                                             && u.RecipientDelete == false 
                                             && u.DateRead == null)
            };
            var message = query.ProjectTo<MessageDto>(this._mapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAysnc(message , 
                                                           messageParams.PageNumber ,
                                                           messageParams.PageSize);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await this._context.SaveChangesAsync() > 0;      
            //throw new System.NotImplementedException();
        }
    }
}