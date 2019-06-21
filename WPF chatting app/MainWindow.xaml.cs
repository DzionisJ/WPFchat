using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace WPF_chatting_app
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket sck;
        EndPoint epLocal, epRemote;

        string MyIP;
        string FriendIP;

        int port = 80;
        int friendPort =81;
        public MainWindow()
        {
            InitializeComponent();
            send.IsEnabled = false;

            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            MyIP = GetLocalIP();
            FriendIP = GetLocalIP();
                
        }

        private string GetLocalIP() 
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "localhost";
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);

                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];

                    receivedData = (byte[])aResult.AsyncState;

                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receiveMessage = eEncoding.GetString(receivedData);

                    The_chat.Items.Add("Friend : " + receiveMessage);
                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(MyIP), Convert.ToInt32(port));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(FriendIP), Convert.ToInt32(friendPort));
                sck.Connect(epRemote);

                byte[] buffer = new byte[1500];

                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                MessageBox.Show("Connected");
                UserMesageBox.Focus();
                Createchat.IsEnabled = false;
                send.IsEnabled = true;
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(UserMesageBox.Text);

                The_chat.Items.Add("You : " + UserMesageBox.Text);
                UserMesageBox.Clear();
                UserMesageBox.Focus();

            }
            catch (Exception s)
            {

                MessageBox.Show(s.ToString());
            }
        }

        private void The_chat_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
