using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Chats // Clients
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		TcpClient client;
		string userName = string.Empty;
		// static List<TcpClient> clients = new List<TcpClient>(); // Liste der TcpClients

		public MainWindow()
		{
			InitializeComponent();
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				userName = NameInput.Text;

				IPAddress ipAddress = IPAddress.Parse(IpAddressInput.Text); // Textbox
				client = new TcpClient();
				client.Connect(ipAddress, 5000); // klappt nur, wenn der Server gestartet ist

				SendButton.IsEnabled = true;

				ChatText.Text = string.Empty; // Damit "Bitte verbinden" beim Klicken d. Connect-Buttons gelöscht wird

				Thread thread = new Thread(() => ReceiveData(client));
				thread.Start();
			}
			catch (FormatException)
			{
				MessageBox.Show("Ungültige IP-Adresse");
			}
			catch (SocketException)
			{
				MessageBox.Show("Server nicht erreichbar");
			}
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			string messageText = string.Format("message|{0}|{1}", userName, MessageInput.Text);
			MessageInput.Text = string.Empty;

			NetworkStream stream = client.GetStream();

			byte[] messageTextBytes = Encoding.UTF8.GetBytes(messageText);
			stream.Write(messageTextBytes, 0, messageTextBytes.Length); // (Byte Buffer, Offset, Bufferlänge)
		}

		void ReceiveData(TcpClient client)
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
					string data = Encoding.UTF8.GetString(buffer, 0, bufferLength); // Rückkonvertierung in einen String,
																					// um Daten ausgeben zu können
					data = data.Replace("\0", string.Empty);
					string[] messageParts = data.Split('|'); // Data wird aufgeteilt in verschiedene Teile (Parts)

					switch (messageParts[0])
					{
						case "message":
							Dispatcher.Invoke(() =>
							{
								ChatText.Text = messageParts[1] + ": " + messageParts[2] + Environment.NewLine + ChatText.Text;
												// neuer Text wird oben angehängt
												// ChatText.Text += Environment.NewLine + data; // neuer Text wird unten angehängt
							});
							break;
						case "onlinezaehler":
							Dispatcher.Invoke(() =>
							{
								OnlineOutput.Content = messageParts[1];
							});
							break;
						default:
							break;
					}
				}
			}
		}
	}
}
