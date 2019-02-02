using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalR.App
{
    class Program
    {
        private static HubConnection connection;


        static void Main(string[] args)
        {
            Console.WriteLine("SignalR from .Net client test!");

            var connection = new HubConnectionBuilder()
               .WithUrl("http://localhost:53333/ChatHub")
               .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            Task.Run(async () => {

                connection.On<string>("ReceiveMessage", (message) =>
                {
                    Console.WriteLine($"Message From SignalR: {message}");
            
                });

                try
                {
                    await connection.StartAsync();
                }
                catch (Exception ex)
                {
                }

            });
           
          Console.ReadLine();

        }

       
    }
}
