using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TcpChat_Server
{
    class Program
    {
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

            while (true)
            {
                // получение сообщения от клиента
                byte[] bytes = new byte[1024];
                int numBytes = socketClient.Receive(bytes);
                string textFromClient = Encoding.Unicode.GetString(bytes, 0, numBytes);
                Console.WriteLine(textFromClient);

                // ответное сообщение от сервера к клиенту
                string answer = "Server: OK";
                byte[] bytes_answer = Encoding.Unicode.GetBytes(answer);
                socketClient.Send(bytes_answer);
            }        
            Console.ReadLine();
        }
    }
}
