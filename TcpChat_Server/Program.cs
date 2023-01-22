using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

using Newtonsoft.Json;

namespace TcpChat_Server
{
    class Program
    {
        static string messageFromUser="";
        static List<User> clientSockets = new List<User>();
        static void Main(string[] args)
        {
            Console.Title = "Server";
            Console.WriteLine("[SERVER]");

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress address = IPAddress.Parse("127.0.0.1");

            //создаем endpoint = 127.0.0.1:7632
            IPEndPoint endPoint = new IPEndPoint(address, 7632);

            // привязываем сокет к endpoint
            socket.Bind(endPoint);

            socket.Listen(2);  // переволим сокет в режим слушать

            Console.WriteLine("Ожидаем звонка от клиента...");

            while (true)
            {
                Socket socketClient = socket.Accept();   // ожидаем звонка
                User user = new User()
                {
                    Socket = socketClient,
                    Name = "test"
                };
                clientSockets.Add(user);

                Console.WriteLine("Клиент на связи");

                // создаем менеджеров
                Thread threadReceive = new Thread(ReceiveMessageForManager);
                Thread threadSend = new Thread(SendMessageForManager);

                threadSend.Start(user);
                threadReceive.Start(user);
            }


            Console.ReadLine();
        }

        public static string ReceiveMessage(Socket socket)
        {
            // получение сообщения
            byte[] bytes = new byte[1024];
            int numBytes = socket.Receive(bytes);
            return Encoding.Unicode.GetString(bytes, 0, numBytes);        
        }

        public static void ReceiveMessageForManager(object socketObj)
        {
            User user = (User)socketObj;
            //получаем доп данные
            string name = ReceiveMessage(user.Socket);
            user.Name = name;
            //получаем сообщения от клиента
            while (true)
            {
                messageFromUser = ReceiveMessage(user.Socket);

                Console.WriteLine($"[{name}]: {messageFromUser}");

                // раскодировать сообщение от пользователя
                //ProcessCommandWord(user.Socket, messageFromUser);
                //ProcessCommandCoding(user.Socket, messageFromUser);

                ProcessCommandJson(user.Socket, messageFromUser);

                #region Receive XML
                /*
                byte[] bytes = new byte[1024];
                int numBytes = user.Socket.Receive(bytes);

                ProcessCommandXML(user.Socket, bytes, numBytes);
                */
                #endregion
            }
        }
        private static void ProcessCommandJson(Socket socket, string text)
        {
            Dumpling dumpling = JsonConvert.DeserializeObject<Dumpling>(text);
        }
        private static void ProcessCommandXML(Socket socket, byte[] bytes, int numBytes)
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(typeof(Platypus));

            MemoryStream stream = new MemoryStream(bytes, 0, numBytes);
            stream.Position = 0;

            Platypus platypus = xmlSerialiser.Deserialize(stream) as Platypus;
        }
        private static void ProcessCommandWord(Socket socket, string comand)
        {
            if (messageFromUser == "color")
            {
                Console.WriteLine($"Пользователь прислал команду color");

                SendMessage(socket, "Сервер принял вашу команду!");
            }
        }
        private static void ProcessCommandCoding(Socket socket, string comand)
        {
            // health, level, money
            // 10,4,5

            int health, level, money;

            string[] numsText = comand.Split(',');
            health = int.Parse(numsText[0]);
            level = int.Parse(numsText[1]);
            money = int.Parse(numsText[2]);

            Console.WriteLine($"Health: {health}, Level: {level}, Money: {money}");
        }

        public static void SendMessage(Socket socket, string message)
        {
            // ответное сообщение от сервера к клиенту
            byte[] bytes_answer = Encoding.Unicode.GetBytes(message);
            socket.Send(bytes_answer);
        }

        public static void SendMessageToAllUsers(string message)
        {
            foreach (var user in clientSockets)
            {
                SendMessage(user.Socket, message);
            }
        }
        public static void SendMessageForManager(object socketObj)
        {
            User user = (User)socketObj;
            while (true)
            {
                string messageFromServer = Console.ReadLine();
                SendMessageToAllUsers(user.Name +", "+ messageFromServer);
            }
        }
    }

    public class User
    {
        public string Name { get; set; }
        public Socket Socket { get; set; }
    }
}
