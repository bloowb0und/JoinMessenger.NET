using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly IUserService _userService;
        private readonly IServerService _serverService;
        private readonly IChatService _chatService;
        private readonly IMessageService _messageService;

        public TestController(IUserService userService, 
            IServerService serverService, 
            IChatService chatService, 
            IMessageService messageService)
        {
            _userService = userService;
            _serverService = serverService;
            _chatService = chatService;
            _messageService = messageService;
        }

        [HttpPost]
        [Route("test")]
        public async Task<ActionResult<User>> TestMethod()
        {
            var user = new User
            {
                Login = "login1",
                Email = "login1@gmail.com",
                Name = "LoginName",
                Password = "qweRt12@eRt",
            };
            
            var user2 = new User
            {
                Login = "login2",
                Email = "login2@gmail.com",
                Name = "Login2Name",
                Password = "eRt@12qweRt",
            };

            await _userService.RegisterAsync(user);
            await _userService.RegisterAsync(user);
            await _userService.RegisterAsync(user2);

            await _serverService.CreateServerAsync("Server1");
            await _serverService.CreateServerAsync("Server2");
            
            var server1 = _serverService.GetServerByName("Server1");
            var server2 = _serverService.GetServerByName("Server2");
            
            await _serverService.AddUserAsync(server1, user);
            await _serverService.AddUserAsync(server1, user2);
            
            await _serverService.AddUserAsync(server2, user);
            await _serverService.AddUserAsync(server2, user2);

            
            var chat1 = new Chat
            {
                Name = "server1_chat1",
                Type = ChatType.Text,
                Server = server1
            };
            
            var msg1Chat1 = new Message
            {
                User = user,
                Chat = chat1,
                Value = "value1_msg1"
            };

            await _chatService.CreateChatAsync(chat1);
            await _messageService.CreateMessageAsync(msg1Chat1);

            var msg2Chat1 = new Message
            {
                User = user,
                Chat = chat1,
                Value = "value2_msg1"
            };
            
            await _messageService.CreateMessageAsync(msg2Chat1);

            var msg3Chat1 = new Message
            {
                User = user2,
                Chat = chat1,
                Value = "value3_msg1"
            };
            
            await _messageService.CreateMessageAsync(msg3Chat1);

            // -----------------------------------------

            var chat2 = new Chat
            {
                Name = "server1_chat2",
                Type = ChatType.Text,
                Server = server1
            };
            
            await _chatService.CreateChatAsync(chat2);
            
            var msg1Chat2 = new Message
            {
                User = user,
                Chat = chat1,
                Value = "value1_msg1_chat2"
            };
            
            await _messageService.CreateMessageAsync(msg1Chat2);

            var msg2Chat2 = new Message
            {
                User = user,
                Chat = chat1,
                Value = "value2_msg1_chat2"
            };
            
            await _messageService.CreateMessageAsync(msg2Chat2);

            var msg3Chat2 = new Message
            {
                User = user2,
                Chat = chat1,
                Value = "value3_msg1_chat2"
            };
            
            await _messageService.CreateMessageAsync(msg3Chat2);

            // ------------------------------------
            // ------------------------------------
            
            var chat1_1 = new Chat
            {
                Name = "server2_chat1",
                Type = ChatType.Text,
                Server = server2
            };
            
            await _chatService.CreateChatAsync(chat1_1);
            
            var msg1Chat1_1 = new Message
            {
                User = user,
                Chat = chat1_1,
                Value = "value1_msg1_1"
            };
            
            await _messageService.CreateMessageAsync(msg1Chat1_1);

            var msg2Chat1_1 = new Message
            {
                User = user,
                Chat = chat1_1,
                Value = "value2_msg1_2"
            };
            
            await _messageService.CreateMessageAsync(msg2Chat1_1);

            var msg3Chat1_1 = new Message
            {
                User = user2,
                Chat = chat1_1,
                Value = "value3_msg1_1"
            };
            
            await _messageService.CreateMessageAsync(msg3Chat1_1);

            // -----------------------------------------
            
            var chat2_2 = new Chat
            {
                Name = "server1_chat2_1",
                Type = ChatType.Text,
                Server = server2
            };
            
            await _chatService.CreateChatAsync(chat2_2);
            
            var msg1Chat2_2 = new Message
            {
                User = user,
                Chat = chat2_2,
                Value = "value1_msg1_chat2"
            };
            
            await _messageService.CreateMessageAsync(msg1Chat2_2);

            var msg2Chat2_2 = new Message
            {
                User = user,
                Chat = chat2_2,
                Value = "value2_msg1_chat2"
            };
            
            await _messageService.CreateMessageAsync(msg2Chat2_2);

            var msg3Chat2_2 = new Message
            {
                User = user2,
                Chat = chat2_2,
                Value = "value3_msg1_chat2"
            };
            
            await _messageService.CreateMessageAsync(msg3Chat2_2);

            return Ok(user);
        }
    }
}