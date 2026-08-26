using FluentValidation;

namespace Scheduler.API.Models.Preference
{
    public class UpsertServiceProviderAttributeRequest
    {
        public Guid? Id { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string AttributeType { get; set; }
        public string? AttributeValue { get; set; }
        public int? AttributeItemId { get; set; }
    }

    public class UpsertServiceProviderAttributeRequestValidator : AbstractValidator<UpsertServiceProviderAttributeRequest>
    {
        public UpsertServiceProviderAttributeRequestValidator()
        {
            RuleFor(x => x.ServiceProviderId)
                .NotEmpty().WithMessage("ServiceProviderId is required.");

            RuleFor(x => x.AttributeType)
                .NotEmpty().WithMessage("AttributeType is required.")
                .MaximumLength(100).WithMessage("AttributeType cannot exceed 100 characters.");

            RuleFor(x => x.AttributeValue)
                .MaximumLength(200).WithMessage("AttributeValue cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.AttributeValue));

            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.AttributeValue) || x.AttributeItemId.HasValue)
                .WithMessage("Either AttributeValue or AttributeItemId must be provided.");
        }
    }
}

