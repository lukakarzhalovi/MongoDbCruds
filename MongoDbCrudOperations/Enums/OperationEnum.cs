namespace MongoDbCrudOperations.Enums;

public enum OperationEnum
{
    CreateBook = 1,
    GetAllBooks = 2,
    GetBookByTitle = 3,
    GetBookById = 4,
    UpdateBookByTitle = 5,
    UpdateBookById = 6,
    DeleteBookByTitle = 7,
    DeleteBookById = 8,
    CheckBookExists = 9,
    Exit = 0,
}