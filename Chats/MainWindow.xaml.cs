using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace Chats
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		TcpClient client;

		public MainWindow()
		{
			InitializeComponent();
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				IPAddress ipAddress = IPAddress.Parse(IpAddressInput.Text); // Textbox
				client = new TcpClient();
				client.Connect(ipAddress, 5000); // klappt nur, wenn der Server gestartet ist
				SendButton.IsEnabled = true;
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
			string messageText = MessageInput.Text;
			MessageInput.Text = string.Empty;

			NetworkStream stream = client.GetStream();

			byte[] messageTextBytes = Encoding.ASCII.GetBytes(messageText);
			stream.Write(messageTextBytes, 0, messageTextBytes.Length); // (Byte Buffer, Offset, Bufferlänge)
		}
	}
}
