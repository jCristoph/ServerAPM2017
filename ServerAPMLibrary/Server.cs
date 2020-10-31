using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerAPMLibrary
{
    public class Server : ServerEcho
    {
        private int data;
        private byte[] dataOut;
        public delegate void TransmissionDataDelegate(NetworkStream stream);

        public Server(IPAddress IP, int port) : base(IP, port)
        {
        }

        protected override void AcceptClient()
        {
            while (true)
            {
                TcpClient tcpClient = TcpListener.AcceptTcpClient();
                Stream = tcpClient.GetStream();
                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);
                //callback style
                //transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);
                // async result style
                IAsyncResult result = transmissionDelegate.BeginInvoke(Stream, null, null);
                //operacje......
                while (!result.IsCompleted);
                //sprzątanie
            }
        }
        private void TransmissionCallback(IAsyncResult ar)
        {
            ar.AsyncWaitHandle.Close();
        }
        protected override void BeginDataTransmission(NetworkStream stream)
        {
            byte[] buffer = new byte[Buffer_size];
            while (true)
            {
                try
                {
                    int message_size = stream.Read(buffer, 0, Buffer_size);
                    readBytes(buffer);
                    power();
                    convertToBytes();
                    stream.Write(dataOut, 0, dataOut.Length);
                }
                catch (IOException e)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// Metoda czytająca tablicę bajtów i przekształca ją na liczbę całkowitą
        /// </summary>
        /// <param name="array"></param>
        private void readBytes(byte[] array)
        {
            try
            {
                data = Int32.Parse(Encoding.UTF8.GetString(array));
            }
            catch(Exception e)
            {
                data = 0;
            }
            Console.WriteLine("Liczba: " + data);
        }
        /// <summary>
        /// Metoda obliczająca potęgę 
        /// </summary>
        private void power()
        {
            data = (int)Math.Pow(data, 2);
            Console.WriteLine("Wynik: " + data);
        }
        /// <summary>
        /// Metoda zamieniająca wynik typu int na tablicę bajtów
        /// </summary>
        private void convertToBytes()
        {
            if(data != 0)
                dataOut = Encoding.UTF8.GetBytes(data.ToString());
            else
                dataOut = Encoding.UTF8.GetBytes("");
        }
        public override void Start()
        {
            StartListening();
            //transmission starts within the accept function
            AcceptClient();
        }
    }
}