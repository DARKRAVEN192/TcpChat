using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.IO;

using Newtonsoft.Json;
using TcpChat_Library;
using TcpChat_Library.Models;

namespace TcpChat_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Client";
            Console.WriteLine("[CLIENT]");

            Socket socket_sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse("127.0.0.1");
            IPEndPoint endRemoutePoint = new IPEndPoint(address, 7632);

            Console.WriteLine("Нажмите Enter для подключения");
            Console.ReadLine();

            //подключаемся к удаленной точке
            socket_sender.Connect(endRemoutePoint);

            // работа с именем клиента
            Console.Write("Пожалуйста, введите имя: ");
            string name = Console.ReadLine();
            Utility.SendMessage(socket_sender, name);

            Action<Socket> taskSendMessage = SendMessageForTask;
            IAsyncResult res = taskSendMessage.BeginInvoke(socket_sender, null, null);

            Action<Socket> taskReceiveMessage = ReceiveMessageForTask;
            IAsyncResult recReceive = taskReceiveMessage.BeginInvoke(socket_sender, null, null);

            taskSendMessage.EndInvoke(res);
            taskReceiveMessage.EndInvoke(recReceive);

            Console.ReadLine();
        }

        public static void SendMessageForTask(Socket socket)
        {
            while (true)
            {
                string message = Console.ReadLine();

                if (message == "platypus")
                {
                    Platypus platypus = new Platypus()
                    {
                        Color = "CoolBrown", Size =2         
                    };

                    Utility.XmlSerialiseAndSend(platypus, socket);
                }
                else if (message == "dumpling")
                {
                    Dumpling dumpling = new Dumpling()
                    {
                        IsFried = true, Name = "Стрелка", 
                        Description = "Супер-пупер афигенно смачный и странно зеленоватый"
                    };

                    Utility.JsonSerialiseAndSend(dumpling, socket);
                }
                else
                {
                    Utility.SendMessage(socket, message);
                }
                
            }
        }

        public static void ReceiveMessageForTask(Socket socket)
        {
            while (true)
            {
                string answer = Utility.ReceiveMessage(socket);
                Console.WriteLine(answer);
            }
        }

    }
}
