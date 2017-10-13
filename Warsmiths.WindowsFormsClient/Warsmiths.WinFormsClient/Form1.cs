using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Warsmiths.Client;
using Warsmiths.Client.Loadbalancing;
using Warsmiths.Client.Loadbalancing.Enums;
using Warsmiths.Common;
using Warsmiths.Common.Utils;

namespace Warsmiths.WinFormsClient
{
    public partial class Form1 : Form
    {
        public static GameClient Client { get; set; }
        public static Form1 Instance { get; set; }
        public static OnlineForm OnlineFormInstance { get; set; }

        public static void NetworkLog(Action logAction)
        {
            logAction();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string address;
            string appVersion = "1.0";
            string nickname = "Tim";

            button1.Text = @"Connection...";
            Application.DoEvents();

            OnlineFormInstance = new OnlineForm
            {
                toolStripStatusLabel1 = {Text = button1.Text}
            };

            Client = new GameClient
            {
                //OnStateChangeAction = s => NetworkLog(() => { OnlineFormInstance.toolStripStatusLabel1.Text = string.Format("state:" + s); })
            };
            var authValues = new AuthenticationValues
            {
                UserId = "write_here_user_id",
                AuthType = CustomAuthenticationType.Custom
            };
            authValues.AddAuthParameter("login", "login_text");
            authValues.AddAuthParameter("password", "password_text");
            authValues.AddAuthParameter("password", "password_text");

            if (comboBox1.SelectedItem == "Local")
            {
                address = NetworkUtils.GetServerConnectionHost(ServerHosts.Local);
            }
            else
            {
                address = NetworkUtils.GetServerConnectionHost(ServerHosts.NewDevelop);
            }

            if (Client.IsConnectedAndReady == false)
            {
                while (true)
                {
                    Client.Service();

                    if (Client.IsConnectedAndReady)
                    {
                        Task.Factory.StartNew(() =>
                        {
                            Instance = this;
                            while (Client.IsConnectedAndReady)
                            {
                                Thread.Sleep(1);
                                Client.Service();
                            }
                        });

                        OnlineFormInstance.SubscribeEvents();
                        Thread.Sleep(1000);
                        OnlineFormInstance.ShowDialog();

                        Hide();
                        break;
                    }

                    Thread.Sleep(100);
                    Client.Connect(address, Client.AppId, appVersion, nickname, authValues);
                }
            }
            else
            {
                MessageBox.Show(@"Already connected");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Client.IsConnected)
            {
                Client.Disconnect();
            }
        }
    }
}