// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Validators;

    internal class UpdateClientUrisCommand : ICommand
    {
        private string clientId;
        private string clientUriName;
        private List<string> clientUriValues;

        private UpdateClientUrisCommand()
        {
        }

        public static void Configure(CommandLineApplication app, CommandLineOptions options, IConsole console)
        {
            // description
            app.Description = "Update URIs for the specified client";
            app.HelpOption();

            // arguments
            var argumentClientId = app.Argument("id", "The client ID", false);

            var argumentUriName = app.Argument("name", "Name of the URI");
            argumentUriName.Validators.Add(new ClientUriNameValidator());
            var argumentUriValues = app.Argument("values", "List of URIs", true);

            // action (for this command)
            app.OnExecute(() =>
            {
                if (string.IsNullOrEmpty(argumentClientId.Value))
                {
                    app.ShowHelp();
                    return;
                }

                if (string.IsNullOrEmpty(argumentUriName.Value))
                {
                    console.Out.WriteLine("Entering interactive mode. Leave entry empty to finish editing.");

                    string uriName;
                    var uriValues = new List<string>();
                    var validator = new ClientUriNameValidator();
                    ValidationResult result;

// Benoit: do {} while syntax exception.
#pragma warning disable SA1500
                    do
                    {
                        uriName = Prompt.GetString("URI field name:");
                        var command = new CommandArgument
                        {
                            Name = "URI name",
                            Values = { uriName }
                        };
                        result = validator.GetValidationResult(command, null);
                        if (!string.IsNullOrEmpty(result?.ErrorMessage))
                        {
                            console.WriteLine(result.ErrorMessage);
                        }
                    } while (result != null && !result.Equals(ValidationResult.Success));
#pragma warning restore SA1500

                    if (result == null)
                    {
                        options.Command = new AbortCommand();
                    }

                    while (true)
                    {
                        var uriValue = Prompt.GetString("URI:");
                        if (string.IsNullOrEmpty(uriValue))
                        {
                            break;
                        }

                        console.WriteLine("Added.");
                        uriValues.Add(uriValue);
                    }

                    options.Command = new UpdateClientUrisCommand
                    {
                        clientId = argumentClientId.Value,
                        clientUriName = uriName,
                        clientUriValues = uriValues
                    };
                }
                else
                {
                    options.Command = new UpdateClientUrisCommand
                    {
                        clientId = argumentClientId.Value,
                        clientUriName = argumentUriName.Value,
                        clientUriValues = argumentUriValues.Values
                    };
                }
            });
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var client = new Ironclad.Client.Client
            {
                Id = this.clientId,
            };

            try
            {
                var property = client.GetType().GetProperty(this.clientUriName);
                property.SetValue(client, this.clientUriValues);

                await context.ClientsClient.ModifyClientAsync(client).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                context.Reporter.Error(e.ToString());
                await context.Console.Out.WriteLineAsync("Failed!").ConfigureAwait(false);
            }
        }

        private class AbortCommand : ICommand
        {
            public async Task ExecuteAsync(CommandContext context)
            {
                await context.Console.Out.WriteLineAsync("Aborted!").ConfigureAwait(false);
            }
        }
    }
}