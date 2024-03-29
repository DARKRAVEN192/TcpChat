﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using TcpChat_Library;
using TcpChat_Library.Models;

using Newtonsoft.Json;


namespace Tcp_Receiver
{
    public class Server
    {
        private Socket socket;
        private bool isConnected = false;

        public bool IsConnected
        {
            get { return isConnected; }
            set { isConnected = value; }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public Server()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(address, 7632);

            socket.Bind(endPoint);

            socket.Listen(2);  // переволим сокет в режим слушать
        }

        public void Start()
        {
            Thread thread = new Thread(Run);
            thread.Start();
        }

        public void Run()
        {
            Socket socketClient = socket.Accept();

            IsConnected = true;

            while (true)
            {
                string text = Utility.ReceiveMessage(socketClient);

                Hero hero = JsonConvert.DeserializeObject<Hero>(text);
                X = hero.X;
                Y = hero.Y;
            }
        }
    }
}
