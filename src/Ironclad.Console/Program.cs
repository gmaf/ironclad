// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console
{
    using System;
    using System.Threading.Tasks;
    using Ironclad.Client;
    using Ironclad.Console.Commands;
    using Ironclad.Console.Persistence;
    using McMaster.Extensions.CommandLineUtils;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal class Program
    {
        private readonly IConsole console;

        public Program(IConsole console)
        {
            this.console = console;
        }

        public static Task<int> Main(string[] args)
        {
            Sdk.DebugHelper.HandleDebugSwitch(ref args);

            JsonConvert.DefaultSettings =
                () =>
                new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
                };

            return new Program(PhysicalConsole.Singleton).TryRunAsync(args);
        }

        public async Task<int> TryRunAsync(string[] args)
        {
            var repository = new CommandDataRepository(null);
            var data = repository.GetCommandData() ??
                new CommandData
                {
                    Authority = LoginCommand.DefaultAuthority,
                };

            CommandLineOptions options;
            try
            {
                options = CommandLineOptions.Parse(args, this.console);
            }
            catch (CommandParsingException ex)
            {
                await this.console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                return 1;
            }

            if (options == null)
            {
                return 1;
            }

            if (options.IsHelp)
            {
                return 2;
            }

            if (options.Command == null)
            {
                return 3;
            }

            // need to get the token and check it's valid
            // get saved data

            // if the command is login then check the current
            if (options.Command?.GetType() != typeof(LoginCommand))
            {
                this.console.WriteLine($"Executing command against {data.Authority}");
            }

            //// var token = GetAccessToken

            using (var clientsClient = new ClientsHttpClient(data.Authority))
            using (var apiResourcesClient = new ApiResourcesHttpClient(data.Authority))
            using (var identityResourcesClient = new IdentityResourcesHttpClient(data.Authority))
            using (var rolesClient = new RolesHttpClient(data.Authority))
            using (var usersClient = new UsersHttpClient(data.Authority))
            {
                var context = new CommandContext(
                    this.console,
                    clientsClient,
                    apiResourcesClient,
                    identityResourcesClient,
                    rolesClient,
                    usersClient,
                    repository);

                try
                {
                    await options.Command.ExecuteAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    await this.console.Out.WriteLineAsync(ex.Message).ConfigureAwait(false);
                    return 500;
                }

                return 0;
            }
        }
    }
}
