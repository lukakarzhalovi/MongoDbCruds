using MongoDbCrudOperations.Enums;
using MongoDbCrudOperations.Mapper;
using MongoDbCrudOperations.Services;
using MongoDbCrudOperations.Ui;
using Microsoft.Extensions.Logging;

namespace MongoDbCrudOperations.Application;

public interface IApplicationService
{
    Task RunAsync();
}

public class ApplicationService(
    IBookService bookService,
    IMapper mapper,
    IUiService uiService,
    ILogger<ApplicationService> logger) : IApplicationService
{
    public async Task RunAsync()
    {
        logger.LogInformation("Starting MongoDB Book Management System");

        while (true)
        {
            try
            {
                uiService.DisplayMainMenu();
                var input = uiService.GetUserInput("Enter your choice: ");

                if (!int.TryParse(input, out var choice))
                {
                    uiService.DisplayMessage("Invalid input. Please enter a number.", true);
                    uiService.WaitForKey();
                    continue;
                }

                var operation = (OperationEnum)choice;

                switch (operation)
                {
                    case OperationEnum.CreateBook:
                        await HandleCreateBook();
                        break;
                    case OperationEnum.GetAllBooks:
                        await HandleGetAllBooks();
                        break;
                    case OperationEnum.GetBookByTitle:
                        await HandleGetBookByTitle();
                        break;
                    case OperationEnum.GetBookById:
                        await HandleGetBookById();
                        break;
                    case OperationEnum.UpdateBookByTitle:
                        await HandleUpdateBookByTitle();
                        break;
                    case OperationEnum.UpdateBookById:
                        await HandleUpdateBookById();
                        break;
                    case OperationEnum.DeleteBookByTitle:
                        await HandleDeleteBookByTitle();
                        break;
                    case OperationEnum.DeleteBookById:
                        await HandleDeleteBookById();
                        break;
                    case OperationEnum.CheckBookExists:
                        await HandleCheckBookExists();
                        break;
                    case OperationEnum.Exit:
                        logger.LogInformation("Exiting application");
                        return;
                    default:
                        uiService.DisplayMessage("Invalid choice. Please try again.", true);
                        break;
                }

                if (operation != OperationEnum.Exit)
                {
                    uiService.WaitForKey();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in the application");
                uiService.DisplayMessage($"An error occurred: {ex.Message}", true);
                uiService.WaitForKey();
            }
        }
    }

    private async Task HandleCreateBook()
    {
        try
        {
            uiService.DisplayInstructions(
                "Create New Book",
                "Title, Author, Description, PageCount, PublishDate (yyyy-mm-dd)",
                "The Great Gatsby, F. Scott Fitzgerald, A classic novel, 180, 1925-04-10"
            );
            
            var input = uiService.GetUserInput("Enter book details: ");
            var createBookDto = await mapper.MapToCreateBookDtoAsync(input.Split(", "));
            var book = await bookService.CreateBookAsync(createBookDto);
            uiService.DisplaySuccess($"Book created successfully with ID: {book.Id}");
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to create book: {ex.Message}", true);
        }
    }

    private async Task HandleGetAllBooks()
    {
        try
        {
            var books = await bookService.GetAllBooksAsync();
            uiService.DisplayBooks(books);
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to retrieve books: {ex.Message}", true);
        }
    }

    private async Task HandleGetBookByTitle()
    {
        try
        {
            uiService.DisplayInstructions("Get Book", "Enter the book title", "The Great Gatsby");
            var title = uiService.GetUserInput("Enter book title: ");
            var book = await bookService.GetBookByTitleAsync(title);
            uiService.DisplayBook(book);
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to retrieve book: {ex.Message}", true);
        }
    }

    private async Task HandleGetBookById()
    {
        try
        {
            uiService.DisplayInstructions("Get Book by ID", "Enter the book ID (GUID format)", "12345678-1234-1234-1234-123456789012");
            var idInput = uiService.GetUserInput("Enter book ID: ");
            var id = await mapper.MapToGuidAsync(idInput);
            var book = await bookService.GetBookByIdAsync(id);
            uiService.DisplayBook(book);
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to retrieve book: {ex.Message}", true);
        }
    }

    private async Task HandleUpdateBookByTitle()
    {
        try
        {
            uiService.DisplayInstructions(
                "Update Book by Title",
                "Title, PageCount (optional), PublishDate (optional), Author (optional), Description (optional)",
                "The Great Gatsby, 200, 1925-04-10, F. Scott Fitzgerald, Updated description"
            );
            
            var input = uiService.GetUserInput("Enter book details: ");
            var updateBookDto = await mapper.MapToUpdateBookDtoAsync(input.Split(", "));
            var success = await bookService.UpdateBookByTitleAsync(updateBookDto.Title, updateBookDto);
            
            if (success)
                uiService.DisplaySuccess("Book updated successfully");
            else
                uiService.DisplayMessage("Failed to update book", true);
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to update book: {ex.Message}", true);
        }
    }

    private async Task HandleUpdateBookById()
    {
        try
        {
            uiService.DisplayInstructions("Update Book by ID", "Enter the book ID (GUID format)", "12345678-1234-1234-1234-123456789012");
            var idInput = uiService.GetUserInput("Enter book ID: ");
            var id = await mapper.MapToGuidAsync(idInput);
            
            uiService.DisplayInstructions(
                "Update Book",
                "PageCount (optional), PublishDate (optional), Author (optional), Description (optional)",
                "200, 1925-04-10, F. Scott Fitzgerald, Updated description"
            );
            
            var input = uiService.GetUserInput("Enter book details: ");
            var updateBookDto = await mapper.MapToUpdateBookDtoAsync(input.Split(", "));
            var success = await bookService.UpdateBookAsync(id, updateBookDto);
            
            if (success)
                uiService.DisplaySuccess("Book updated successfully");
            else
                uiService.DisplayMessage("Failed to update book", true);
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to update book: {ex.Message}", true);
        }
    }

    private async Task HandleDeleteBookByTitle()
    {
        try
        {
            uiService.DisplayInstructions("Delete Book", "Enter the book title to delete", "The Great Gatsby");
            var title = uiService.GetUserInput("Enter book title: ");
            var success = await bookService.DeleteBookByTitleAsync(title);
            
            if (success)
                uiService.DisplaySuccess("Book deleted successfully");
            else
                uiService.DisplayMessage("Failed to delete book", true);
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to delete book: {ex.Message}", true);
        }
    }

    private async Task HandleDeleteBookById()
    {
        try
        {
            uiService.DisplayInstructions("Delete Book by ID", "Enter the book ID (GUID format)", "12345678-1234-1234-1234-123456789012");
            var idInput = uiService.GetUserInput("Enter book ID: ");
            var id = await mapper.MapToGuidAsync(idInput);
            var success = await bookService.DeleteBookAsync(id);
            
            if (success)
                uiService.DisplaySuccess("Book deleted successfully");
            else
                uiService.DisplayMessage("Failed to delete book", true);
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to delete book: {ex.Message}", true);
        }
    }

    private async Task HandleCheckBookExists()
    {
        try
        {
            uiService.DisplayInstructions("Check if Book Exists", "Enter the book title to check", "The Great Gatsby");
            var title = uiService.GetUserInput("Enter book title: ");
            var exists = await bookService.BookExistsAsync(title);
            
            if (exists)
                uiService.DisplaySuccess($"Book '{title}' exists in the database");
            else
                uiService.DisplayMessage($"Book '{title}' does not exist in the database");
        }
        catch (Exception ex)
        {
            uiService.DisplayMessage($"Failed to check book existence: {ex.Message}", true);
        }
    }
}