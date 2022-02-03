using Core.Models;
using NextGenWPF.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace NextGenWPF.ViewModels
{
    public class MainPageViewModel : BasePageViewModel
    {
        public ObservableCollection<Server> Servers { get; set; }
        public List<Chat> CurrentServerChats { get; set; }
        public List<Message> CurrentChatMessages { get; set; }
        private Server CurrentServer { get; set; }
        public ObservableCollection<Message> Messages { get; set; }
        public MainPageViewModel()
        {
            Servers = new ObservableCollection<Server>();
#if DEBUG
            Servers.Add(new Server()
            {
                Name = "Kekises",
                DateCreated = DateTime.Now,
                Chats = new List<Chat>() {
                new Chat()
                {
                    Name = "#  speaking",
                },
                new Chat()
                {
                    Name = "#  workayem parni"
                },
                new Chat()
                {
                    Name = "#  chill parni"
                },
                new Chat()
                {
                    Name = "#  dimich kill us parni"
                }
                },
                Icon = @"D:\Project\JoinMes\Application\NextGenWPF\Images\image (1).png",
            });
            Servers.Add(new Server()
            {
                Name = "Team",
                DateCreated = DateTime.Now,
                Chats = new List<Chat>() { new Chat()
                {
                    Name = "BestTeam",
                } },
                Icon = @"D:\Project\JoinMes\Application\NextGenWPF\Images\image (1).png",
            });
            Servers.Add(new Server()
            {
                Name = "Doka foo",
                DateCreated = DateTime.Now,
                Chats = new List<Chat>() { new Chat()
                {
                    Name = "BestTeam",
                } },
                Icon = @"D:\Project\JoinMes\Application\NextGenWPF\Images\image (1).png",
            });
            CurrentServer = Servers.FirstOrDefault();
            CurrentServerChats = CurrentServer.Chats;
#endif
        }
        private string message;
        public string Message
        {
            get => this.message;
            set
            {
                this.message = value;
                this.OnPropertyChanged();
            }
        }
        private void SetServer()
        {
            CurrentServer = Servers.FirstOrDefault();
            CurrentServerChats = CurrentServer.Chats;
        }
    }
}
