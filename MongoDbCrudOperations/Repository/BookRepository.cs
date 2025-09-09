using MongoDB.Driver;
using MongoDbCrudOperations.Configuration;
using MongoDbCrudOperations.Dto;
using MongoDbCrudOperations.Entities;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace MongoDbCrudOperations.Repository;

public interface IBookRepository
{
    Task<Book> AddBookAsync(Book book);
    Task<List<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(Guid id);
    Task<Book?> GetBookByTitleAsync(string title);
    Task<bool> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto);
    Task<bool> DeleteBookAsync(Guid id);
    Task<bool> DeleteBookByTitleAsync(string title);
    Task<bool> BookExistsAsync(string title);
}

public class BookRepository : IBookRepository
{
    private readonly IMongoCollection<Book> _collection;
    private readonly ILogger<BookRepository> _logger;

    public BookRepository(IOptions<MongoDbSettings> settings, ILogger<BookRepository> logger)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<Book>(settings.Value.CollectionName);
        _logger = logger;
    }

    public async Task<Book> AddBookAsync(Book book)
    {
        try
        {
            await _collection.InsertOneAsync(book);
            _logger.LogInformation("Book added successfully with ID: {BookId}", book.Id);
            return book;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding book with title: {Title}", book.Title);
            throw;
        }
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        try
        {
            var result = await _collection.FindAsync(_ => true);
            var books = await result.ToListAsync();
            _logger.LogInformation("Retrieved {Count} books", books.Count);
            return books;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all books");
            throw;
        }
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        try
        {
            var result = await _collection.FindAsync(book => book.Id == id);
            return await result.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book with ID: {BookId}", id);
            throw;
        }
    }

    public async Task<Book?> GetBookByTitleAsync(string title)
    {
        try
        {
            var result = await _collection.FindAsync(book => book.Title == title);
            return await result.FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book with title: {Title}", title);
            throw;
        }
    }

    public async Task<bool> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto)
    {
        try
        {
            var filter = Builders<Book>.Filter.Eq(book => book.Id, id);
            var update = Builders<Book>.Update;

            var updateDefinition = update.Set(book => book.Title, updateBookDto.Title);

            if (!string.IsNullOrEmpty(updateBookDto.Author))
                updateDefinition = updateDefinition.Set(book => book.Author, updateBookDto.Author);

            if (!string.IsNullOrEmpty(updateBookDto.Description))
                updateDefinition = updateDefinition.Set(book => book.Description, updateBookDto.Description);

            if (updateBookDto.PageCount.HasValue)
                updateDefinition = updateDefinition.Set(book => book.PageCount, updateBookDto.PageCount.Value);

            if (updateBookDto.PublishDate.HasValue)
                updateDefinition = updateDefinition.Set(book => book.PublishDate, updateBookDto.PublishDate.Value);

            var result = await _collection.UpdateOneAsync(filter, updateDefinition);
            _logger.LogInformation("Updated {Count} book(s) with ID: {BookId}", result.ModifiedCount, id);
            return result.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating book with ID: {BookId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteBookAsync(Guid id)
    {
        try
        {
            var result = await _collection.DeleteOneAsync(book => book.Id == id);
            _logger.LogInformation("Deleted {Count} book(s) with ID: {BookId}", result.DeletedCount, id);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting book with ID: {BookId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteBookByTitleAsync(string title)
    {
        try
        {
            var result = await _collection.DeleteOneAsync(book => book.Title == title);
            _logger.LogInformation("Deleted {Count} book(s) with title: {Title}", result.DeletedCount, title);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting book with title: {Title}", title);
            throw;
        }
    }

    public async Task<bool> BookExistsAsync(string title)
    {
        try
        {
            var count = await _collection.CountDocumentsAsync(book => book.Title == title);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if book exists with title: {Title}", title);
            throw;
        }
    }
}