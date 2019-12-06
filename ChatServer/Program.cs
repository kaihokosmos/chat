/*
 1. Server läuft/wartet auf Verbindungen
 2. Client1 verbindet s. mit Server (Connect)
 3. Client1 ist mit Server verbunden (Endlosschleife); Server staret Thread, um auf Nachrichten zu hören.
 4. Client2 will s. mit Server verbinden, währemd #3 läuft.
 5. Client1 und Client2 sind mit Server verbunden; bisher nur Abfrage Nachricht von Client1 => Server fragt keine Nachrichten
	von Client2 ab, wenn man ohne Threads arbeitet; d.h. ein Thread pro Client notwendig.
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatServer
{
	class Program
	{
		static List<TcpClient> clients = new List<TcpClient>(); // Liste der TcpClients
		static bool parallelExecution = false;
		static Timer sendClientUpdateTimer;

		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8; // damit die Console nicht nur

			Console.WriteLine("Soll der Server parallel verarbeiten? (j/n)");
			// string inputString = Console.ReadLine().ToLower();
			// char input = inputString[0];
			
			var input1 = Console.ReadKey();

			// parallelExecution = (input == 'j');

			if (input1.Key == ConsoleKey.J)
			// if (input == 'j')
			{
				Console.WriteLine("Parallele Verarbeitung aktiviert.");
				parallelExecution = true;
			}
			else
			{
				Console.WriteLine("Parallele Verarbeitung deaktiviert.");
			}

			/* Funktioniert noch nicht:
			var input1 = Console.ReadKey();
			if (input1.Key == ConsoleKey.Y)
			{
				parallelExecution = true;
			}
			*/
			
			// TcpListener tcpListener = new TcpListener(5000); // funktioniert, aber obsolet; jetzt mit IP-Address-Bereich
			TcpListener tcpListener = new TcpListener(IPAddress.Any, 5000); // Server initialisieren; höre auf Port 5000
													// IPAAddress.Any: IPAddress.Parse("127.0.0.1")
			tcpListener.Start();

			sendClientUpdateTimer = new Timer(SendClientUpdate, null, 0, 5000); 
				// 1. Methode, die übergeben wird; 2. State; 3. Due-Time (wie lange mit dem Starten d. Timers gewartet wird)
				// 4. Period: wie oft der Timer ausgelöst wird

			while (true) // Endlosschleife, damit der Server nach der ersten Verbindung nicht beendet wird
			{
				TcpClient client = tcpListener.AcceptTcpClient(); // wird ausgeführt, wenn Client .Connect() ausführt
				clients.Add(client); // den Client in einer Liste speichern

				Thread thread = new Thread(() => HandleClient(client)); // jeder Client bekommt einen eigenen Thread
												// () => ...: Schreiweise, um Thread e. Parameter zu übergeben
				thread.Start(); // Thread startet, um auf Nachrichten des Clients zu hören

				Console.WriteLine($"Es sind {clients.Count} Teilnehmer online.");
			}
			
			// Console.ReadKey();
		}

		private static void SendClientUpdate(object state)
		{
			string message = string.Format("onlinezaehler|{0}", clients.Count);
			byte[] byteMessage = Encoding.UTF8.GetBytes(message);
			Broadcast(byteMessage);
		}

		static void HandleClient(TcpClient client)
		{
			while (true) // Endlosschleife, um permanent eingehende Nachrichten zu überprüfen
						 // Ohne Schleife: Einmal Nachricht abfragen, dann Ende
						 // Ohne Thread: Nachrichtenabfrage blockiert das Programm
			{
				NetworkStream stream = client.GetStream();

				byte[] buffer = new byte[1024]; // mit fixer Buffergröße!
				int bufferLength = stream.Read(buffer, 0, buffer.Length);

				if (bufferLength > 0) // wenn Buffer (Datenpaket) nicht leer
				{
					Broadcast(buffer);

					string data = Encoding.UTF8.GetString(buffer, 0, bufferLength); // Rückkonvertierung in einen String,
																					// um Daten ausgeben zu können
					Console.WriteLine(data);
				}
			}
		}

		private static void Broadcast(byte[] buffer)
		{
			// Broadcast Anfang
			if (parallelExecution)
			{
				// Parallel
				Parallel.ForEach(clients, (otherClient) => // auch o. runde Klammern: otherClient => 
				{
					NetworkStream otherStream = otherClient.GetStream();
					otherStream.Write(buffer, 0, buffer.Length);
				});
			}
			else
			{
				// Nicht-parallel
				foreach (var otherClient in clients)
				{
					NetworkStream otherStream = otherClient.GetStream();
					otherStream.Write(buffer, 0, buffer.Length);
				}
			}
			// Broadcast Ende
		}
	}
}
