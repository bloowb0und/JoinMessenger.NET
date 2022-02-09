using Core.Models;
using GalaSoft.MvvmLight.Command;
using NextGenWPF.Services;
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
        public List<Server> Servers { get; set; }
        private Server _server;
        private Chat _chat;
        private User _user;
        private Dictionary<string, string> _chatsSavedMessage = new Dictionary<string, string>();
        public Server OnServerChanged 
        { 
            get { return _server; }
            set
            {
                this.SetServer(value);
            } 
        }
        public Chat OnChatChanged
        {
            get { return _chat; }
            set
            {
                this.SetChat(value);
            }
        }
        private ICurrentDeterminatorService _currentDeterminatorService;

        public MainPageViewModel()
        {
                
        }
        public MainPageViewModel(ICurrentDeterminatorService currentDeterminatorService)
        {
            _currentDeterminatorService = currentDeterminatorService;
            _currentDeterminatorService.userSubject.Subscribe((user) =>
            {
                if (user!=null)
                {
                    _user = user;
                    this.OnPropertyChanged(nameof(_user));
                }
            });
            /*Servers.Add(new Server()
            {
                Name = "Kekises",
                DateCreated = DateTime.Now,
                Chats = new List<Chat>() {
                new Chat()
                {
                    Name = "#  speaking",
                    Messages = new List<Message>()
                    {
                        new Message()
                        {
                            Value ="Test",
                            User = new User
                            {
                                Name = null,
                                Email = null,
                                Login = "Kerich",
                                Password = null,
                            }
                        }
                    }
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
            });;
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
#endif*/
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
        protected override void OnPageLoaded()
        {
            Servers = _user.UserServers.Where(u=>u.User.Id == _user.Id).Select(u=>u.Server).ToList();
        }
        private void SetServer(Server server)
        {
            this.OnServerChanged = server;
        }
        private void SetChat(Chat chat)
        {
            this._chat = chat;
            if (_chatsSavedMessage.ContainsKey(chat.Name))
            {
                this.Message = _chatsSavedMessage.GetValueOrDefault(chat.Name);
            }
            else
            {
                _chatsSavedMessage.Add(chat.Name, String.Empty);
                this.Message = String.Empty;
            }

        }
    }
}
