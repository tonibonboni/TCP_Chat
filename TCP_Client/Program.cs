using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888; //задава порта
    private const string ServerIp = "127.0.0.1"; // задава IP адреса

    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port); //създава обект,който да играе роля на клиент и задава параметрите,които трябва да съдържа
        Console.WriteLine("Connected to server. Start chatting!");

        NetworkStream stream = client.GetStream(); // инициализира обект,който изобразява дрйствията на клиента

        Thread receiveThread = new Thread(ReceiveMessages); //създава обект
        receiveThread.Start(stream); //обекта започва да изпълмява действие

        while (true)
        {
            string message = Console.ReadLine(); // програмата чете криптираното съобщение
            byte[] buffer = Encoding.ASCII.GetBytes(message); //съобщението се декриптира в буферната памет
            stream.Write(buffer, 0, buffer.Length); //програмата извежда декриптираното съобщение
        } 
    }

    static void ReceiveMessages(object obj)
    {
        NetworkStream stream = (NetworkStream)obj;
        byte[] buffer = new byte[1024]; //създава се масив 
        int bytesRead; //инициализира променлива

        while (true)
        {
            try // извършва проверка
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
            catch (Exception) // хвърля изключение при неуспешно премината променлива на входните данни
            {
                break; //цикъла се прекъсва
            }
        }
    }
}
