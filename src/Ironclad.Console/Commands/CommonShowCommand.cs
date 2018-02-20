// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System.Globalization;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;

    internal static class CommonShowCommand
    {
        public static void Configure(CommandLineApplication app, CommandLineOptions options, IShowTraits traits)
        {
            // description
            app.Description = $"Lists the {traits.Name}s.";
            app.HelpOption();

            // arguments
            var argument = app.Argument(traits.ArgumentName, traits.ArgumentDescription /* You can use wildcards for searching."*/, false);

            // options
            var optionSkip = app.Option("-s|--skip", $"The number of {traits.Name}s to skip.", CommandOptionType.SingleValue);
            var optionTake = app.Option("-t|--take", $"The number of {traits.Name}s to take.", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (!string.IsNullOrEmpty(argument.Value) && !argument.Value.EndsWith('*'))
                    {
                        options.Command = traits.GetShowCommand(argument.Value);

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

                    options.Command = traits.GetListCommand(startsWith, skip, take);
                });
        }
    }
}