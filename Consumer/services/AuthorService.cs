using Contract.Entities;
using Contract.Resourse;
using Domins.mangers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.services
{
    public class ToReciveAuthor
    {
        public int Id { get; set; }
        public string OperationType { get; set; }
        public string Type { get; set; }
    }
    public interface IAuthor
    {
        Task CreateAuthor(int Id);
        Task<Exception> UpdateAuthor(int Id);
        Task RemoveAuthor(int Id);
        Task GetAuthors();
    }
    class AuthorService
    {
        private readonly HttpClient _httpClient;
        private readonly BookContext _Context;
        private readonly IAuthorRepositories _AuthorRepositories;
        public AuthorService(HttpClient httpClient, BookContext Context, IAuthorRepositories AuthorRepositories)
        {
            _httpClient = httpClient;
            _Context = Context;
            _AuthorRepositories = AuthorRepositories;
        }

        string URI = "https://localhost:5001/api/";
        public async Task CreateAuthor(int Id)
        {
            Uri getUri = new Uri(URI + "authors" + Id);
            var response = await _httpClient.GetAsync(getUri, HttpCompletionOption.ResponseHeadersRead);
            Console.WriteLine($" is it json :::::::::::::: {response}");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<AuthorResource>(responseString);
            //write on db
            Console.WriteLine($"myData{data}");
            var AuthorEntities = new Author()
            {
                Id = data.Id,
                FullName = data.FullName,
                Email = data.Email,
                Age = data.Age,
            };
            await _AuthorRepositories.CreateAuthor(AuthorEntities);
        }
        public async Task RemoveAuthor(int Id)
        {
            try
            {
                Uri geturl = new Uri(URI + "authors/" + Id);
                var response = await _httpClient.GetAsync(geturl, HttpCompletionOption.ResponseHeadersRead);
                if (response != null)
                {
                    await _AuthorRepositories.Delete(Id);
                }
            }

            catch (Exception ex)
            {
                throw new Exception($"there is No Id to Delete{ex.Message}");
            }
        }
        public async Task<Exception> UpdateAuthor(int Id)
        {
            try
            {
                Uri geturl = new Uri(URI + "authors/" + Id);
                var response = await _httpClient.GetAsync(geturl);
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<AuthorResource>(responseString);
                Console.WriteLine($"myData {data}");
                var publisherEntities = new Author()
                {
                    Id = data.Id,
                    FullName = data.FullName,
                    Email = data.Email,
                    Age = data.Age,

                };
                await _AuthorRepositories.Update(publisherEntities);
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
        //Harvest
        string URL = "https://localhost:5001/api/";
        public async Task GetAuthors()
        {
            Uri geturl = new Uri(URL + "Publisher/");
            var response = await _httpClient.GetAsync(geturl, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<List<AuthorResource>>(responseString);
            Console.WriteLine($"myData{data[0]}");
        }
    }
}