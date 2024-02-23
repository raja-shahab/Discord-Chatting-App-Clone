using ChatServer.Net.IO;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Program
    {
        static List<Client> users;
        static TcpListener _listener;
        static void Main(string[] args)
        {
            users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            _listener.Start();


            while (true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                users.Add(client);
                BroadcastConnection();
            }
            
        }

        static void BroadcastConnection()
        {
            foreach (var user in users)
            {
                foreach (var user1 in users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(user1.Username);
                    broadcastPacket.WriteMessage(user1.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());


                }

            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach (var user in users)
            {
                var messagePacket = new PacketBuilder();
                messagePacket.WriteOpCode(5);
                messagePacket.WriteMessage(message);
                user.ClientSocket.Client.Send(messagePacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnected(string uid)
        {
            var disconnectedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            users.Remove(disconnectedUser);
            foreach (var user in users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());

            }

            BroadcastMessage($"[{disconnectedUser.UID}: Disconnected! ]");

        }

        

    }
}