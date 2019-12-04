using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatServer
{
	class Program
	{
		static List<TcpClient> clients = new List<TcpClient>(); // Liste der TcpClients

		static void Main(string[] args)
		{
			// TcpListener tcpListener = new TcpListener(5000); // funktioniert, aber obsolet; jetzt mit IP-Address-Bereich
			TcpListener tcpListener = new TcpListener(IPAddress.Any, 5000);
			tcpListener.Start();

			while (true)
			{
				TcpClient client = tcpListener.AcceptTcpClient();
				clients.Add(client);

				Thread thread = new Thread(() => HandleClient(client)); // jeder Client bekommt einen eigenen Thread
				thread.Start();

				Console.WriteLine($"Es sind {clients.Count} Teilnehmer online.");
			}
			
			// Console.ReadKey();
		}

		static void HandleClient(TcpClient client)
		{
			while (true)
			{
				NetworkStream stream = client.GetStream();

				byte[] buffer = new byte[1024];
				int bufferLength = stream.Read(buffer, 0, buffer.Length);

				if (bufferLength > 0)
				{
					string data = Encoding.ASCII.GetString(buffer, 0, bufferLength);
					Console.WriteLine(data);
				}
			}
		}
	}
}
