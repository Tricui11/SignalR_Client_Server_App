using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChatClientWinForms
{
    public partial class Form1 : Form
    {
        HubConnection connection;
        bool loggedIn;
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                connection = new HubConnectionBuilder().WithUrl("http://localhost:53353/chat").Build();
                connection.Closed += async (error) =>
                {
                    await Task.Delay(new Random().Next(0, 5) * 1000);
                    await connection.StartAsync();
                };

                connection.On<string, string>("ReceiveMessage", (message, userName) =>
                {
                    listBoxChat.Invoke((MethodInvoker)delegate
                    {
                        var newMessage = $"{userName}: {message}";
                        listBoxChat.Items.Add(newMessage);
                    });
                });

                connection.On("Disconnect", () =>
                {
                    Invoke((MethodInvoker)delegate { Application.Exit(); });
                });

                connection.On("NickNameBusy", () =>
                {
                    Invoke((MethodInvoker)async delegate {
                        MessageBox.Show("Данный ник занят.");
                        loggedIn = false;
                        await connection.DisposeAsync();
                        txtBoxUser.Enabled = true;
                        txtBoxUser.Text = string.Empty;
                        btnSendMessage.Enabled = false;
                        listBoxChat.Items.Add("Connection closed");
                    });
                });

                await connection.StartAsync();
                await connection.InvokeAsync("AddToGuestGroup", txtBoxUser.Text);
                listBoxChat.Items.Add("Connection started");
                loggedIn = true;
                txtBoxUser.Enabled = false;
                btnConnect.Enabled = false;
                btnSendMessage.Enabled = !string.IsNullOrWhiteSpace(txtBoxMessage.Text);
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
                txtBoxMessage.Text = string.Empty;
            }
            catch (Exception ex)
            {
                listBoxChat.Items.Add(ex.Message);
            }
        }

        private void listBoxChat_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            var color = Color.Black;
            string itemText = listBoxChat.Items[e.Index].ToString();
            if (itemText.EndsWith(" присоединился к чату."))
            {
                color = Color.Green;
            }
            else if (itemText.EndsWith(" покинул чат."))
            {
                color = Color.Red;
            }
            e.Graphics.DrawString(itemText, new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold), new SolidBrush(color), e.Bounds);
        }

        private void txtBoxUser_TextChanged(object sender, EventArgs e)
        {
            btnConnect.Enabled = !loggedIn && !string.IsNullOrEmpty(txtBoxUser.Text);
        }
        
        private void txtBoxMessage_TextChanged(object sender, EventArgs e)
        {
            btnSendMessage.Enabled = loggedIn && !string.IsNullOrEmpty(txtBoxUser.Text) && !string.IsNullOrWhiteSpace(txtBoxMessage.Text);
        }
    }
}