using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using TcpChat_Library.Models;
using TcpChat_Library;

namespace Tcp_Controler
{
    public class Client
    {
        Hero hero = new Hero();
        Socket socket_sender;
        public Client()
        {
            socket_sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endRemoutePoint = new IPEndPoint(address, 7632);

            socket_sender.Connect(endRemoutePoint);
        }

        public void MoveRight()
        {
            hero.X += 50;

            Utility.JsonSerialiseAndSend(hero, socket_sender);
        }
    }
}
