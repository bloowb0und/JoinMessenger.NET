using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions.Interfaces
{
    public interface IServerInvitationService
    {
        public Task InviteByEmailAsync(Server server, User user);
    }
}
