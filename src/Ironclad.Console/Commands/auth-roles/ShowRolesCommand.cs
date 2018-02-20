// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal static class ShowRolesCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options)
        {
            // description
            app.Description = "Lists the roles.";
            app.HelpOption();

            // arguments
            var argument = app.Argument("role", "The role. You can end the role with a wildcard.", false);

            // options
            var optionSkip = app.Option("-s|--skip", "The number of roles to skip.", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", "The number of roles to take.", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argument.Value) && !argument.Value.EndsWith('*'))
                    {
                        options.Command = new ExistsCommand { RoleName = argument.Value };
                        return;
                    }

                    var startsWith = argument.Value?.TrimEnd('*') ?? argument.Value;

                    var skip = 0;
                    if (optionSkip.HasValue() && !int.TryParse(optionSkip.Value(), out skip))
                    {
                        throw new CommandParsingException(app, $"Unable to parse [skip] value of '{optionSkip.Value()}'");
                    }

                    var take = 20;
                    if (optionTake.HasValue() && !int.TryParse(optionTake.Value(), out take))
                    {
                        throw new CommandParsingException(app, $"Unable to parse [take] value of '{optionTake.Value()}'");
                    }

                    options.Command = new ListCommand<string>(
                        "roles",
                        async context => await context.RolesClient.GetRolesAsync(startsWith, skip, take).ConfigureAwait(false),
                        ("role", role => role));
                });
        }

        private class ExistsCommand : ICommand
        {
            public string RoleName { get; set; }

            public async Task ExecuteAsync(CommandContext context)
            {
                var exists = await context.RolesClient.RoleExistsAsync(this.RoleName).ConfigureAwait(false);
                context.Reporter.Output(exists ? this.RoleName : $"Role '{this.RoleName}' not found");
            }
        }
    }
}
