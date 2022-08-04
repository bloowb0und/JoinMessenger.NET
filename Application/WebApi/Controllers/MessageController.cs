using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public MessageController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }
        
        [HttpGet]
        public ActionResult<Message> GetMessageById(int messageId)
        {
            if (messageId < 1)
            {
                return BadRequest("Incoming data was null.");
            }

            var foundMessage = _messageService.GetMessageById(messageId);

            if (foundMessage.IsFailed || foundMessage.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(foundMessage.Errors));
            }
            
            return Ok(foundMessage.Value);
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesFromChat(int chatId)
        {
            if (chatId < 1)
            {
                return BadRequest("Incoming data was null.");
            }

            var foundMessages = await _messageService.GetAllMessagesFromChatAsync(chatId);
            
            if (foundMessages.IsFailed || foundMessages.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(foundMessages.Errors));
            }
            
            return Ok(foundMessages.Value);
        }
        
        [HttpPost]
        public async Task<ActionResult> CreateMessage([FromBody] int userId, int chatId, string value)
        {
            if (chatId < 1
                || userId < 1
                || string.IsNullOrWhiteSpace(value))
            {
                return BadRequest("Incoming data was null.");
            }

            var createdMessage = await _messageService.CreateMessageAsync(userId, chatId, value);

            if (createdMessage.IsFailed || createdMessage.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(createdMessage.Errors));
            }
            
            return Ok(createdMessage.Value);
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteMessage(int messageId)
        {
            if (messageId < 1)
            {
                return BadRequest("Incoming data was null.");
            }
            
            var userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var curUserById = _userService.GetUserById(Convert.ToInt32(userId));

            if (curUserById.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(curUserById.Errors));
            }

            var res = await _messageService.DeleteMessageAsync(curUserById.Value, messageId);

            if (res.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(res.Errors));
            }
            
            return Ok();
        }
    }
}