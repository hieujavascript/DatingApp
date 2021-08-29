using System.Collections.Generic;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessagesRepository
    {
        void AddGroup(Group groupName);
        void RemoveConnection(Connection connection);

        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessGroup(string groupName);

        void AddMessage(Message message);
        Task<Group> GetGroupForConnection(string connectionId);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id); // lay ra 1 message
        Task<PagedList<MessageDto>> GetMessageForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
       // Task<bool>SaveAllAsync(); thay bang unit of work

    }
}