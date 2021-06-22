using Contract.Entities;
using Contract.Resourse;
using Domain.mangers;
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
using WebApplication1.Repositories;

namespace Consumer.services
{
    public interface IPublisher
    {
        Task CreatePublisher(int Id);
        Task<Exception> UpdatePublisher(int Id);
        Task RemovePublisher(int Id);
        Task GetPublishers();

    }
    public class ToRecive
    {
        public int Id { get; set; }
        public string Type { get; set; }
    }
    public class PublisherServices : IPublisher
    {
        private readonly HttpClient _httpClient;
        private readonly BookContext _Context;
        private readonly IPublisherRepositories _repository;

        public PublisherServices(HttpClient httpClient, BookContext Context, IPublisherRepositories repository)
        {
            _httpClient = httpClient;
            _Context = Context;
            _repository = repository;
        }

        string URI = "https://localhost:5001/api/";

        public async Task CreatePublisher(int Id)
        {
            Uri geturl = new Uri(URI + "Publisher/" + Id);
            var response = await _httpClient.GetAsync(geturl, HttpCompletionOption.ResponseHeadersRead);
            Console.WriteLine($" is it json :::::::::::::: {response}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<PublisherResource>(responseString);
            // write on db 
            Console.WriteLine($"myData{data}");
            var publisherEntities = new Publisher()
            {
                Id = data.Id,
                Name = data.Name,
                Email = data.Email,
                Salery = data.Salary,
                DateOfBirth = data.DateOfBirth
            };
            await _repository.CreatePublisher(publisherEntities);
        }
        public async Task<Exception> UpdatePublisher(int Id)
        {
            try
            {
                Uri geturl = new Uri(URI + "Publisher/" + Id);
                var response = await _httpClient.GetAsync(geturl);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<PublisherResource>(responseString);
                Console.WriteLine($"myData {data.Salary}");
                var publisherEntities = new Publisher()
                {
                    Id = data.Id,
                    Name = data.Name,
                    Email = data.Email,
                    DateOfBirth = data.DateOfBirth,
                    Salery = data.Salary,
                };
                await _repository.updatePublisher(publisherEntities);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public async Task RemovePublisher(int Id)
        {
            try
            {
                Uri geturl = new Uri(URI + "Publisher/" + Id);
                var response = await _httpClient.GetAsync(geturl, HttpCompletionOption.ResponseHeadersRead);
                if (response != null)
                {
                    await _repository.deletePublisher(Id);
                }
            }

            catch (Exception ex)
            {
                throw new Exception($"there is No Id to Delete{ex.Message}");
            }

        }

        //Harvest
        string URL = "https://localhost:5001/api/";
        public async Task GetPublishers()
        {
            Uri geturl = new Uri(URL + "Publisher/" );
            var response = await _httpClient.GetAsync(geturl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<PublisherResource>>(responseString);
            Console.WriteLine($"myData{data}");
       
        }
    }

}


