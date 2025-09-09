using FluentValidation;
using MongoDbCrudOperations.Dto;
using MongoDbCrudOperations.Entities;

namespace MongoDbCrudOperations.Validators;

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author cannot exceed 100 characters");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Page count must be greater than 0");

        RuleFor(x => x.PublishDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Publish date cannot be in the future");
    }
}

public class CreateBookDtoValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookDtoValidator()
    {
        RuleFor(x => x.Author)
            .NotEmpty().WithMessage("Author is required")
            .MaximumLength(100).WithMessage("Author cannot exceed 100 characters");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Page count must be greater than 0");

        RuleFor(x => x.PublishDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Publish date cannot be in the future");
    }
}

public class UpdateBookDtoValidator : AbstractValidator<UpdateBookDto>
{
    public UpdateBookDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Author)
            .MaximumLength(100).WithMessage("Author cannot exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Author));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.PageCount)
            .GreaterThan(0).WithMessage("Page count must be greater than 0")
            .When(x => x.PageCount.HasValue);

        RuleFor(x => x.PublishDate)
            .LessThanOrEqualTo(DateTime.Today).WithMessage("Publish date cannot be in the future")
            .When(x => x.PublishDate.HasValue);
    }
}
