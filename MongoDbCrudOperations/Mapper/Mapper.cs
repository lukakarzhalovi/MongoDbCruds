using MongoDbCrudOperations.Dto;
using MongoDbCrudOperations.Services;
using Microsoft.Extensions.Logging;

namespace MongoDbCrudOperations.Mapper;

public interface IMapper
{
    Task<CreateBookDto> MapToCreateBookDtoAsync(string[] input);
    Task<UpdateBookDto> MapToUpdateBookDtoAsync(string[] input);
    Task<Guid> MapToGuidAsync(string input);
}

public class Mapper(IValidationService validationService, ILogger<Mapper> logger) : IMapper
{
    public async Task<CreateBookDto> MapToCreateBookDtoAsync(string[] input)
    {
        if (input == null || input.Length != 5)
        {
            throw new ArgumentException("Invalid input format. Expected: Title, Author, Description, PageCount, PublishDate");
        }

        try
        {
            var createBookDto = new CreateBookDto
            {
                Title = input[0].Trim(),
                Author = input[1].Trim(),
                Description = input[2].Trim(),
                PageCount = int.Parse(input[3].Trim()),
                PublishDate = DateTime.Parse(input[4].Trim())
            };

            var validationResult = await validationService.ValidateAsync(createBookDto);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Validation failed: {validationResult.ErrorMessage}");
            }

            return createBookDto;
        }
        catch (FormatException ex)
        {
            logger.LogError(ex, "Error parsing input: {Input}", string.Join(", ", input));
            throw new ArgumentException("Invalid format for PageCount or PublishDate", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping to CreateBookDto: {Input}", string.Join(", ", input));
            throw;
        }
    }

    public async Task<UpdateBookDto> MapToUpdateBookDtoAsync(string[] input)
    {
        if (input == null || input.Length < 1)
        {
            throw new ArgumentException("Invalid input format. At least Title is required.");
        }

        try
        {
            var updateBookDto = new UpdateBookDto
            {
                Title = input[0].Trim()
            };

            // Parse optional fields if provided
            if (input.Length > 1 && !string.IsNullOrWhiteSpace(input[1]))
            {
                if (validationService.TryParseInt(input[1].Trim(), out var pageCount))
                {
                    updateBookDto.PageCount = pageCount;
                }
            }

            if (input.Length > 2 && !string.IsNullOrWhiteSpace(input[2]))
            {
                if (validationService.TryParseDateTime(input[2].Trim(), out var publishDate))
                {
                    updateBookDto.PublishDate = publishDate;
                }
            }

            if (input.Length > 3 && !string.IsNullOrWhiteSpace(input[3]))
            {
                updateBookDto.Author = input[3].Trim();
            }

            if (input.Length > 4 && !string.IsNullOrWhiteSpace(input[4]))
            {
                updateBookDto.Description = input[4].Trim();
            }

            var validationResult = await validationService.ValidateAsync(updateBookDto);
            if (!validationResult.IsValid)
            {
                throw new ArgumentException($"Validation failed: {validationResult.ErrorMessage}");
            }

            return updateBookDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error mapping to UpdateBookDto: {Input}", string.Join(", ", input));
            throw;
        }
    }

    public Task<Guid> MapToGuidAsync(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input cannot be null or empty", nameof(input));
        }

        if (Guid.TryParse(input.Trim(), out var guid))
        {
            return Task.FromResult(guid);
        }

        throw new ArgumentException($"Invalid GUID format: {input}");
    }
}