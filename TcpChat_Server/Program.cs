using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace TcpChat_Server
{
    class Program
    {
        static string messageFromUser="";
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

            socket.Listen(1);  // переволим сокет в режим слушать

            Console.WriteLine("Ожидаем звонка от клиента...");

            Socket socketClient = socket.Accept();   // ожидаем звонка

            Console.WriteLine("Клиент на связи");

            // создаем менеджеров
            Thread threadReceive = new Thread(ReceiveMessageForManager);
            Thread threadSend = new Thread(SendMessageForManager);

            threadSend.Start(socketClient);
            threadReceive.Start(socketClient);


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
            while (true)
            {
                Socket socket = (Socket)socketObj;

                messageFromUser = ReceiveMessage(socket);

                Console.WriteLine($"[MANAGER]: {messageFromUser}");
            }
        }

        public static void SendMessage(Socket socket, string message)
        {
            // ответное сообщение от сервера к клиенту
            byte[] bytes_answer = Encoding.Unicode.GetBytes(message);
            socket.Send(bytes_answer);
        }

        public static void SendMessageForManager(object socketObj)
        {
            Socket socket = (Socket)socketObj;
            while (true)
            {
                string messageFromServer = Console.ReadLine();
                SendMessage(socket, messageFromServer);
            }
        }
    }
}
