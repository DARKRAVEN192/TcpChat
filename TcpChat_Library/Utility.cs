using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using TcpChat_Library.Models;
using System.IO;
using Newtonsoft.Json;

namespace TcpChat_Library
{
    public static class Utility
    {
        public static void SendMessage(Socket socket, string message)
        {
            // ответное сообщение от сервера к клиенту
            byte[] bytes_answer = Encoding.Unicode.GetBytes(message);
            socket.Send(bytes_answer);
        }

        public static string ReceiveMessage(Socket socket)
        {
            // получение сообщения
            byte[] bytes = new byte[1024];
            int numBytes = socket.Receive(bytes);
            return Encoding.Unicode.GetString(bytes, 0, numBytes);
        }

        public static void XmlSerialiseAndSend(object obj, Socket socket)
        {
            XmlSerializer xmlSerialiser = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            xmlSerialiser.Serialize(stream, obj);
            stream.Position = 0;
            byte[] bytes = stream.ToArray();

            socket.Send(bytes);
        }

        public static string JsonSerialise(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static void JsonSerialiseAndSend(object obj, Socket socket)
        {
            string text = JsonSerialise(obj);
            SendMessage(socket, text);
        }

        public static void JsonDeserialise(string text)
        {
            Dumpling dumpling = JsonConvert.DeserializeObject<Dumpling>(text);
        }
    }
}
