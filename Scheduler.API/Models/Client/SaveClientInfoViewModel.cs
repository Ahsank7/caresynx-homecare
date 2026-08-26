using FluentValidation;

namespace Scheduler.API.Models.Client
{
    public class SaveClientInfoViewModel
    {
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        public string? LastName { get; set; }
        public string? Alias { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNo { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? PassportNo { get; set; }
        public string? IdentityNo { get; set; }
        public int? EthnicityId { get; set; }
        public string? PasswordHash { get; set; }
        public int Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? Notes { get; set; }
        public string? UserNo { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? AddressLine3 { get; set; }
        public int AddressId  { get; set; }
        public int? CountyId     { get; set; }
        public int? MaritalStatusId { get; set; }
        public int? StateId      { get; set; }
        public int? CountryId    { get; set; }
        public float Latitude     { get; set; }
        public float Longitude { get; set; }
        public int? TitleId { get; set; }
        public int? GenderId { get; set; }
        public int? NationalityId { get; set; }
        public Guid FranchiseId { get; set; }
    }

    public class SaveClientInfoViewModelValidator : AbstractValidator<SaveClientInfoViewModel>
    {
        public SaveClientInfoViewModelValidator()
        {
            RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("FirstName is required.");

            RuleFor(user => user.LastName)
              .NotEmpty().WithMessage("LastName is required.");

            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage("UserName is required.");

            RuleFor(user => user.IdentityNo)
               .NotEmpty().WithMessage("IdentityNo is required.");

            RuleFor(user => user.PassportNo)
               .NotEmpty().WithMessage("PassportNo is required.");

            RuleFor(user => user.UserName)
               .NotEmpty().WithMessage("UserName is required.");

            RuleFor(user => user.PhoneNo)
              .NotEmpty().WithMessage("PhoneNo is required.");

            RuleFor(user => user.MobileNo)
             .NotEmpty().WithMessage("MobileNo is required.");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(user => user.Age)
                .GreaterThan(0).WithMessage("Age must be greater than 0.")
                .LessThan(100).WithMessage("Age must be less than 100.");

            RuleFor(user => user.TitleId)
           .GreaterThan(0).WithMessage("TitleId must be provided.");

            RuleFor(user => user.GenderId)
              .GreaterThan(0).WithMessage("GenderId must be provided.");

            RuleFor(user => user.NationalityId)
            .GreaterThan(0).WithMessage("NationalityId must be provided.");

            RuleFor(user => user.BirthDate)
            .NotEmpty().WithMessage("BirthDate is required.");

            RuleFor(user => user.JoiningDate)
           .NotEmpty().WithMessage("JoiningDate is required.");

           RuleFor(user => user.AddressLine1)
          .NotEmpty().WithMessage("AddressLine1 is required.");

            RuleFor(user => user.CountyId)
           .GreaterThan(0).WithMessage("CountyId must be provided.");

            RuleFor(user => user.CountryId)
         .GreaterThan(0).WithMessage("CountryId must be provided.");

            RuleFor(user => user.StateId)
         .GreaterThan(0).WithMessage("StateId must be provided.");

        }
    }
}
