using MongoDbCrudOperations.Entities;

namespace MongoDbCrudOperations.Ui;

public interface IUiService
{
    void DisplayMainMenu();
    string GetUserInput(string prompt);
    void DisplayBooks(List<Book> books);
    void DisplayBook(Book? book);
    void DisplayMessage(string message, bool isError = false);
    void DisplaySuccess(string message);
    void DisplayInstructions(string operation, string format, string example);
    void WaitForKey();
    void ClearScreen();
}

public class UiService : IUiService
{
    public void DisplayMainMenu()
    {
        ClearScreen();
        Console.WriteLine("=== MongoDB Book Management System ===");
        Console.WriteLine();
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
    }

    public string GetUserInput(string prompt)
    {
        Console.Write(prompt);
        var input = Console.ReadLine();
        
        while (string.IsNullOrWhiteSpace(input))
        {
            DisplayMessage("Input cannot be empty. Please try again.", true);
            Console.Write(prompt);
            input = Console.ReadLine();
        }
        
        return input.Trim();
    }

    public void DisplayBooks(List<Book> books)
    {
        if (books.Count == 0)
        {
            DisplayMessage("No books found.");
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

    public void DisplayBook(Book? book)
    {
        if (book == null)
        {
            DisplayMessage("Book not found.");
            return;
        }

        Console.WriteLine("\n=== Book Details ===");
        Console.WriteLine(book.ToString());
    }

    public void DisplayMessage(string message, bool isError = false)
    {
        var color = isError ? ConsoleColor.Red : ConsoleColor.Yellow;
        var prefix = isError ? "ERROR: " : "INFO: ";
        
        Console.ForegroundColor = color;
        Console.WriteLine($"\n{prefix}{message}");
        Console.ResetColor();
    }

    public void DisplaySuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nSUCCESS: {message}");
        Console.ResetColor();
    }

    public void DisplayInstructions(string operation, string format, string example)
    {
        Console.WriteLine($"\n=== {operation} ===");
        Console.WriteLine($"Format: {format}");
        Console.WriteLine($"Example: {example}");
        Console.WriteLine();
    }

    public void WaitForKey()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public void ClearScreen()
    {
        Console.Clear();
    }
}