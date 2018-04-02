// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;

    public class AddApiResourceCommand : ICommand
    {
        private ApiResource resource;

        private AddApiResourceCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = $"Creates a new API";
            app.ExtendedHelpText = $"{Environment.NewLine}Use 'apis add -i' to enter interactive mode{Environment.NewLine}To add user claims to scopes please use interactive mode{Environment.NewLine}";

            // arguments
            var argumentName = app.Argument("name", "The API name", false);
            var argumentApiSecret = app.Argument("secret", "The API secret", false);

            // options
#pragma warning disable SA1025
            var optionDisplayName = app.Option("-d|--description <description>",     "The API description",                                                CommandOptionType.SingleValue);
            var optionUserClaims =  app.Option("-c|--claim <claim>",                 "A user claim required by the API (you can call this several times)", CommandOptionType.MultipleValue);
            var optionApiScopes =   app.Option("-a|--scope <scope>",                 "A scope associated with this API (you can call this several times)", CommandOptionType.MultipleValue);
            var optionDisabled =    app.Option("-x|--disabled",                      "Creates the new API in a disabled state",                            CommandOptionType.NoValue);
            var optionInteractive = app.Option("-i|--interactive",                   "Enters interactive mode",                                            CommandOptionType.NoValue);
#pragma warning restore SA1025

            app.HelpOption();

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if ((string.IsNullOrEmpty(argumentName.Value) || string.IsNullOrEmpty(argumentApiSecret.Value)) && !optionInteractive.HasValue())
                    {
                        app.ShowVersionAndHelp();
                        return;
                    }

                    var reporter = new ConsoleReporter(console, options.Verbose.HasValue(), false);
                    var helper = new ApiResourceHelper();

                    var resource = new ApiResource
                    {
                        Name = argumentName.Value,
                        ApiSecret = argumentApiSecret.Value,
                        DisplayName = optionDisplayName.Value(),
                        UserClaims = optionUserClaims.HasValue() ? optionUserClaims.Values.Distinct().ToHashSet() : null,
                        ApiScopes = optionApiScopes.HasValue() ? optionApiScopes.Values.Select(name => new ApiResource.Scope { Name = name }).ToHashSet() : null,
                        Enabled = optionDisabled.HasValue() ? (bool?)(!(optionDisabled.Value() == "on")) : null,
                    };

                    reporter.Verbose("Prototype API (from command line arguments):");
                    reporter.Verbose(JsonConvert.SerializeObject(resource));

                    if (!helper.IsValid(resource) || optionInteractive.HasValue())
                    {
                        try
                        {
                            resource = helper.GetValid(resource);
                        }
                        catch (NotSupportedException ex)
                        {
                            throw new CommandParsingException(app, $"Operation Aborted. {ex.Message}", ex);
                        }

                        reporter.Verbose("Validated API (from interactive console):");
                        reporter.Verbose(JsonConvert.SerializeObject(resource));
                    }

                    options.Command = new AddApiResourceCommand { resource = resource };
                });
        }

        public async Task ExecuteAsync(CommandContext context) => await context.ApiResourcesClient.AddApiResourceAsync(this.resource).ConfigureAwait(false);

        private static string Safe(string value, string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NotSupportedException(errorMessage);
            }

            return value;
        }

        private static IEnumerable<ApiResource.Scope> GetScopes(IEnumerable<ApiResource.Scope> scopes)
        {
            foreach (var scope in scopes ?? Array.Empty<ApiResource.Scope>())
            {
                scope.Name = Prompt.GetString("Scope name:", scope.Name);
                scope.UserClaims = Prompt.GetString(
                    $"User claims for the '{scope.Name}' API scope (space separated) [optional]:",
                    scope.UserClaims == null ? null : string.Join(' ', scope.UserClaims))
                    ?.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                yield return scope;
            }

            var newScope = new ApiResource.Scope { Name = Prompt.GetString("Scope name [optional]:") };
            while (!string.IsNullOrWhiteSpace(newScope.Name))
            {
                newScope.UserClaims = Prompt.GetString("User claims for the API scope (space separated) [optional]:")?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                yield return newScope;

                newScope = new ApiResource.Scope { Name = Prompt.GetString("Scope name [optional]:") };
            }
        }

        private class ApiResourceHelper
        {
            public ApiResource GetPrototype(ApiResource resource) => resource;

            public bool IsValid(ApiResource resource) => !string.IsNullOrEmpty(resource.Name) && !string.IsNullOrEmpty(resource.ApiSecret);

            public ApiResource GetValid(ApiResource resource)
            {
                resource.Name = Safe(Prompt.GetString("API name:", resource.Name), "Cannot create an API without a name.");
                resource.DisplayName = Prompt.GetString("API description:", resource.DisplayName);
                resource.ApiSecret = resource.ApiSecret ?? Safe(Prompt.GetPassword("API secret:"), "Cannot create an API without an introspection secret.");
                resource.UserClaims = Prompt.GetString(
                    "User claims for the API (space separated) [optional]:",
                    resource.UserClaims == null ? null : string.Join(' ', resource.UserClaims))
                    ?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                resource.ApiScopes = GetScopes(resource.ApiScopes).ToHashSet();

                // defaults
                resource.DisplayName = string.IsNullOrWhiteSpace(resource.DisplayName) ? null : resource.DisplayName;

                return resource;
            }
        }
    }
}
