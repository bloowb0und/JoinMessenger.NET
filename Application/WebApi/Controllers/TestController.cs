using System;
using System.Linq;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using FluentResults;
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

            Result res;
            // if ((res = await _userService.RegisterAsync(user)).IsFailed)
            // {
            //     return BadRequest(res.Errors.Select(e => e.Message));
            // }
            if ((res = await _userService.RegisterAsync(user)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            
            if ((res = await _userService.RegisterAsync(user2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            if ((res = await _serverService.CreateServerAsync("Server1")).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            if ((res = await _serverService.CreateServerAsync("Server2")).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            if ((res = await _serverService.CreateServerAsync("Server2")).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            
            if ((res = await _serverService.CreateServerAsync("Server2")).IsFailed) // error
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            Result<Server> serverResult;
            
            if ((serverResult = _serverService.GetServerByName("Server1")).IsFailed)
            {
                Console.WriteLine(serverResult.Errors.Select(e => e.Message));
            }
            var server1 = serverResult.Value;
            
            if ((serverResult = _serverService.GetServerByName("Server2")).IsFailed)
            {
                Console.WriteLine(serverResult.Errors.Select(e => e.Message));
            }
            var server2 = serverResult.Value;
            
            if ((serverResult = _serverService.GetServerByName("Server56")).IsFailed) // error
            {
                Console.WriteLine(serverResult.Errors.Select(e => e.Message));
            }

            if ((res = await _serverService.AddUserAsync(server1, user)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            if((res = await _serverService.AddUserAsync(server1, user2)).IsFailed) 
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            
            if((res = _serverService.AddUserAsync(server1, user2).Result).IsFailed) // error
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            if ((res = await _serverService.AddUserAsync(server2, user)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            if ((res = await _serverService.AddUserAsync(server2, user2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

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

            if ((res = await _chatService.CreateChatAsync(chat1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            if ((res = await _messageService.CreateMessageAsync(msg1Chat1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg2Chat1 = new Message
            {
                User = user,
                Chat = chat1,
                Value = "value2_msg1"
            };

            if ((res = await _messageService.CreateMessageAsync(msg2Chat1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg3Chat1 = new Message
            {
                User = user2,
                Chat = chat1,
                Value = "value3_msg1"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg3Chat1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            // -----------------------------------------

            var chat2 = new Chat
            {
                Name = "server1_chat2",
                Type = ChatType.Text,
                Server = server1
            };
            
            if ((res = await _chatService.CreateChatAsync(chat2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            
            var msg1Chat2 = new Message
            {
                User = user,
                Chat = chat1,
                Value = "value1_msg1_chat2"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg1Chat2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg2Chat2 = new Message
            {
                User = user,
                Chat = chat1,
                Value = "value2_msg1_chat2"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg2Chat2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg3Chat2 = new Message
            {
                User = user2,
                Chat = chat1,
                Value = "value3_msg1_chat2"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg3Chat2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            // ------------------------------------
            // ------------------------------------
            
            var chat1_1 = new Chat
            {
                Name = "server2_chat1",
                Type = ChatType.Text,
                Server = server2
            };
            
            if ((res = await _chatService.CreateChatAsync(chat1_1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            
            var msg1Chat1_1 = new Message
            {
                User = user,
                Chat = chat1_1,
                Value = "value1_msg1_1"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg1Chat1_1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg2Chat1_1 = new Message
            {
                User = user,
                Chat = chat1_1,
                Value = "value2_msg1_2"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg2Chat1_1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg3Chat1_1 = new Message
            {
                User = user2,
                Chat = chat1_1,
                Value = "value3_msg1_1"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg3Chat1_1)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            // -----------------------------------------
            
            var chat2_2 = new Chat
            {
                Name = "server1_chat2_1",
                Type = ChatType.Text,
                Server = server2
            };
            
            if ((res = await _chatService.CreateChatAsync(chat2_2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }
            
            var msg1Chat2_2 = new Message
            {
                User = user,
                Chat = chat2_2,
                Value = "value1_msg1_chat2"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg1Chat2_2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg2Chat2_2 = new Message
            {
                User = user,
                Chat = chat2_2,
                Value = "value2_msg1_chat2"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg2Chat2_2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            var msg3Chat2_2 = new Message
            {
                User = user2,
                Chat = chat2_2,
                Value = "value3_msg1_chat2"
            };
            
            if ((res = await _messageService.CreateMessageAsync(msg3Chat2_2)).IsFailed)
            {
                Console.WriteLine(res.Errors.Select(e => e.Message));
            }

            return Ok(user);
        }
    }
}