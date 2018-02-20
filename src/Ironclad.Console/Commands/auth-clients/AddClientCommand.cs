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
        private string clientId;
        private string clientSecret;
        private string clientName;

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
            var optionsName = app.Option("-n|--name", "The name of the client", CommandOptionType.SingleValue);

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
                            clientId = argumentClientId.Value,
                            clientSecret = argumentClientSecret.Value,
                            clientName = optionsName.Value()
                        };
                    }
                });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var client = new Ironclad.Client.Client
            {
                Id = this.clientId,
                Name = this.clientName,
                Secret = this.clientSecret,
            };

            await context.ClientsClient.AddClientAsync(client).ConfigureAwait(false);
        }

        private static AddClientCommand GetClientFromPrompt(IReporter reporter)
        {
            var id = Prompt.GetString("Unique Id:");
            if (string.IsNullOrEmpty(id))
            {
                reporter.Error("Id cannot be null");
                return null;
            }

            var name = Prompt.GetString("Name:", id);
            var secret = Prompt.GetPassword("Secret:");
            if (string.IsNullOrEmpty(secret))
            {
                reporter.Error("Secret cannot be null");
                return null;
            }

            var accessTokenType = Prompt.GetString("AccessTokenType:", "JWT");

            var cors = PromptList("AllowedCorsOrigins", reporter);
            var redirects = PromptList("RedirectUris", reporter);
            var postLogoutRedirectUris = PromptList("PostLogoutRedirectUris", reporter);
            var allowedScopes = PromptList("AllowedScopes", reporter);
            var allowedGrantTypes = PromptList("AllowedGrantTypes", reporter);

            var allowAccessTokensViaBrowser = Prompt.GetYesNo("Allow Access Token Via Browser ? ", true);
            var allowOfflineAccess = Prompt.GetYesNo("Allow Offline Access ? ", true);

            return new AddClientCommand
            {
                clientId = id,
                clientName = name,
                clientSecret = secret,
            };
        }

        private static IEnumerable<string> PromptList(string elementName, IReporter reporter)
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