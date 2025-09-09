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
    ILogger<ApplicationService> logger)
    : IApplicationService
{
    public async Task RunAsync()
    {
        logger.LogInformation("Starting MongoDB Book Management System");

        while (true)
        {
            try
            {
                await uiService.DisplayMainMenuAsync();
                var input = await uiService.GetUserInputAsync(string.Empty);

                if (!int.TryParse(input, out var choice))
                {
                    await uiService.DisplayMessageAsync("Invalid input. Please enter a number.", true);
                    await uiService.WaitForUserInputAsync();
                    continue;
                }

                var operation = (OperationEnum)choice;

                switch (operation)
                {
                    case OperationEnum.CreateBook:
                        await HandleCreateBookAsync();
                        break;
                    case OperationEnum.GetAllBooks:
                        await HandleGetAllBooksAsync();
                        break;
                    case OperationEnum.GetBookByTitle:
                        await HandleGetBookByTitleAsync();
                        break;
                    case OperationEnum.GetBookById:
                        await HandleGetBookByIdAsync();
                        break;
                    case OperationEnum.UpdateBookByTitle:
                        await HandleUpdateBookByTitleAsync();
                        break;
                    case OperationEnum.UpdateBookById:
                        await HandleUpdateBookByIdAsync();
                        break;
                    case OperationEnum.DeleteBookByTitle:
                        await HandleDeleteBookByTitleAsync();
                        break;
                    case OperationEnum.DeleteBookById:
                        await HandleDeleteBookByIdAsync();
                        break;
                    case OperationEnum.CheckBookExists:
                        await HandleCheckBookExistsAsync();
                        break;
                    case OperationEnum.Exit:
                        logger.LogInformation("Exiting application");
                    return;
                    default:
                        await uiService.DisplayMessageAsync("Invalid choice. Please try again.", true);
                        break;
                }

                if (operation != OperationEnum.Exit)
                {
                    await uiService.WaitForUserInputAsync();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in the application");
                await uiService.DisplayMessageAsync($"An error occurred: {ex.Message}", true);
                await uiService.WaitForUserInputAsync();
            }
        }
    }

    private async Task HandleCreateBookAsync()
    {
        try
        {
            await uiService.DisplayCreateBookInstructionsAsync();
            var input = await uiService.GetUserInputAsync("Enter book details: ");
            
            var createBookDto = await mapper.MapToCreateBookDtoAsync(input.Split(", "));
            var book = await bookService.CreateBookAsync(createBookDto);
            
            await uiService.DisplaySuccessMessageAsync($"Book created successfully with ID: {book.Id}");
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to create book: {ex.Message}", true);
        }
    }

    private async Task HandleGetAllBooksAsync()
    {
        try
        {
            var books = await bookService.GetAllBooksAsync();
            await uiService.DisplayBooksAsync(books);
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to retrieve books: {ex.Message}", true);
        }
    }

    private async Task HandleGetBookByTitleAsync()
    {
        try
        {
            await uiService.DisplayGetBookInstructionsAsync();
            var title = await uiService.GetUserInputAsync("Enter book title: ");
            
            var book = await bookService.GetBookByTitleAsync(title);
            await uiService.DisplayBookAsync(book);
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to retrieve book: {ex.Message}", true);
        }
    }

    private async Task HandleGetBookByIdAsync()
    {
        try
        {
            await uiService.DisplayGetBookByIdInstructionsAsync();
            var idInput = await uiService.GetUserInputAsync("Enter book ID: ");
            
            var id = await mapper.MapToGuidAsync(idInput);
            var book = await bookService.GetBookByIdAsync(id);
            await uiService.DisplayBookAsync(book);
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to retrieve book: {ex.Message}", true);
        }
    }

    private async Task HandleUpdateBookByTitleAsync()
    {
        try
        {
            await uiService.DisplayUpdateBookInstructionsAsync();
            var input = await uiService.GetUserInputAsync("Enter book details: ");
            
            var updateBookDto = await mapper.MapToUpdateBookDtoAsync(input.Split(", "));
            var success = await bookService.UpdateBookByTitleAsync(updateBookDto.Title, updateBookDto);
            
            if (success)
            {
                await uiService.DisplaySuccessMessageAsync("Book updated successfully");
            }
            else
            {
                await uiService.DisplayMessageAsync("Failed to update book", true);
            }
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to update book: {ex.Message}", true);
        }
    }

    private async Task HandleUpdateBookByIdAsync()
    {
        try
        {
            await uiService.DisplayGetBookByIdInstructionsAsync();
            var idInput = await uiService.GetUserInputAsync("Enter book ID: ");
            var id = await mapper.MapToGuidAsync(idInput);
            
            await uiService.DisplayUpdateBookInstructionsAsync();
            var input = await uiService.GetUserInputAsync("Enter book details: ");
            
            var updateBookDto = await mapper.MapToUpdateBookDtoAsync(input.Split(", "));
            var success = await bookService.UpdateBookAsync(id, updateBookDto);
            
            if (success)
            {
                await uiService.DisplaySuccessMessageAsync("Book updated successfully");
            }
            else
            {
                await uiService.DisplayMessageAsync("Failed to update book", true);
            }
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to update book: {ex.Message}", true);
        }
    }

    private async Task HandleDeleteBookByTitleAsync()
    {
        try
        {
            await uiService.DisplayDeleteBookInstructionsAsync();
            var title = await uiService.GetUserInputAsync("Enter book title: ");
            
            var success = await bookService.DeleteBookByTitleAsync(title);
            
            if (success)
            {
                await uiService.DisplaySuccessMessageAsync("Book deleted successfully");
            }
            else
            {
                await uiService.DisplayMessageAsync("Failed to delete book", true);
            }
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to delete book: {ex.Message}", true);
        }
    }

    private async Task HandleDeleteBookByIdAsync()
    {
        try
        {
            await uiService.DisplayDeleteBookByIdInstructionsAsync();
            var idInput = await uiService.GetUserInputAsync("Enter book ID: ");
            var id = await mapper.MapToGuidAsync(idInput);
            
            var success = await bookService.DeleteBookAsync(id);
            
            if (success)
            {
                await uiService.DisplaySuccessMessageAsync("Book deleted successfully");
            }
            else
            {
                await uiService.DisplayMessageAsync("Failed to delete book", true);
            }
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to delete book: {ex.Message}", true);
        }
    }

    private async Task HandleCheckBookExistsAsync()
    {
        try
        {
            await uiService.DisplayCheckBookExistsInstructionsAsync();
            var title = await uiService.GetUserInputAsync("Enter book title: ");
            
            var exists = await bookService.BookExistsAsync(title);
            
            if (exists)
            {
                await uiService.DisplaySuccessMessageAsync($"Book '{title}' exists in the database");
            }
            else
            {
                await uiService.DisplayMessageAsync($"Book '{title}' does not exist in the database");
            }
        }
        catch (Exception ex)
        {
            await uiService.DisplayMessageAsync($"Failed to check book existence: {ex.Message}", true);
        }
    }
}