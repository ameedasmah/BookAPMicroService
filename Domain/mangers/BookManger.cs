using Contract.Entities;
using Contract.models;
using Contract.Resourse;
using Domins.mangers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Helper;

namespace Domain.mangers
{
    public interface IBookManger
    {
        Task<IEnumerable<BookPublisherResource>> GetBooks();
        Task<BookPublisherResource> GetBook(int id);
        Task<BookPublisherResource> PostBook(BookModel bookModel);
        Task<BookPublisherResource> PutBook(int id, BookModel book);
        Task Delete(int id);
    }
   public class BookManger : IBookManger
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepositories _authorRepositories;

        public BookManger(IBookRepository bookRepository, IAuthorRepositories authorRepositories)
        {
        _bookRepository= bookRepository;
        _authorRepositories= authorRepositories;

        }
        public async Task Delete(int id)
        {
            var bookToDelete = await _bookRepository.Get(id);
            if (bookToDelete == null) { throw new Exception("book to delete is null"); }
            await _bookRepository.Delete(bookToDelete.Id);
        }

        public async Task<IEnumerable<BookPublisherResource>> GetBooks()
        {
            var BookEntities = await _bookRepository.Get();
            var bookResources = new List<BookPublisherResource>();
            foreach (var item in BookEntities)
            {
                bookResources.Add(item.ToResourceNew());
            }
            return bookResources;
        }

        public async Task<BookPublisherResource> GetBook(int id)
        {
            var BookEntities = await _bookRepository.Get(id);
            if (BookEntities is null)
            {
                throw new Exception("there is no book to return");
            }
            return BookEntities.ToResourceNew();
        }

        public async Task<BookPublisherResource> PostBook(BookModel bookModel)
        {
            var author = _authorRepositories.GetAuthors(item => bookModel.AuthorIds.Contains(item.Id)).Result.ToList();
            var newBook = new Book()
            {
                Title = bookModel.Title,
                Discraptions = bookModel.Discraptions,
                PublisherId = bookModel.PublisherId,
                Authors = author
            };

            var newBooksss = await _bookRepository.Create(newBook);
            return newBooksss.ToResourceNew();
        }

        public async Task<BookPublisherResource> PutBook(int id, BookModel book)
        {
            var bookToUpdate = await _bookRepository.Get(id);
            if (bookToUpdate == null)
            {
                //return NotFound();
                throw new Exception("there is no book");
            }

            var authors = await _authorRepositories.GetAuthors(item => book.AuthorIds.Contains(item.Id));

            bookToUpdate.PublisherId = book.PublisherId;
            bookToUpdate.Title = book.Title;
            bookToUpdate.Discraptions = book.Discraptions;
            authors = authors;


            var BookEntities = await _bookRepository.Update(bookToUpdate);
            var bookResources = BookEntities.ToResourceNew();
            return bookResources;
        }

    
    }
}
