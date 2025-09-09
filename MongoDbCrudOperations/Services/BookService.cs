using MongoDbCrudOperations.Dto;
using MongoDbCrudOperations.Entities;
using MongoDbCrudOperations.Repository;
using MongoDbCrudOperations.Services;
using Microsoft.Extensions.Logging;

namespace MongoDbCrudOperations.Services;

public interface IBookService
{
    Task<Book> CreateBookAsync(CreateBookDto createBookDto);
    Task<List<Book>> GetAllBooksAsync();
    Task<Book?> GetBookByIdAsync(Guid id);
    Task<Book?> GetBookByTitleAsync(string title);
    Task<bool> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto);
    Task<bool> UpdateBookByTitleAsync(string title, UpdateBookDto updateBookDto);
    Task<bool> DeleteBookAsync(Guid id);
    Task<bool> DeleteBookByTitleAsync(string title);
    Task<bool> BookExistsAsync(string title);
}

public class BookService(
    IBookRepository bookRepository,
    IValidationService validationService,
    ILogger<BookService> logger)
    : IBookService
{
    private readonly ILogger<BookService> _logger = logger;

    public async Task<Book> CreateBookAsync(CreateBookDto createBookDto)
    {
        var validationResult = await validationService.ValidateAsync(createBookDto);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException($"Validation failed: {validationResult.ErrorMessage}");
        }

        // Check if book with same title already exists
        if (await bookRepository.BookExistsAsync(createBookDto.Title))
        {
            throw new InvalidOperationException($"A book with title '{createBookDto.Title}' already exists.");
        }

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Author = createBookDto.Author,
            Title = createBookDto.Title,
            Description = createBookDto.Description,
            PageCount = createBookDto.PageCount,
            PublishDate = createBookDto.PublishDate
        };

        return await bookRepository.AddBookAsync(book);
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await bookRepository.GetAllBooksAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid id)
    {
        return await bookRepository.GetBookByIdAsync(id);
    }

    public async Task<Book?> GetBookByTitleAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        }

        return await bookRepository.GetBookByTitleAsync(title);
    }

    public async Task<bool> UpdateBookAsync(Guid id, UpdateBookDto updateBookDto)
    {
        var validationResult = await validationService.ValidateAsync(updateBookDto);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException($"Validation failed: {validationResult.ErrorMessage}");
        }

        // Check if book exists
        var existingBook = await bookRepository.GetBookByIdAsync(id);
        if (existingBook == null)
        {
            throw new InvalidOperationException($"Book with ID '{id}' not found.");
        }

        // Check if new title conflicts with existing book
        if (updateBookDto.Title != existingBook.Title &&
            await bookRepository.BookExistsAsync(updateBookDto.Title))
        {
            throw new InvalidOperationException($"A book with title '{updateBookDto.Title}' already exists.");
        }

        return await bookRepository.UpdateBookAsync(id, updateBookDto);
    }

    public async Task<bool> UpdateBookByTitleAsync(string title, UpdateBookDto updateBookDto)
    {
        var validationResult = await validationService.ValidateAsync(updateBookDto);
        if (!validationResult.IsValid)
        {
            throw new ArgumentException($"Validation failed: {validationResult.ErrorMessage}");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        }

        // Check if book exists
        var existingBook = await bookRepository.GetBookByTitleAsync(title);
        if (existingBook == null)
        {
            throw new InvalidOperationException($"Book with title '{title}' not found.");
        }

        // Check if new title conflicts with existing book
        if (updateBookDto.Title != title &&
            await bookRepository.BookExistsAsync(updateBookDto.Title))
        {
            throw new InvalidOperationException($"A book with title '{updateBookDto.Title}' already exists.");
        }

        return await bookRepository.UpdateBookAsync(existingBook.Id, updateBookDto);
    }

    public async Task<bool> DeleteBookAsync(Guid id)
    {
        var existingBook = await bookRepository.GetBookByIdAsync(id);
        if (existingBook == null)
        {
            throw new InvalidOperationException($"Book with ID '{id}' not found.");
        }

        return await bookRepository.DeleteBookAsync(id);
    }

    public async Task<bool> DeleteBookByTitleAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty", nameof(title));
        }

        var existingBook = await bookRepository.GetBookByTitleAsync(title);
        if (existingBook == null)
        {
            throw new InvalidOperationException($"Book with title '{title}' not found.");
        }

        return await bookRepository.DeleteBookByTitleAsync(title);
    }

    public async Task<bool> BookExistsAsync(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return false;
        }

        return await bookRepository.BookExistsAsync(title);
    }
}
