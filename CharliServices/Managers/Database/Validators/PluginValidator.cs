using FluentValidation;
using McpNetwork.Charli.Server.Models.Entities;

namespace McpNetwork.Charli.Managers.DatabaseManager.Validators
{
    internal class PluginValidator : AbstractValidator<Plugin>
    {
        public PluginValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is mandatory");
            RuleFor(p => p.Name).MaximumLength(255).WithMessage("Name cannot exceed 255 characters long");
            RuleFor(p => p.FileName).NotEmpty().WithMessage("FileName is mandatory");
            RuleFor(p => p.FileName).MaximumLength(255).WithMessage("FileName cannot exceed 255 characters long");
            RuleFor(p => p.Version).NotEmpty().WithMessage("Version is mandatory");
            RuleFor(p => p.Version).MaximumLength(255).WithMessage("Version cannot exceed 255 characters long");
        }
    }
}
