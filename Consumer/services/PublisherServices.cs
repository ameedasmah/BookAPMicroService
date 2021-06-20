using Contract.Entities;
using Contract.Resourse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Consumer.services
{
    public interface IPublisher
    {
        Task CreatePublisher(int Id);
        Task UpdatePublisher(int Id);
        Task RemovePublisher(int Id);
    }
    public interface ToRecive
    {
        public int Id { get; set; }
        public Publisher publisher { get; set; }
        public string Type { get; set; }
    }
    class PublisherServices : IPublisher
    {
        private readonly HttpClient _httpClient;
        private readonly IPublisher _publisher;

        public PublisherServices(HttpClient httpClient,IPublisher publisher)
        {
            _httpClient = httpClient;
            _publisher = publisher;
        }

        string URI = "http://localhost/44325/api/";

        public async Task CreatePublisher(int Id)
        {
            Uri geturi = new Uri(URI + "Publisher/" + Id); 

            var response = await _httpClient.GetAsync(geturi);
            response.EnsureSuccessStatusCode();
             var responseString = await response.Content.ReadAsStringAsync();
            var data =  JsonConvert.DeserializeObject<PublisherResource>(responseString);
            Console.WriteLine($"myData{data}");
        }

        public Task RemovePublisher(int Id)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePublisher(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
