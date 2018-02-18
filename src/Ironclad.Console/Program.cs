// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

#pragma warning disable CA1812

namespace Ironclad.Console
{
    using System;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Ironclad.Client;
    using Ironclad.Console.Commands;
    using Ironclad.Console.Persistence;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    internal class Program
    {
        private readonly IConsole console;
        private readonly IDataProtectionProvider provider;

        public Program(IConsole console, IDataProtectionProvider provider)
        {
            this.console = console;
            this.provider = provider;
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

            // LINK (Cameron): https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/using-data-protection
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDataProtection();
            serviceCollection.AddSingleton(PhysicalConsole.Singleton);
            var services = serviceCollection.BuildServiceProvider();

            var instance = ActivatorUtilities.CreateInstance<Program>(services);
            return instance.TryRunAsync(args);
        }

        public async Task<int> TryRunAsync(string[] args)
        {
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

            var repository = new CommandDataRepository(this.provider);
            var data = repository.GetCommandData() ??
                new CommandData
                {
                    Authority = LoginCommand.DefaultAuthority,
                };

            // if the command is login then check the current
            if (options.Command?.GetType() != typeof(LoginCommand))
            {
                // validate tokens
                this.console.WriteLine($"Executing command against {data.Authority}");
            }

            // by this point we have to have valid tokens unless we're calling the login command
            using (var handler = new RefreshTokenHandler(new TokenClient(data.Authority), data.RefreshToken, data.AccessToken))
            using (var clientsClient = new ClientsHttpClient(data.Authority, handler))
            using (var apiResourcesClient = new ApiResourcesHttpClient(data.Authority, handler))
            using (var identityResourcesClient = new IdentityResourcesHttpClient(data.Authority, handler))
            using (var rolesClient = new RolesHttpClient(data.Authority, handler))
            using (var usersClient = new UsersHttpClient(data.Authority, handler))
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
