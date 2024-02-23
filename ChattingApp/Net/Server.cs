using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    class Server
    {
        TcpClient tcpClient;
        public PacketReader PacketReader;


        public event Action ConnectedEvent;
        public event Action msgReceivedEvent;
        public event Action diconnectedUserEvent;



        public Server()
        {
            tcpClient = new TcpClient();
            tcpClient = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!tcpClient.Connected)
            {
                tcpClient.Connect("127.0.0.1", 7891);
                PacketReader = new PacketReader(tcpClient.GetStream());
                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    tcpClient.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();

            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {

                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            ConnectedEvent?.Invoke();
                            break;

                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;

                        case 10:
                            diconnectedUserEvent?.Invoke();
                            break;


                        default:
                            Console.WriteLine();
                            break;
                    }


                }

            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();

            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            tcpClient.Client.Send(messagePacket.GetPacketBytes());
        }


    }






}
