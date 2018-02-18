// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Console.Persistence
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.DataProtection.Repositories;
    using Microsoft.Extensions.Logging.Abstractions;

    public class CommandDataRepository : ICommandDataRepository
    {
        private const string RepositoryFolderName = "auth.exe";

        private static readonly XmlSerializer Serializer = new XmlSerializer(typeof(CommandData));

        private readonly IXmlRepository innerRepository = new FileSystemXmlRepository(GetDefaultDataStorageDirectory(), NullLoggerFactory.Instance);
        private readonly IDataProtector protector;

        public CommandDataRepository(IDataProtectionProvider provider)
        {
            this.protector = provider.CreateProtector("auth_console");
        }

        public CommandData GetCommandData()
        {
            var element = this.innerRepository.GetAllElements().FirstOrDefault();
            if (element == null)
            {
                return null;
            }

            var data = (CommandData)Serializer.Deserialize(element.CreateReader());

            return new CommandData
            {
                Authority = data.Authority,
                AccessToken = this.protector.Unprotect(data.AccessToken),
                RefreshToken = this.protector.Unprotect(data.RefreshToken),
            };
        }

        public void SetCommandData(CommandData commandData)
        {
            ////var authority = this.protector.Protect(commandData.Authority);
            var data = new CommandData
            {
                Authority = commandData.Authority,
                AccessToken = this.protector.Protect(commandData.AccessToken),
                RefreshToken = this.protector.Protect(commandData.RefreshToken),
            };

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                Serializer.Serialize(streamWriter, data);

                var xml = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));
                this.innerRepository.StoreElement(xml, "command-data");
            }
        }

        // LINK (Cameron): https://github.com/aspnet/DataProtection/blob/dev/src/Microsoft.AspNetCore.DataProtection/Repositories/FileSystemXmlRepository.cs
        private static DirectoryInfo GetDefaultDataStorageDirectory()
        {
            DirectoryInfo directoryInfo;

            // Environment.GetFolderPath returns null if the user profile isn't loaded.
            var localAppDataFromSystemPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var localAppDataFromEnvPath = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            var userProfilePath = Environment.GetEnvironmentVariable("USERPROFILE");
            var homePath = Environment.GetEnvironmentVariable("HOME");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !string.IsNullOrEmpty(localAppDataFromSystemPath))
            {
                // To preserve backwards-compatibility with 1.x, Environment.SpecialFolder.LocalApplicationData
                // cannot take precedence over $LOCALAPPDATA and $HOME/.aspnet on non-Windows platforms
                directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(localAppDataFromSystemPath);
            }
            else if (localAppDataFromEnvPath != null)
            {
                directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(localAppDataFromEnvPath);
            }
            else if (userProfilePath != null)
            {
                directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(Path.Combine(userProfilePath, "AppData", "Local"));
            }
            else if (homePath != null)
            {
                // If LOCALAPPDATA and USERPROFILE are not present but HOME is, it's a good guess that this is a *NIX machine.  Use *NIX conventions for a folder name.
                directoryInfo = new DirectoryInfo(Path.Combine(homePath, ".lykke", RepositoryFolderName));
            }
            else if (!string.IsNullOrEmpty(localAppDataFromSystemPath))
            {
                // Starting in 2.x, non-Windows platforms may use Environment.SpecialFolder.LocalApplicationData
                // but only after checking for $LOCALAPPDATA, $USERPROFILE, and $HOME.
                directoryInfo = GetKeyStorageDirectoryFromBaseAppDataPath(localAppDataFromSystemPath);
            }
            else
            {
                return null;
            }

            Debug.Assert(directoryInfo != null, "The storage directory was not located.");

            try
            {
                directoryInfo.Create(); // throws if we don't have access, e.g., user profile not loaded
                return directoryInfo;
            }
            catch
            {
                return null;
            }
        }

        private static DirectoryInfo GetKeyStorageDirectoryFromBaseAppDataPath(string basePath) => new DirectoryInfo(Path.Combine(basePath, "Lykke", RepositoryFolderName));
    }
}