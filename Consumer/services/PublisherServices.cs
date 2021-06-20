using Contract.Entities;
using Contract.Resourse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public class ToRecive
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
    public class PublisherServices : IPublisher
    {
        private readonly HttpClient _httpClient;

        public PublisherServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        string URI = "http://localhost/44325/api/";

        public async Task CreatePublisher(int Id)
        {
            Uri geturl = new Uri(URI + "Publisher/" + Id);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(geturl);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var item = await reader.ReadToEndAsync();
                var josnObject = JsonConvert.DeserializeObject<Publisher>(item);
            }
        }

        public Task UpdatePublisher(int Id)
        {
            throw new NotImplementedException();
        }

        public Task RemovePublisher(int Id)
        {
            throw new NotImplementedException();
        }

        //    var response = await _httpClient.GetAsync(geturl);
        //response.EnsureSuccessStatusCode();
        //var responseString = await response.Content.ReadAsStringAsync();
        //var data = JsonConvert.DeserializeObject<PublisherResource>(responseString);
        //Console.WriteLine($"myData{data}");
    }
  
    }

