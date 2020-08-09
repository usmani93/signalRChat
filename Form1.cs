using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace netcore
{
    public partial class Form1 : Form
    {
        HubConnection hubConnection;
        private object BackendURL;
        private bool connected;

        public Form1()
        {
            InitializeComponent();
            BackendURL = "http://192.168.1.109:5000";
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{BackendURL}/hubs/messages")
                .Build();

            StartConnection();
        }

        async void StartConnection()
        {
            if (!connected)
            {
                await hubConnection.StartAsync();
                connected = true;
            }

            hubConnection.On<string>("NewItem", (item) =>
            {
                try
                {
                    var newItem = JsonConvert.DeserializeObject(item);
                    values = item;
                    listBox1.Items.Add(newItem.ToString());
                }
                catch (Exception ex)
                {

                }
            });

            MessageBox.Show("connected");
        }

        public string values { get; private set; }

        private void button1_Click(object sender, EventArgs e)
        {
            GetMessage();
        }

        async void GetMessage()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri($"http://192.168.1.109:5000/");
            var json = await client.GetStringAsync($"api/Messages");
            var items = Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Message>>(json));
            var itemss = items.Result;
            foreach (var item in itemss)
            {
                listBox1.Items.Add(item.Details);
            }
        }
    }

    public class Message
    {
        public string Details { get; set; }
        public string Id { get; set; }
        public string Text { get; set; }
    }
}
