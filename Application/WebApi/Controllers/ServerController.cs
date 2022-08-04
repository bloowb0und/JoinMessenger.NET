using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BLL.Abstractions.Interfaces;
using Core.Models.API;
using Core.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ServerController : ControllerBase
    {
        private readonly IServerService _serverService;
        private readonly IUserService _userService;

        public ServerController(IServerService serverService, 
            IUserService userService)
        {
            _serverService = serverService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerDto>>> GetServers()
        {
            var userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var curUserById = _userService.GetUserById(Convert.ToInt32(userId));

            if (curUserById.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(curUserById.Errors));
            }

            var serversForUser = await _serverService.GetServersForUser(curUserById.Value);

            if (serversForUser.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(serversForUser.Errors));
            }

            return Ok(serversForUser.Value);
        }

        [HttpPost]
        public async Task<ActionResult<ServerDto>> CreateServer([FromBody] CreateServerModel serverModel)
        {
            if (serverModel == null
                || string.IsNullOrWhiteSpace(serverModel.ServerName))
            {
                return BadRequest("Incoming data was null.");
            }

            var userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var curUserById = _userService.GetUserById(Convert.ToInt32(userId));

            if (curUserById.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(curUserById.Errors));
            }

            var createdServer = await _serverService.CreateServerAsync(serverModel.ServerName, curUserById.Value);

            if (createdServer.IsFailed || createdServer.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(createdServer.Errors));
            }

            return Ok(createdServer.Value);
        }
        
        [HttpDelete]
        public async Task<ActionResult> DeleteServer(int serverId)
        {
            if (serverId < 0)
            {
                return BadRequest("Incoming data was null.");
            }

            var foundServer = _serverService.GetServerById(serverId);
            if (foundServer.ValueOrDefault == null)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(foundServer.Errors));
            }
            
            var userId = User.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var curUserById = _userService.GetUserById(Convert.ToInt32(userId));

            if (curUserById.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(curUserById.Errors));
            }

            var res = await _serverService.DeleteServerAsync(foundServer.Value, curUserById.Value);

            if (res.IsFailed)
            {
                return BadRequest(ErrorStringHelper.AppendErrors(res.Errors));
            }
            
            return Ok();
        }
    }
}