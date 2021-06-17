using Contract.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebApplication1.Repositories;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string Connection = "server=.;database=Book;Trusted_Connection=true";
            var serviceProvider = new ServiceCollection().AddDbContext<BookContext>(options => options.UseSqlServer(Connection))
             .AddScoped<IPublisherRepositories, PublisherReposoitories>()
             .BuildServiceProvider();
            var publisherService = serviceProvider.GetRequiredService<IPublisherRepositories>();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "publisher", durable: false, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Wating message ");
                    var ConvertTojson = JsonConvert.DeserializeObject<JObject>(message);
                };
            }
        }

    }
}
