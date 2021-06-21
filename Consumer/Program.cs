using Consumer.services;
using Contract.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            Console.WriteLine("consumerrrrrrrrrrrrrrrrr ");

            string Connection = "server=.;database=Book;Trusted_Connection=true";
            var serviceProvider = new ServiceCollection();
            ConfigureServices(serviceProvider);
            var services = serviceProvider.BuildServiceProvider();
            var publisherService = services.GetRequiredService<PublisherServices>();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "publisher", durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    string jsonString = Encoding.UTF8.GetString(body);
                    var json = JsonConvert.DeserializeObject<ToRecive>(jsonString);

                    var message = Encoding.UTF8.GetString(body);
                    var PublisherTojson = JsonConvert.DeserializeObject<ToRecive>(message);
                    switch (PublisherTojson.Type)
                    {
                        case "Create":
                            await publisherService.CreatePublisher(PublisherTojson.Id);
                            break;
                        case "Update":
                            await publisherService.UpdatePublisher(PublisherTojson.Id);
                            break;
                        case "Delete":
                            await publisherService.RemovePublisher(PublisherTojson.Id);
                            break;
                        default: break;
                    }
                };
                channel.BasicConsume(queue: "publisher",
                                    autoAck: true,
                                    consumer: consumer);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            string Connection = "server=.;database=Book;Trusted_Connection=true";
            var serviceProvider = services.AddDbContext<BookContext>(options => options.UseSqlServer(Connection, b => b.MigrationsAssembly("WebApplication1").UseNetTopologySuite()))
            .AddScoped<IPublisherRepositories, PublisherReposoitories>()
            .AddScoped<PublisherServices>()
            .AddHttpClient<PublisherServices>();
        }
    }
}
