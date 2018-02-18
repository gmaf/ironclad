// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Persistence
{
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.Repositories;

    public class CommandDataRepository : ICommandDataRepository
    {
        private readonly IXmlRepository innerRepository;
        private readonly IDataProtector protector;

        public CommandDataRepository(IDataProtectionProvider provider)
        {
            this.protector = provider.CreateProtector("auth_console");
        }

        public CommandData GetCommandData()
        {
            return new CommandData
            {
                Authority = "http://localhost:5005"
            };
        }

        public void SetCommandData(CommandData commandData)
        {
            var authority = this.protector.Protect(commandData.Authority);
        }
    }
}