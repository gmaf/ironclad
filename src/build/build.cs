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

            var isPullRequest = default(bool);

            var nugetServer = default(string);
            var nugetApiKey = default(string);

            var dockerRegistry = default(string);
            var dockerUsername = default(string);
            var dockerPassword = default(string);
            var dockerTag = default(string);

            // LINK (Cameron): https://docs.travis-ci.com/user/environment-variables/#default-environment-variables
            if (settings.TRAVIS == true)
            {
                Console.WriteLine("Travis build server detected.");

                if (int.TryParse(settings.TRAVIS_PULL_REQUEST, out var _))
                {
                    Console.WriteLine("Pull request build detected.");
                    isPullRequest = true;
                    return;
                }

                if (!string.IsNullOrEmpty(settings.TRAVIS_TAG))
                {
                    Console.WriteLine("Release build detected.");

                    nugetServer = settings.BUILD_SERVER__NUGET__SERVER;
                    nugetApiKey = settings.BUILD_SERVER__NUGET__API_KEY;

                    dockerRegistry = settings.BUILD_SERVER__DOCKER_REGISTRY;
                    dockerUsername = settings.BUILD_SERVER__DOCKER_USERNAME;
                    dockerPassword = settings.BUILD_SERVER__DOCKER_PASSWORD;
                    dockerTag = settings.TRAVIS_TAG;

                    Console.WriteLine($"Values: nugetServer = '{nugetServer}', nugetApiKey.Length = '{nugetApiKey.Length}'");
                }
                else
                {
                    Console.WriteLine("Pre-release build detected.");

                    nugetServer = settings.BUILD_SERVER__NUGET__BETA_SERVER;
                    nugetApiKey = settings.BUILD_SERVER__NUGET__BETA_API_KEY;

                    dockerRegistry = settings.BUILD_SERVER__DOCKER__BETA_REGISTRY;
                    dockerUsername = settings.BUILD_SERVER__DOCKER__BETA_USERNAME;
                    dockerPassword = settings.BUILD_SERVER__DOCKER__BETA_PASSWORD;
                    dockerTag = "latest";

                    Console.WriteLine("Release build detected.");
                }
            }
            else
            {
                Console.WriteLine("Build server not detected.");
            }

            Target(
                RestoreNugetPackages,
                () => Run("dotnet", "restore src/Ironclad.sln"));

            Target(
                BuildSolution,
                DependsOn(RestoreNugetPackages),
                () => Run("dotnet", "build src/Ironclad.sln -c CI --no-restore"));

            Target(
                BuildDockerImage,
                () => Run("docker", "build --tag ironclad ."));

            Target(
                TestDockerImage,
                DependsOn(BuildSolution, BuildDockerImage),
                // dotnet test --test-adapter-path:C:\Users\cameronfletcher\.nuget\packages\xunitxml.testlogger\2.0.0\build\_common --logger:"xunit;LogFilePath=test_result.xml"
                () => Run("dotnet", $"test src/tests/Ironclad.Tests/Ironclad.Tests.csproj -r ../../../{ArtifactsFolder} -l trx;LogFileName=Ironclad.Tests.xml --no-build"));

            Target(
                CreateNugetPackages,
                DependsOn(BuildSolution),
                ForEach(
                    "src/Ironclad.Client/Ironclad.Client.csproj", 
                    "src/Ironclad.Console/Ironclad.Console.csproj", 
                    "src/tests/Ironclad.Tests.Sdk/Ironclad.Tests.Sdk.csproj"),
                project => Run("dotnet", $"pack {project} -c Release -o ../../{(project.StartsWith("src/tests") ? "../" : "") + ArtifactsFolder} --no-build"));

            Target(
                PublishNugetPackages,
                DependsOn(CreateNugetPackages, TestDockerImage),
                () =>
                {
                    var packagesToPublish = Directory.GetFiles(ArtifactsFolder, "*.nupkg", SearchOption.TopDirectoryOnly);
                    Console.WriteLine($"Found packages to publish: {string.Join("; ", packagesToPublish)}");

                    if (isPullRequest)
                    {
                        Console.WriteLine("Build is pull request. Packages will not be published.");
                        return;
                    }

                    Console.WriteLine($"Values: nugetServer = '{nugetServer}', nugetApiKey.Length = '{nugetApiKey.Length}'");

                    if (string.IsNullOrWhiteSpace(nugetServer) || string.IsNullOrWhiteSpace(nugetApiKey))
                    {
                        Console.WriteLine("NuGet settings not specified. Packages will not be published.");
                        return;
                    }

                    foreach (var packageToPublish in packagesToPublish)
                    {
                        Run("dotnet", $"nuget push {packageToPublish} -s {nugetServer} -k {nugetApiKey}", noEcho: true);
                    }
                });

            Target(
                PublishDockerImage,
                DependsOn(TestDockerImage),
                () =>
                {
                    if (isPullRequest)
                    {
                        Console.WriteLine("Build is pull request. Docker images will not be published.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(dockerRegistry) || string.IsNullOrWhiteSpace(dockerUsername) || string.IsNullOrWhiteSpace(dockerPassword))
                    {
                        Console.WriteLine("Docker settings not specified. Docker images will not be published.");
                        return;
                    }

                    Run("docker", $"login {dockerRegistry} -u {dockerUsername} -p {dockerPassword}");
                    Run("docker", $"tag ironclad:latest {dockerRegistry}/ironclad:{dockerTag}");
                    Run("docker", $"docker push {dockerRegistry}/ironclad:{dockerTag}");
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
            // travis specific
            public bool? TRAVIS { get; set; }
            public string TRAVIS_PULL_REQUEST { get; set; }
            public string TRAVIS_TAG { get; set; }

            // build specific
            public string BUILD_SERVER__NUGET__BETA_SERVER { get; set; }
            public string BUILD_SERVER__NUGET__BETA_API_KEY { get; set; }
            public string BUILD_SERVER__NUGET__SERVER { get; set; }
            public string BUILD_SERVER__NUGET__API_KEY { get; set; }
            public string BUILD_SERVER__DOCKER__BETA_REGISTRY { get; set; }
            public string BUILD_SERVER__DOCKER__BETA_USERNAME { get; set; }
            public string BUILD_SERVER__DOCKER__BETA_PASSWORD { get; set; }
            public string BUILD_SERVER__DOCKER_REGISTRY { get; set; }
            public string BUILD_SERVER__DOCKER_USERNAME { get; set; }
            public string BUILD_SERVER__DOCKER_PASSWORD { get; set; }
        }
    }
}
