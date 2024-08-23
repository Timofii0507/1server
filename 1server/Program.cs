using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.CursorVisible = false;
        IPAddress ipAddress = IPAddress.Parse("192.168.0.1");
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
                Socket handler = await listener.AcceptAsync();
                _ = HandleClientAsync(handler);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static async Task HandleClientAsync(Socket handler)
    {
        string data = null;

        byte[] bytes = new byte[1024];
        int bytesRec = await handler.ReceiveAsync(new ArraySegment<byte>(bytes), SocketFlags.None);
        data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

        Console.WriteLine($"At {DateTime.Now.ToShortTimeString()} received from {((IPEndPoint)handler.RemoteEndPoint).Address}: {data}");

        string response = "Hello, client!";
        byte[] msg = Encoding.UTF8.GetBytes(response);
        await handler.SendAsync(new ArraySegment<byte>(msg), SocketFlags.None);

        handler.Shutdown(SocketShutdown.Both);
        handler.Close();
    }
}