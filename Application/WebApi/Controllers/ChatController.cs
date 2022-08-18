using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models;
using Core.Models.API;
using Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IServerService _serverService;

        public ChatController(IChatService chatService, IServerService serverService)
        {
            _chatService = chatService;
            _serverService = serverService;
        }

        [HttpGet]
        public async Task<ActionResult<ChatDto>> GetChats(int serverId)
        {
            var foundChats = await _chatService.GetChatsByServerAsync(serverId);

            if (foundChats.IsFailed || foundChats.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(foundChats.Errors));
            }
            
            return Ok(foundChats.Value);
        }

        [HttpPost]
        public async Task<ActionResult<Chat>> CreateChat([FromBody] CreateChatModel chatModel)
        {
            if (chatModel == null
                || string.IsNullOrWhiteSpace(chatModel.Name)
                || chatModel.ServerId < 1)
            {
                return BadRequest("Incoming data was null.");
            }

            var foundServer = _serverService.GetServerById(chatModel.ServerId);

            if (foundServer.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(foundServer.Errors));
            }

            var createdChat = await _chatService.CreateChatAsync(chatModel.Name, chatModel.Type, foundServer.Value);

            if (createdChat.IsFailed || createdChat.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(createdChat.Errors));
            }
            
            return Ok(createdChat.Value);
        }

        [HttpDelete]
        public ActionResult DeleteChat(int chatId)
        {
            if (chatId < 0)
            {
                return BadRequest("Incoming data was null.");
            }

            var foundChat = _chatService.GetChatById(chatId);
            
            if (foundChat.IsFailed || foundChat.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(foundChat.Errors));
            }

            return Ok(foundChat.Value);
        }
    }
}