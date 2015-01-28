using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft .AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;
using System.Net;

namespace SignalR.Dynamic.Client.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            System.Console.ReadLine();
            //Set connection
            var connection = new HubConnection("http://localhost:8596",true);
            connection.Credentials = CredentialCache.DefaultCredentials;
            //Make proxy to hub based on hub name on server
            var myHub = connection.CreateHubProxy("HealthMonitor");
            //Start connection

            myHub.On<string>("BroadcastMessage", param =>
            {
                System.Console.WriteLine(param);
            });

            connection.Start().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    System.Console.WriteLine("There was an error opening the connection:{0}",
                                      task.Exception.GetBaseException());
                }
                else
                {
                    System.Console.WriteLine("Connected");
                }

            }).Wait();

            
            System.Console.Read();
            connection.Stop();
        }
    }
}
