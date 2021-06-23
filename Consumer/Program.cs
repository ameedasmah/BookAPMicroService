using Consumer.services;
using Contract.Entities;
using Domins.mangers;
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
            var AuthorService = services.GetRequiredService<AuthorService>();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "publisher", durable: true, exclusive: false, autoDelete: false, arguments: null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var ObjTojson = JsonConvert.DeserializeObject<ToRecive>(message);
                    if (ObjTojson.OperationType == "Publisher")
                    {

                        switch (ObjTojson.Type)
                        {
                            case "Create":
                                await publisherService.CreatePublisher(ObjTojson.Id);
                                break;
                            case "Update":
                                await publisherService.UpdatePublisher(ObjTojson.Id);
                                break;
                            case "Delete":
                                await publisherService.RemovePublisher(ObjTojson.Id);
                                break;
                            default: break;
                        }
                    }
                    if (ObjTojson.OperationType == "Author")
                    {
                        switch (ObjTojson.Type)
                        {
                            case "Create":
                                await AuthorService.CreateAuthor(ObjTojson.Id);
                                break;
                            case "Update":
                                await AuthorService.UpdateAuthor(ObjTojson.Id);
                                break;
                            case "Delete":
                                await AuthorService.RemoveAuthor(ObjTojson.Id);

                                break;
                            default: break;
                        }
                    }
                };
                channel.BasicConsume(queue: "publisher",
                                        autoAck: true,
                                        consumer: consumer);

                //Harvest Channel

                channel.QueueDeclare(queue: "Harvest", durable: false, exclusive: false, autoDelete: false, arguments: null);
                var consumerHarvest = new EventingBasicConsumer(channel);
                consumerHarvest.Received += async (model, ea) =>
               {
                   var body = ea.Body.ToArray();
                   var message = Encoding.UTF8.GetString(body);
                   if (message == "HarvestPublishers")
                   {
                       await publisherService.GetPublishers();
                   }
                   if (message == "HarvestAuthor")
                   {
                       await AuthorService.GetAuthors();
                   }

               };
                /*should to connect with Service*/


                channel.BasicConsume(queue: "Harvest",
                               autoAck: true,
                               consumer: consumerHarvest);
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            string Connection = "server=.;database=BookTask;Trusted_Connection=true";
            var serviceProvider = services.AddDbContext<BookContext>(options => options.UseSqlServer(Connection, b => b.MigrationsAssembly("WebApplication1").UseNetTopologySuite()))
            .AddScoped<IPublisherRepositories, PublisherReposoitories>()
            .AddScoped<IPublisherRepositories, PublisherReposoitories>()
            .AddScoped<IAuthorRepositories, AuthorRepositories>()
            .AddScoped<PublisherServices>()
            .AddScoped<AuthorService>()
            .AddHttpClient<PublisherServices>();
            //.AddHttpClient<AuthorService>();

        }
        
    }
}