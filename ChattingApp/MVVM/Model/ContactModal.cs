using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.MVVM.Model
{
    class ContactModal
    {
        public string Username { get; set; }

        public string ImageSource { get; set; }

        public ObservableCollection<MessageModal> Messages { get; set; }

        public string LastMessage => Messages.Last().Message;

    }
}
