using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static readonly List<TcpClient> clients = new List<TcpClient>(); // генерира се списък от клиенти
    private const int Port = 8888; // задава се порт

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port); // генерира се сървър
        server.Start(); // сървъра се стартира
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client); // клиента се добавя в списъка
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj; // създава се обект
        NetworkStream stream = tcpClient.GetStream();// клиента става активен

        byte[] buffer = new byte[1024]; // създава се масив 
        int bytesRead; //инициализира се променлива

        while (true) 
        {
            try // стартира се проверка
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length); 
                if (bytesRead == 0)
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                BroadcastMessage(tcpClient, message);
            }
            catch (Exception) // хвърля се изключение при неуспешно извършена проверка
            {
                break;
            }
        }

        clients.Remove(tcpClient); //клиента се премахва от списъка
        tcpClient.Close();
    }

    static void BroadcastMessage(TcpClient sender, string message)
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);

        foreach (TcpClient client in clients) //за всеки клиент от списъка се извършва проверката
        {
            if (client != sender) //проверява дали клиента изпраща или не
            {
                NetworkStream stream = client.GetStream(); 
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
    }
}
