using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TA1718._1._003
{
    class RMQ
    {
        //default settings
        //private const string hostname_local = "167.205.66.191";
        private const string hostname_itb = "167.205.7.226";
        private const int port = 5672;
        private const string username = "kondisiruang";
        private const string password = "kondisiruang";
        private const string virtualhost = "/kondisiruang";

        public ConnectionFactory connectionFactory;
        public IConnection connection;
        public IModel channel;

        private string data = "";
        private string data_voice = "";
        private string data_face = "";
        private string data_gesture = "";
        private string data_gesture_kinect = "";

        public RMQ()
        {
            InitRMQConnection();
            CreateRMQConnection();
        }

        private void InitRMQConnection(string host = hostname_itb, int port = port, string user = username, string pass = password, string vhost = virtualhost)
        {
            connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = host;
            connectionFactory.Port = port;
            connectionFactory.UserName = user;
            connectionFactory.Password = pass;
            connectionFactory.VirtualHost = vhost;
        }

        private void CreateRMQConnection()
        {
            connection = connectionFactory.CreateConnection();
            Debug.WriteLine("Koneksi " + (connection.IsOpen ? "Berhasil!" : "Gagal!"));
        }

        public void createChannel(string queue, string exchange, string routing_key, bool durable = true, string type = "topic")
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();

                Debug.WriteLine("Channel : " + channel.IsOpen);
            }

            if (channel.IsOpen)
            {
                Debug.WriteLine("Declare Exchange");
                channel.ExchangeDeclare(exchange: exchange,
                                        type: type,
                                        durable: true);

                Debug.WriteLine("Declare Queue");
                channel.QueueDeclare(queue: queue,
                                     durable: durable,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                Debug.WriteLine("Bind Queue");
                channel.QueueBind(queue: queue,
                                  exchange: exchange,
                                  routingKey: routing_key);

                Debug.WriteLine("Declare Consuming Queue Process");
                var consumer = new EventingBasicConsumer(channel);

                Debug.WriteLine("Begin Receiving data");

                consumer.Received += (model, ea) =>
                {

                    Debug.WriteLine("Retriving......");
                    var body = ea.Body;
                    data = Encoding.UTF8.GetString(body);
                    Debug.WriteLine("Data : " + data);
                };

                channel.BasicConsume(queue: queue,
                                    noAck: true,
                                    consumer: consumer);

            }

        }

        public void createChannel_Voice(string queue, string exchange, string routing_key, bool durable = true, string type = "topic")
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();

                Debug.WriteLine("Channel : " + channel.IsOpen);
            }

            if (channel.IsOpen)
            {
                Debug.WriteLine("Declare Exchange");
                channel.ExchangeDeclare(exchange: exchange,
                                        type: type,
                                        durable: true);

                Debug.WriteLine("Declare Queue");
                channel.QueueDeclare(queue: queue,
                                     durable: durable,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                Debug.WriteLine("Bind Queue");
                channel.QueueBind(queue: queue,
                                  exchange: exchange,
                                  routingKey: routing_key);

                Debug.WriteLine("Declare Consuming Queue Process");
                var consumer = new EventingBasicConsumer(channel);

                Debug.WriteLine("Begin Receiving data");

                consumer.Received += (model, ea) =>
                {

                    Debug.WriteLine("Retriving......");
                    var body = ea.Body;
                    data_voice = Encoding.UTF8.GetString(body);
                    Debug.WriteLine("Data : " + data_voice);
                };

                channel.BasicConsume(queue: queue,
                                    noAck: true,
                                    consumer: consumer);

            }

        }

        public void createChannel_Face(string queue, string exchange, string routing_key, bool durable = true, string type = "topic")
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();

                Debug.WriteLine("Channel : " + channel.IsOpen);
            }

            if (channel.IsOpen)
            {
                Debug.WriteLine("Declare Exchange");
                channel.ExchangeDeclare(exchange: exchange,
                                        type: type,
                                        durable: true);

                Debug.WriteLine("Declare Queue");
                channel.QueueDeclare(queue: queue,
                                     durable: durable,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                Debug.WriteLine("Bind Queue");
                channel.QueueBind(queue: queue,
                                  exchange: exchange,
                                  routingKey: routing_key);

                Debug.WriteLine("Declare Consuming Queue Process");
                var consumer = new EventingBasicConsumer(channel);

                Debug.WriteLine("Begin Receiving data");

                consumer.Received += (model, ea) =>
                {

                    Debug.WriteLine("Retriving......");
                    var body = ea.Body;
                    data_face = Encoding.UTF8.GetString(body);
                    Debug.WriteLine("Data : " + data_face);
                };

                channel.BasicConsume(queue: queue,
                                    noAck: true,
                                    consumer: consumer);

            }

        }

        public void createChannel_Gesture(string queue, string exchange, string routing_key, bool durable = true, string type = "topic")
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();

                Debug.WriteLine("Channel : " + channel.IsOpen);
            }

            if (channel.IsOpen)
            {
                Debug.WriteLine("Declare Exchange");
                channel.ExchangeDeclare(exchange: exchange,
                                        type: type,
                                        durable: true);

                Debug.WriteLine("Declare Queue");
                channel.QueueDeclare(queue: queue,
                                     durable: durable,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                Debug.WriteLine("Bind Queue");
                channel.QueueBind(queue: queue,
                                  exchange: exchange,
                                  routingKey: routing_key);

                Debug.WriteLine("Declare Consuming Queue Process");
                var consumer = new EventingBasicConsumer(channel);

                Debug.WriteLine("Begin Receiving data");

                consumer.Received += (model, ea) =>
                {

                    Debug.WriteLine("Retriving......");
                    var body = ea.Body;
                    data_gesture = Encoding.UTF8.GetString(body);
                    Debug.WriteLine("Data : " + data_gesture);
                };

                channel.BasicConsume(queue: queue,
                                    noAck: true,
                                    consumer: consumer);

            }

        }

        public void createChannel_Gesture_Kinect(string queue, string exchange, string routing_key, bool durable = true, string type = "topic")
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();

                Debug.WriteLine("Channel : " + channel.IsOpen);
            }

            if (channel.IsOpen)
            {
                Debug.WriteLine("Declare Exchange");
                channel.ExchangeDeclare(exchange: exchange,
                                        type: type,
                                        durable: true);

                Debug.WriteLine("Declare Queue");
                channel.QueueDeclare(queue: queue,
                                     durable: durable,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                Debug.WriteLine("Bind Queue");
                channel.QueueBind(queue: queue,
                                  exchange: exchange,
                                  routingKey: routing_key);

                Debug.WriteLine("Declare Consuming Queue Process");
                var consumer = new EventingBasicConsumer(channel);

                Debug.WriteLine("Begin Receiving data");

                consumer.Received += (model, ea) =>
                {

                    Debug.WriteLine("Retriving......");
                    var body = ea.Body;
                    data_gesture_kinect = Encoding.UTF8.GetString(body);
                    Debug.WriteLine("Data : " + data_gesture_kinect);
                };

                channel.BasicConsume(queue: queue,
                                    noAck: true,
                                    consumer: consumer);

            }

        }

        public void CreateRMQChannel2( string queue_publish, string exchange_name_publish, string routing_key_publish)
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();

                Debug.WriteLine("Channel : " + channel.IsOpen);
            }

            if (channel.IsOpen)
            {
                Debug.WriteLine("Declare Exchange");
                channel.ExchangeDeclare(exchange: exchange_name_publish,
                                        type: "topic",
                                        durable: true);


                Debug.WriteLine("Declare Queue Publish");
                channel.QueueDeclare(queue: queue_publish,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                Debug.WriteLine("Bind Queue");
                channel.QueueBind(queue: queue_publish,
                                  exchange: exchange_name_publish,
                                  routingKey: routing_key_publish);

            }

        }

        public void CreateRMQChannel(string queue_publish, string exchange_name_publish, string routing_key_publish)
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();

                Debug.WriteLine("Channel : " + channel.IsOpen);
            }

            if (channel.IsOpen)
            {
                Debug.WriteLine("Declare Exchange");
                channel.ExchangeDeclare(exchange: exchange_name_publish,
                                        type: "topic",
                                        durable: true);


                Debug.WriteLine("Declare Queue Publish");
                channel.QueueDeclare(queue: queue_publish,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                Debug.WriteLine("Bind Queue");
                channel.QueueBind(queue: queue_publish,
                                  exchange: exchange_name_publish,
                                  routingKey: routing_key_publish);

            }

        }

        public void SendMessage(string exchange_name, string routing_key, string msg = "send")
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(msg);
            channel.BasicPublish(exchange: exchange_name,
                                routingKey: routing_key,
                                basicProperties: null,
                                body: responseBytes);
        }

        public void deleteQueue(string queue_name)
        {
            using (channel = connection.CreateModel())
            {
                if (channel.IsOpen)
                {
                    channel.QueueDelete(queue_name);
                }
            }
        }

        public void deleteExchange(string exchange_name)
        {
            using (channel = connection.CreateModel())
            {
                if (channel.IsOpen)
                {
                    channel.ExchangeDelete(exchange_name);
                }
            }
        }

        public string getData()
        {
            return data;
        }

        public string getData_Voice()
        {
            return data_voice;
        }

        public string getData_Face()
        {
            return data_face;
        }

        public string getData_Gesture()
        {
            return data_gesture;
        }

        public string getData_Gesture_Kinect()
        {
            return data_gesture_kinect;
        }

        ~RMQ()
        {
            Disconnect();
        }

        public void Disconnect()
        {
            channel.Close();
            channel = null;
            Debug.WriteLine("Channel ditutup!");
            if (connection.IsOpen)
            {
                connection.Close();
            }
            Debug.WriteLine("Koneksi diputus!");
            connection.Dispose();
            connection = null;
        }
    }
}

