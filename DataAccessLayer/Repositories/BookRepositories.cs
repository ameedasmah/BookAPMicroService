using Contract.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domins.mangers
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> Get();
        Task<Book> Get(int id);
        Task<Book> Create(Book book);
        Task<Book> Update(Book book);
        Task Delete(int id);
    }

    public class BookRepositories : IBookRepository
    {

        private readonly BookContext _Context;

        public BookRepositories(BookContext Context)
        {
            _Context = Context;

        }
        public async Task<Book> Create(Book book)
        {
            _Context.Books.Add(book);

            await _Context.SaveChangesAsync();

            return await _Context.Books.Include(X => X.Publisher).Include(x => x.Authors).FirstOrDefaultAsync(X => X.Id == book.Id);
        }

        public async Task Delete(int Id)
        {
            var bookToDelete = await _Context.Books.Include(x => x.Authors).FirstOrDefaultAsync(X => X.Id == Id);
            _Context.Remove(bookToDelete);

            await _Context.SaveChangesAsync();

        }

        public async Task<IEnumerable<Book>> Get()
        {
            try
            {

                return await _Context.Books.Include(x => x.Authors).Include(X => X.Publisher).ToListAsync();
            }
            catch (Exception exiption)
            {
                throw new Exception($" there is no Book to retrive {exiption.Message}");
            }
        }

        public async Task<Book> Get(int Id)
        {
            try
            {

                return await _Context.Books.Include(X => X.Publisher).Include(X => X.Authors).FirstOrDefaultAsync(X => X.Id == Id);
            }
            catch (Exception exiption)
            {
                throw new Exception($"there is no Book to retrive {exiption.Message}");
            }
        }

        public async Task<Book> Update(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException($"{nameof(Update)} must not be null");
            }
            try
            {

                _Context.Books.Update(book);
                await _Context.SaveChangesAsync();
                return book;
            }
            catch (Exception ex)
            {
                throw new Exception($"coulen't update book :{ex.Message}");
            }

        }

    }
}

