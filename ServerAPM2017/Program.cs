using System.Net;
using ServerAPMLibrary;

namespace ServerAPM2017
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server(IPAddress.Loopback, 2048);
            s.Start();
        }
    }
}
