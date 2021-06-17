using Contract.Entities;
using Contract.models;
using Contract.Resourse;
using Domain.mangers;
using Domins.mangers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Helper;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookManger BookManger;
        public BooksController(IBookManger BookManger)
        {
            this.BookManger = BookManger;

        }
        [HttpGet]
        public async Task<IEnumerable<BookPublisherResource>> GetBooks()
        {
            return await BookManger.GetBooks();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookPublisherResource>> GetBooks(int id)
        {

            return await BookManger.GetBook(id);
        }

        [HttpPost]
        public async Task<ActionResult<BookPublisherResource>> PostBooks([FromBody] BookModel bookModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return await BookManger.PostBook(bookModel);
        }
        [HttpPut("{id}")]

        public async Task<ActionResult<BookPublisherResource>> PutBooks(int id, [FromBody] BookModel book)
        {
            return await BookManger.PutBook(id, book);

        }
        [HttpDelete("{id}")]
        public async Task Delete(int id)
        {
            await BookManger.Delete(id);
        }

    }
}
