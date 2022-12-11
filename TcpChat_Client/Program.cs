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

            while (true)
            {
                //отправка сообщения от клиента
                Console.WriteLine("Введите сообщение для отправки на сервер");
                string message = Console.ReadLine();
                byte[] bytes = Encoding.Unicode.GetBytes(message);
                //отправляем послыку на сервер
                socket_sender.Send(bytes);
                Console.WriteLine($"Посылка \"{message}\" отправленна");

                //получаем ответ от сервера
                byte[] bytes_answer = new byte[1024];
                int num_bytes = socket_sender.Receive(bytes_answer);
                string answer = Encoding.Unicode.GetString(bytes_answer, 0, num_bytes);
                Console.WriteLine(answer);
                Console.WriteLine();
            }

            Console.ReadLine();
        }
    }
}
