using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.CursorVisible = false;
        IPAddress ipAddress = IPAddress.Parse("192.168.0.103");
        int port = 11000;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);

        Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Console.WriteLine("Waiting for a connection...");

            while (true)
            {
                Socket handler = listener.Accept();
                string data = null;

                byte[] bytes = new byte[1024];
                int bytesRec = handler.Receive(bytes);
                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                string response;
                if (data.ToLower().Contains("time"))
                {
                    response = DateTime.Now.ToString("HH:mm:ss");
                }
                else if (data.ToLower().Contains("date"))
                {
                    response = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    response = "Invalid request";
                }

                byte[] msg = Encoding.UTF8.GetBytes(response);
                handler.Send(msg);

                Console.WriteLine($"At {DateTime.Now.ToShortTimeString()} received from {((IPEndPoint)handler.RemoteEndPoint).Address}: {data}");
                Console.WriteLine($"Sent response: {response}");

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}