using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Chats
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				IPAddress ipAddress = IPAddress.Parse(IpAddressInput.Text); // Textbox
				TcpClient client = new TcpClient();
				client.Connect(ipAddress, 5000);
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
	}
}
