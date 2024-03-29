﻿using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRChatClient
{
    public partial class MainWindow : Window
    {
        HubConnection connection;
        public MainWindow()
        {
            InitializeComponent();

            connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/chat")
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0,5) * 1000);
                await connection.StartAsync();
            };
        }

        private async void connectButton_Click(object sender, RoutedEventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (message, userName) =>
            {
                Dispatcher.Invoke(() =>
                {
                   var newMessage = $"{userName}: {message}";
                   messagesList.Items.Add(newMessage);
                });
            });

            connection.On<string>("Disconnect", (userID) =>
            {
                Dispatcher.Invoke(() =>
                {
                    Application.Current.Shutdown();
                });
            });

            try
            {
                await connection.StartAsync();
                await connection.InvokeAsync("AddToGuestGroup", userTextBox.Text);
                messagesList.Items.Add("Connection started");
                connectButton.IsEnabled = false;
                sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                messagesList.Items.Add(ex.Message);
            }
        }

        private async void sendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await connection.InvokeAsync("SendMessage", messageTextBox.Text, userTextBox.Text);
            }
            catch (Exception ex)
            {                
                messagesList.Items.Add(ex.Message);                
            }
        }
    }
}