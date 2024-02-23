using ChatClient.MVVM.Core;
using ChatClient.MVVM.Model;
using ChatClient.Net;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ChatClient.MVVM.ViewModel
{
    class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        
        public ObservableCollection<ContactModal> Contacts { get; set; }
        public string Username { get; set; }
        public string Message { get; set; }
        private Server _server;

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();
            _server = new Server();
            _server.ConnectedEvent += UserConnected;
            _server.msgReceivedEvent += MessageReceived;
            _server.diconnectedUserEvent += RemoveUser;

            Contacts = new ObservableCollection<ContactModal>();

            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => SendMessage(), o => !string.IsNullOrEmpty(Message));
        }

        private void SendMessage()
        {
            _server.SendMessageToServer(Message);
            // Optionally, you can add the sent message to the local collection for display
          //  Application.Current.Dispatcher.Invoke(() => Messages.Add($"You: {Message}"));
            Message = string.Empty; // Clear the message box after sending
        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.FirstOrDefault(x => x.UID == uid);
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var msg = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(msg));
        }

        private void UserConnected()
        {
            var user = new UserModel()
            {
                UserName = _server.PacketReader.ReadMessage(),
                UID = _server.PacketReader.ReadMessage(),

            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
                
            }

        }

       

    }
}

