using FluentValidation;

namespace Scheduler.API.Models.Preference
{
    public class UpsertClientPreferenceRequest
    {
        public Guid? Id { get; set; }
        public Guid ClientId { get; set; }
        public string PreferenceType { get; set; }
        public string? PreferenceValue { get; set; }
        public int? PreferenceItemId { get; set; }
        public bool IsRequired { get; set; } = false;
    }

    public class UpsertClientPreferenceRequestValidator : AbstractValidator<UpsertClientPreferenceRequest>
    {
        public UpsertClientPreferenceRequestValidator()
        {
            RuleFor(x => x.ClientId)
                .NotEmpty().WithMessage("ClientId is required.");

            RuleFor(x => x.PreferenceType)
                .NotEmpty().WithMessage("PreferenceType is required.")
                .MaximumLength(100).WithMessage("PreferenceType cannot exceed 100 characters.");

            RuleFor(x => x.PreferenceValue)
                .MaximumLength(200).WithMessage("PreferenceValue cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.PreferenceValue));

            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.PreferenceValue) || x.PreferenceItemId.HasValue)
                .WithMessage("Either PreferenceValue or PreferenceItemId must be provided.");
        }
    }
}

