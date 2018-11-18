using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace MqttPc
{
    class Program
    {
        static void Main(string[] args)
        {

            string caCert_path = @"C:\Users\nunxy\UNIVERSITA'\Magistrale\Projects and laboratory on communication systems\provaMQTT\Asus_nunzio.pfx";
            string clientCert_path = @"C:\Users\nunxy\UNIVERSITA'\Magistrale\Projects and laboratory on communication systems\provaMQTT\Asus_nunzio.pfx";
            string password = "password";

            string IotEndPoint = "a2w9oltzedidx9.iot.us-east-1.amazonaws.com";
            Console.WriteLine("AWS IOT Dotnet core message publiser starting");
            int BrokerPort = 8883;
            string Topic = "data";

            var CaCert = X509Certificate.CreateFromCertFile(caCert_path);
            var ClientCert = new X509Certificate2(clientCert_path, password);

            var Message = "aLotOfData";
            string ClientId = Guid.NewGuid().ToString();

            var IotClient = new MqttClient(IotEndPoint, BrokerPort, true, CaCert, ClientCert, MqttSslProtocols.TLSv1_2);

            Console.WriteLine("TRY TO Connect to AWS IOT");
            IotClient.Connect(ClientId);
            Console.WriteLine("Connected to AWS IOT");


            IotClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            IotClient.MqttMsgSubscribed += Client_MqttMsgSubscribed;

            //IotClient.Connect(ClientId);
            Console.WriteLine("Connected");
            IotClient.Subscribe(new string[] { "errors" }, new byte[] { uPLibrary.Networking.M2Mqtt.Messages.MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

            int i = 0;


            while (true)
            {
                Message = "aLotOfData"; 
                Message = Message + " " + i.ToString();

                IotClient.Publish(Topic, Encoding.UTF8.GetBytes(Message));
                Console.WriteLine("published: " + Message);
                Thread.Sleep(5000);
                i++;

            }

        }

        private static void Client_MqttMsgSubscribed(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgSubscribedEventArgs e)
        {
            Console.WriteLine("Message subscribed");
        }

        private static void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            Console.WriteLine("Message Received is      " + System.Text.Encoding.UTF8.GetString(e.Message));
        }
    }
}
