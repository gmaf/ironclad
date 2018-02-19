// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands.Validators
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;
    using McMaster.Extensions.CommandLineUtils.Validation;

    public class ClientUriNameValidator : IArgumentValidator
    {
        private readonly IEnumerable<string> names;

        public ClientUriNameValidator()
        {
            this.names = typeof(Client.Client)
                .GetProperties()
                .Select(prop => prop.Name)
                .Where(name => name.ToLower(CultureInfo.CurrentCulture).Contains("uri"));
        }

        public ValidationResult GetValidationResult(CommandArgument arg, ValidationContext context)
        {
            // This validator only runs if there is a value
            if (string.IsNullOrEmpty(arg.Value))
            {
                return ValidationResult.Success;
            }

            if (this.names.Contains(arg.Value))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"'{arg.Name}' must be one of the following elements: {string.Join(", ", this.names)}");
        }
    }
}