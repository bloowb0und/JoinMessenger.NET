using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public interface IServerService
    {
        public bool CreateServer(string name);

        public bool DeleteServer(Server server);

        public bool AddChat(Server server, Chat chat);

        public bool DeleteChat(Server server, Chat chat);

        public bool AddUser(Server server, User user);

        public bool DeleteUser(Server server, User user);

    }
}
