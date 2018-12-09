namespace Ironclad.Build
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    using static Bullseye.Targets;
    using static SimpleExec.Command;

    internal static class Program
    {
        private const string RestoreNugetPackages   = "restore";
        private const string BuildSolution          = "build";
        private const string BuildDockerImage       = "docker";
        private const string TestDockerImage        = "test";
        private const string PublishDockerImage     = "publish-docker";
        private const string CreateNugetPackages    = "pack";
        private const string PublishNugetPackages   = "publish-nuget";
        private const string PublishAll             = "publish";

        private const string ArtifactsFolder        = "artifacts";

        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var settings = configuration.Get<Settings>();

            Target(
                    RestoreNugetPackages,                           () => Run("dotnet", "restore src/Ironclad.sln"));

            Target(
                    BuildSolution,
                    DependsOn(RestoreNugetPackages),                () => Run("dotnet", "build src/Ironclad.sln -c CI --no-restore"));

            Target(
                    BuildDockerImage,                               () => Run("docker", "build --tag ironclad ."));

            Target(
                    TestDockerImage,                                // dotnet test --test-adapter-path:C:\Users\cameronfletcher\.nuget\packages\xunitxml.testlogger\2.0.0\build\_common --logger:"xunit;LogFilePath=test_result.xml"
                    DependsOn(BuildSolution, BuildDockerImage),     () => Run("dotnet", $"test src/tests/Ironclad.Tests/Ironclad.Tests.csproj -r ../../../{ArtifactsFolder} -l trx;LogFileName=Ironclad.Tests.xml --no-build"));

            Target(
                    CreateNugetPackages,
                    DependsOn(BuildSolution),                       ForEach("src/Ironclad.Client/Ironclad.Client.csproj", "src/Ironclad.Console/Ironclad.Console.csproj", "src/tests/Ironclad.Tests.Sdk/Ironclad.Tests.Sdk.csproj"),
                                                                    project => Run("dotnet", $"pack {project} -c Release -o ../../{(project.StartsWith("src/tests") ? "../" : "") + ArtifactsFolder} --no-build"));

            Target(
                    PublishNugetPackages,
                    DependsOn(CreateNugetPackages, TestDockerImage),() =>
                                                                    {
                                                                        var packagesToPublish = Directory.GetFiles(ArtifactsFolder, "*.nupkg", SearchOption.TopDirectoryOnly);
                                                                        Console.WriteLine($"Found packages to publish: {string.Join("; ", packagesToPublish)}");

                                                                        var apiKey = settings.NUGET__API_KEY;

                                                                        if (string.IsNullOrWhiteSpace(apiKey))
                                                                        {
                                                                            Console.WriteLine("NuGet API key not specified. Packages will not be published.");
                                                                            Console.WriteLine($"To enable publishing of NuGet packages please add the environment variables: '{nameof(Settings.NUGET__API_KEY)}' and '{nameof(Settings.NUGET__SERVER)}'.");
                                                                            return;
                                                                        }

                                                                        foreach (var packageToPublish in packagesToPublish)
                                                                        {
                                                                            Run("dotnet", $"nuget push {packageToPublish} -s {settings.NUGET__SERVER} -k {apiKey}", noEcho: true);
                                                                        }
                                                                    });

            Target(
                    PublishDockerImage,
                    DependsOn(TestDockerImage),                     () =>
                                                                    {
                                                                        if (string.IsNullOrWhiteSpace(settings.BUILD_SERVER__DOCKER_REGISTRY))
                                                                        {
                                                                            Console.WriteLine("Docker registry not specified. Docker images will not be published.");
                                                                            Console.WriteLine($"To enable publishing of docker images please add the environment variable: '{nameof(Settings.BUILD_SERVER__DOCKER_REGISTRY)}'.");
                                                                            return;
                                                                        }

                                                                        if (string.IsNullOrWhiteSpace(settings.BUILD_SERVER__DOCKER_USERNAME) || string.IsNullOrWhiteSpace(settings.BUILD_SERVER__DOCKER_PASSWORD))
                                                                        {
                                                                            Console.WriteLine("Docker credentials not specified. Docker images will not be published.");
                                                                            Console.WriteLine($"To enable publishing of docker images please add the environment variables: '{nameof(Settings.BUILD_SERVER__DOCKER_USERNAME)}' and '{nameof(Settings.BUILD_SERVER__DOCKER_PASSWORD)}'.");
                                                                            return;
                                                                        }

                                                                        Run("docker", $"login {settings.BUILD_SERVER__DOCKER_REGISTRY} -u {settings.BUILD_SERVER__DOCKER_USERNAME} -p {settings.BUILD_SERVER__DOCKER_PASSWORD}");
                                                                        Run("docker", $"tag ironclad:latest {settings.BUILD_SERVER__DOCKER_REGISTRY}/ironclad:latest");
                                                                        Run("docker", $"docker push {settings.BUILD_SERVER__DOCKER_REGISTRY}/ironclad:latest");
                                                                    });

            Target(
                    PublishAll,
                    DependsOn(PublishNugetPackages, PublishDockerImage));

            Target(
                    "default",
                    DependsOn(PublishNugetPackages, PublishDockerImage));

            RunTargets(args);
        }

        private class Settings
        {
            public string NUGET__SERVER { get; set; }
            public string NUGET__API_KEY { get; set; }
            public string BUILD_SERVER__DOCKER_REGISTRY { get; set; }
            public string BUILD_SERVER__DOCKER_USERNAME { get; set; }
            public string BUILD_SERVER__DOCKER_PASSWORD { get; set; }
        }
    }
}
