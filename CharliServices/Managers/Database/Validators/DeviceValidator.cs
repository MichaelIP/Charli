using FluentValidation;
using McpNetwork.Charli.Server.Models.Entities;

namespace McpNetwork.Charli.Managers.DatabaseManager.Validators
{
    internal class DeviceValidator : AbstractValidator<Device>
    {
        public DeviceValidator()
        {
            RuleFor(d => d.UserId).NotNull().WithMessage("User Id should not be null");
            RuleFor(d => d.Platform).NotEmpty().WithMessage("Platform is mandatory");
            RuleFor(d => d.Platform).MaximumLength(255).WithMessage("Platform cannot exceed 255 caracters long");
        }
    }
}
