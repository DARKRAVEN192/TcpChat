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
            SendMessage(socket_sender, name);

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

                    XmlSerializer xmlSerialiser = new XmlSerializer(typeof(Platypus));

                    MemoryStream stream = new MemoryStream();

                    xmlSerialiser.Serialize(stream, platypus);

                    stream.Position = 0;
                    //Platypus platypus2 = xmlSerialiser.Deserialize(stream) as Platypus;

                    byte[] bytes = stream.ToArray();
                    // отправляем утконоса
                    socket.Send(bytes);
                }
                else if (message == "dumpling")
                {
                    Dumpling dumpling = new Dumpling()
                    {
                        IsFried = true, Name = "Стрелка", 
                        Description = "Супер-пупер афигенно смачный и странно зеленоватый"
                    };

                    string text = JsonConvert.SerializeObject(dumpling);
                    SendMessage(socket, text);
                }
                else
                {
                    SendMessage(socket, message);
                }
                
            }
        }

        public static void ReceiveMessageForTask(Socket socket)
        {
            while (true)
            {
                string answer = ReceiveMessage(socket);
                Console.WriteLine(answer);
            }
        }

        public static void SendMessage(Socket socket, string message)
        {
            byte[] bytes_answer = Encoding.Unicode.GetBytes(message);
            socket.Send(bytes_answer);
        }

        public static string ReceiveMessage(Socket socket)
        {
            byte[] bytes = new byte[1024];
            int numBytes = socket.Receive(bytes);
            return Encoding.Unicode.GetString(bytes, 0, numBytes);
        }
    }
}
