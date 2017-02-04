using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR
{
    public class eve3Hub : Hub
    {
        public void SendString(string message)
        {
            Console.WriteLine(message);
            string newMessage = "HelloWorld";
            Clients.All.ReceiveString(newMessage);
        }
    }
}
