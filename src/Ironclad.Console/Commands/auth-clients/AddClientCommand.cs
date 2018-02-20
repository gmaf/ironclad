// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    internal class AddClientCommand : ICommand
    {
        private Client.Client client;

        private AddClientCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IReporter reporter)
        {
            // description
            app.Description = "Creates a new client trust relationship with the auth server";
            app.ExtendedHelpText = $"{Environment.NewLine}Use \"clients add\" without argument to enter in interactive mode.{Environment.NewLine}";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("id", "The client ID", false);
            var argumentClientSecret = app.Argument("secret", "The client secret", false);

            // options
            var optionsAccessToken = app.Option("-b|--access_tokens_via_browser", "Allow tokens access from browser.", CommandOptionType.NoValue);
            var optionsCors = app.Option("-c|--cors_uri <uri>", "An allowed CORS origin of the client. You can call this several times.", CommandOptionType.MultipleValue);
            var optionsGrants = app.Option("-g|--grant_types <type>", "A grant type of the client. You can call this several times.", CommandOptionType.MultipleValue);
            var optionsSecretRequired = app.Option("-k|--secret_required", "Set client secret required.", CommandOptionType.NoValue);
            var optionsLogouts = app.Option("-l|--logout_uri <uri>", "A logout URI of the client. You can call this several times.", CommandOptionType.MultipleValue);
            var optionsName = app.Option("-n|--name <name>", "The name of the client", CommandOptionType.SingleValue);
            var optionsOffline = app.Option("-o|--offline", "Allow offline access.", CommandOptionType.NoValue);
            var optionsPkceRequired = app.Option("-p|--pkce_requred", "Set Proof Key for Code Exchange (PKCE) required.", CommandOptionType.NoValue);
            var optionsConsentRequired = app.Option("-q|--constent_required", "Set consent required.", CommandOptionType.NoValue);
            var optionsRedirects = app.Option("-r|--redirect_uri <uri>", "A redirect URI of the client. You can call this several times.", CommandOptionType.MultipleValue);
            var optionsScopes = app.Option("-s|--scope <scope>", "An allowed scope for the client. You can call this several times.", CommandOptionType.MultipleValue);
            var optionsToken = app.Option("-t|--token <Jwt/Reference>", "The access token type of the client", CommandOptionType.SingleValue);

            // action (for this command)
            app.OnExecute(
                () =>
                {
                    if (string.IsNullOrEmpty(argumentClientId.Value))
                    {
                        options.Command = GetClientFromPrompt(reporter);
                    }
                    else if (string.IsNullOrEmpty(argumentClientSecret.Value))
                    {
                        app.ShowVersionAndHelp();
                    }
                    else
                    {
                        options.Command = new AddClientCommand
                        {
                            client = new Client.Client
                            {
                                Id = argumentClientId.Value,
                                Secret = argumentClientSecret.Value,
                                Name = optionsName.Value(),
                                AccessTokenType = optionsToken.Value(),
                                AllowedCorsOrigins = optionsCors.Values,
                                RedirectUris = optionsRedirects.Values,
                                PostLogoutRedirectUris = optionsLogouts.Values,
                                AllowedScopes = optionsScopes.Values,
                                AllowedGrantTypes = optionsGrants.Values,
                                AllowAccessTokensViaBrowser = optionsAccessToken.HasValue(),
                                AllowOfflineAccess = optionsOffline.HasValue(),
                                RequirePkce = optionsPkceRequired.HasValue(),
                                RequireClientSecret = optionsSecretRequired.HasValue(),
                                RequireConsent = optionsConsentRequired.HasValue()
                            }
                        };
                    }
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            await context.ClientsClient.AddClientAsync(this.client).ConfigureAwait(false);
        }

        private static AddClientCommand GetClientFromPrompt(IReporter reporter)
        {
            var clientDto = new Client.Client();

            clientDto.Id = Prompt.GetString("Unique Id:");
            if (string.IsNullOrEmpty(clientDto.Id))
            {
                reporter.Error("Id cannot be null");
                return null;
            }

            clientDto.Name = Prompt.GetString("Name:", clientDto.Id);
            clientDto.Secret = Prompt.GetPassword("Secret:");
            if (string.IsNullOrEmpty(clientDto.Secret))
            {
                reporter.Error("Secret cannot be null");
                return null;
            }

            clientDto.AccessTokenType = Prompt.GetString("AccessTokenType:", "Jwt");

            clientDto.AllowedCorsOrigins = PromptList("AllowedCorsOrigins", reporter);
            clientDto.RedirectUris = PromptList("RedirectUris", reporter);
            clientDto.PostLogoutRedirectUris = PromptList("PostLogoutRedirectUris", reporter);
            clientDto.AllowedScopes = PromptList("AllowedScopes", reporter);
            clientDto.AllowedGrantTypes = PromptList("AllowedGrantTypes", reporter);

            clientDto.AllowAccessTokensViaBrowser = Prompt.GetYesNo("Allow Access Token Via Browser ? ", true);
            clientDto.AllowOfflineAccess = Prompt.GetYesNo("Allow Offline Access ? ", true);
            clientDto.RequireClientSecret = Prompt.GetYesNo("Is client secret required ?", false);
            clientDto.RequirePkce = Prompt.GetYesNo("Is Proof Key for Code Exchange (PKCE) required ?", false);
            clientDto.RequireConsent = Prompt.GetYesNo("Is consent required ? ", false);

            return new AddClientCommand
            {
                client = clientDto,
            };
        }

        private static ICollection<string> PromptList(string elementName, IReporter reporter)
        {
            reporter.Output($"{elementName} part. Leave empty once we want no more scopes.");

            var elements = new List<string>();
            while (true)
            {
                var element = Prompt.GetString($"{elementName} {elements.Count + 1}:");
                if (string.IsNullOrEmpty(element))
                {
                    break;
                }

                elements.Add(element);
            }

            return elements;
        }
    }
}