using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


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
                SendMessage(socket, $"[CLIENT]: {message}");
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
