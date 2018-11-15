// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    internal class ModifyScopeCommand : ICommand
    {
        private IdentityResource scope;

        private ModifyScopeCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Modify an identity-based scope";
            app.HelpOption();

            // arguments
            var argumentName = app.Argument("name", "The name of the identity-based scope", false);

#pragma warning disable SA1025
            var optionDisplayName =                 app.Option("-n|--display_name <name>",         "The display name of the identity-based scope",                                                    CommandOptionType.SingleValue);
            var optionUserClaims =                  app.Option("-u|--user_claims <user_claim>",    "The user claim types associated with the identity-based scope (you can call this several times)", CommandOptionType.MultipleValue);
            var optionInteractive =                 app.Option("-i|--interactive",                 "Enters interactive mode",                                                                         CommandOptionType.NoValue);
#pragma warning restore SA1025

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);

                    if (string.IsNullOrEmpty(argumentName.Value))
                    {
                        reporter.Warn("The name of the scope is required.");
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var helper = new IdentityResourceHelper();

                    var scope = helper.GetPrototype(
                        new IdentityResource
                        {
                            Name = argumentName.Value,
                            DisplayName = optionDisplayName.HasValue() ? optionDisplayName.Value() : null,
                            UserClaims =
                                optionUserClaims.HasValue() ? optionUserClaims.Values.Distinct().ToHashSet() : null
                        });

                    reporter.Verbose("Prototype scope (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(scope));

                    if (!helper.IsValid(scope) || optionInteractive.HasValue())
                    {
                        try
                        {
                            scope = helper.GetValid(scope);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated scope (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(scope));
                    }

                    options.Command = new ModifyScopeCommand
                    {
                        scope = scope
                    };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.IdentityResourcesClient.ModifyIdentityResourceAsync(this.scope).ConfigureAwait(false);

        private static string Safe(string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException(errorMessage);
            }

            return value;
        }

        private class IdentityResourceHelper : IHelper<IdentityResource>
        {
            public IdentityResource GetPrototype(IdentityResource scope) => scope;

            public bool IsValid(IdentityResource scope) =>
                !string.IsNullOrEmpty(scope.Name) &&
                !string.IsNullOrEmpty(scope.DisplayName) &&
                scope.UserClaims?.Any() == true;

            public IdentityResource GetValid(IdentityResource scope)
            {
                scope.DisplayName = Prompt.GetString("Scope display name:", scope.DisplayName);
                scope.UserClaims = Safe(
                        Prompt.GetString(
                            "Claim types supported by this scope (space separated):",
                            scope.UserClaims == null ? null : string.Join(' ', scope.UserClaims)),
                        "Cannot modify a scope without any claim types.")
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries);

                return scope;
            }
        }
    }
}