using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChatClientWinForms
{
    public partial class Form1 : Form
    {
        HubConnection connection;
        public Form1()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/chat")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (message, userName) =>
            {
                this.listBoxChat.Invoke((MethodInvoker)delegate
                {
                    var newMessage = $"{userName}: {message}";
                    listBoxChat.Items.Add(newMessage);
                });
            });

            connection.On<string>("Disconnect", (userID) =>
            {
                this.Invoke((MethodInvoker)delegate { Application.Exit(); });
            });

            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("AddToGuestGroup");
                listBoxChat.Items.Add("Connection started");
                btnConnect.Enabled = false;
                btnSendMessage.Enabled = true;
            }
            catch (Exception ex)
            {
                listBoxChat.Items.Add(ex.Message);
            }
        }

        private async void btnSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", txtBoxMessage.Text, txtBoxUser.Text);
            }
            catch (Exception ex)
            {
                listBoxChat.Items.Add(ex.Message);
            }
        }
    }
}