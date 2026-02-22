using FluentValidation;
using TaskService.Application.DTOs;

namespace TaskService.Application.Validation;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .Must(title => !string.IsNullOrWhiteSpace(title)).WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.")
            .When(x => x.Description is not null);

        RuleFor(x => x.OriginalEstimatedWork)
            .GreaterThanOrEqualTo(0).WithMessage("OriginalEstimatedWork must be 0 or greater.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid value: Todo, InProgress, or Done.")
            .Must(status => Convert.ToInt32(status) != 3)
            .WithMessage("The 'Done' status is not permitted for new tasks.");
    }
}
