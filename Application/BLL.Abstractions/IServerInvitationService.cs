using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Abstractions
{
    public interface IServerInvitationService
    {
        public Task InviteByEmailAsync(Server server, User user);
    }
}
