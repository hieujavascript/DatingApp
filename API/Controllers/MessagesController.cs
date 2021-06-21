using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class MessagesController : BaseAPIController
    {
        private readonly IMessagesRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        // create constructor access Repository Dependency InJector
        public MessagesController(IMessagesRepository messageRepository , 
                                  IUserRepository userRepository ,
                                  IMapper mapper
                                  )
        {
            this._messageRepository = messageRepository;
            this._userRepository = userRepository;
            this._mapper = mapper;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessage) {                        
            var username = User.getUserName();
            // nguoi gui
            var sender = await this._userRepository.GetUserByUserNameAsync(username);
            // neu nguoi gui = nguoi nhan
            if(username == createMessage.RecipientUsername.ToLower()) 
            return BadRequest("you can't send message to your self");
            // nguoi nhan
            var recipient = await this._userRepository.GetUserByUserNameAsync(createMessage.RecipientUsername);
            if(recipient == null) return NotFound();

            // tạo message

            // gán những thứ cần thiết cho message
            var messageEntity = new Message {
                Sender = sender , 
                Recipient = recipient,
                Content = createMessage.Content,
                SenderUsername = username,
                RecipientUsername = recipient.UserName
            };
            this._messageRepository.AddMessage(messageEntity);
            
            if(await this._messageRepository.SaveAllAsync())
            return Ok(this._mapper.Map<MessageDto>(messageEntity));

            return BadRequest("Failed to send message");
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForUser(
        [FromQuery] MessageParams messageParams) {
            messageParams.Username = User.getUserName();
           var message = await this._messageRepository.GetMessageForUser(messageParams);
           // add vao header
           Response.AddPaginationHeader(message.CurrentPage , 
                                        message.PageSize , 
                                        message.TotalCount , 
                                        message.TotalPages);
           return message;
        }
       [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageForThread(string username) {
          var currentUsername = User.getUserName();         
           var message =  await this._messageRepository.GetMessageThread(currentUsername , username);
           return Ok(message);
        } 
        [HttpDelete("{id}")] 
        public async Task<ActionResult> DeleteMessae(int id) {
          
            var username = User.getUserName(); // day la username login

            var message  = await this._messageRepository.GetMessage(id);
            if(message.Sender.UserName != username && message.Recipient.UserName != username)
            return Unauthorized();
            if(message.Sender.UserName == username) 
               message.SenderDelete = true;
            if(message.Recipient.UserName == username) 
                message.RecipientDelete = true;
            this._messageRepository.DeleteMessage(message);
            if(await this._messageRepository.SaveAllAsync()) 
            return Ok();
            
            return BadRequest("Problem deleting the message");
        }

    }
}