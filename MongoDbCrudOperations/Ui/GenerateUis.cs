using MongoDbCrudOperations.Entities;

namespace MongoDbCrudOperations.Ui;

public interface IUiService
{
    Task DisplayMainMenuAsync();
    Task<string> GetUserInputAsync(string prompt);
    Task DisplayBooksAsync(List<Book> books);
    Task DisplayBookAsync(Book? book);
    Task DisplayMessageAsync(string message, bool isError = false);
    Task DisplaySuccessMessageAsync(string message);
    Task ClearScreenAsync();
    Task WaitForUserInputAsync();
    Task DisplayCreateBookInstructionsAsync();
    Task DisplayUpdateBookInstructionsAsync();
    Task DisplayGetBookInstructionsAsync();
    Task DisplayGetBookByIdInstructionsAsync();
    Task DisplayDeleteBookInstructionsAsync();
    Task DisplayDeleteBookByIdInstructionsAsync();
    Task DisplayCheckBookExistsInstructionsAsync();
}

public class UiService() : IUiService
{
    public async Task DisplayMainMenuAsync()
    {
        await ClearScreenAsync();
        Console.WriteLine("=== MongoDB Book Management System ===");
        Console.WriteLine();
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1 -> Create a book");
        Console.WriteLine("2 -> Get all books");
        Console.WriteLine("3 -> Get a book by title");
        Console.WriteLine("4 -> Get a book by ID");
        Console.WriteLine("5 -> Update a book by title");
        Console.WriteLine("6 -> Update a book by ID");
        Console.WriteLine("7 -> Delete a book by title");
        Console.WriteLine("8 -> Delete a book by ID");
        Console.WriteLine("9 -> Check if book exists");
        Console.WriteLine("0 -> Exit");
        Console.WriteLine();
        Console.Write("Enter your choice: ");
    }

    public async Task<string> GetUserInputAsync(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(input))
        {
            await DisplayMessageAsync("Input cannot be empty. Please try again.", true);
            return await GetUserInputAsync(prompt);
        }

        return input.Trim();
    }

    public async Task DisplayBooksAsync(List<Book> books)
    {
        if (books.Count == 0)
        {
            await DisplayMessageAsync("No books found.");
            return;
        }

        Console.WriteLine($"\n=== Found {books.Count} book(s) ===");
        Console.WriteLine(new string('-', 80));
        
        for (var i = 0; i < books.Count; i++)
        {
            Console.WriteLine($"Book {i + 1}:");
            Console.WriteLine(books[i].ToString());
            Console.WriteLine(new string('-', 80));
        }
    }

    public async Task DisplayBookAsync(Book? book)
    {
        if (book == null)
        {
            await DisplayMessageAsync("Book not found.");
            return;
        }

        Console.WriteLine("\n=== Book Details ===");
        Console.WriteLine(book.ToString());
    }

    public Task DisplayMessageAsync(string message, bool isError = false)
    {
        var color = isError ? ConsoleColor.Red : ConsoleColor.Yellow;
        var prefix = isError ? "ERROR: " : "INFO: ";
        
        Console.ForegroundColor = color;
        Console.WriteLine($"\n{prefix}{message}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

    public Task DisplaySuccessMessageAsync(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nSUCCESS: {message}");
        Console.ResetColor();
        return Task.CompletedTask;
    }

    public Task ClearScreenAsync()
    {
        Console.Clear();
        return Task.CompletedTask;
    }

    public Task WaitForUserInputAsync()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        return Task.CompletedTask;
    }

    public Task DisplayCreateBookInstructionsAsync()
    {
        Console.WriteLine("\n=== Create New Book ===");
        Console.WriteLine("Enter book details in the following format:");
        Console.WriteLine("Title, Author, Description, PageCount, PublishDate (yyyy-mm-dd)");
        Console.WriteLine("Example: The Great Gatsby, F. Scott Fitzgerald, A classic novel, 180, 1925-04-10");
        Console.WriteLine();
        return Task.CompletedTask;
    }

    public Task DisplayUpdateBookInstructionsAsync()
    {
        Console.WriteLine("\n=== Update Book ===");
        Console.WriteLine("Enter book details in the following format:");
        Console.WriteLine("Title, PageCount (optional), PublishDate (optional), Author (optional), Description (optional)");
        Console.WriteLine("Example: The Great Gatsby, 200, 1925-04-10, F. Scott Fitzgerald, Updated description");
        Console.WriteLine("Note: At least Title is required. Other fields are optional.");
        Console.WriteLine();
        return Task.CompletedTask;
    }

    public Task DisplayGetBookInstructionsAsync()
    {
        Console.WriteLine("\n=== Get Book ===");
        Console.WriteLine("Enter the book title:");
        return Task.CompletedTask;
    }

    public Task DisplayGetBookByIdInstructionsAsync()
    {
        Console.WriteLine("\n=== Get Book by ID ===");
        Console.WriteLine("Enter the book ID (GUID format):");
        return Task.CompletedTask;
    }

    public Task DisplayDeleteBookInstructionsAsync()
    {
        Console.WriteLine("\n=== Delete Book ===");
        Console.WriteLine("Enter the book title to delete:");
        return Task.CompletedTask;
    }

    public Task DisplayDeleteBookByIdInstructionsAsync()
    {
        Console.WriteLine("\n=== Delete Book by ID ===");
        Console.WriteLine("Enter the book ID (GUID format) to delete:");
        return Task.CompletedTask;
    }

    public Task DisplayCheckBookExistsInstructionsAsync()
    {
        Console.WriteLine("\n=== Check if Book Exists ===");
        Console.WriteLine("Enter the book title to check:");
        return Task.CompletedTask;
    }
}