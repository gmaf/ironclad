# Ironclad #

Secure ALL the things!

### Prerequisites

This project requires a running instance of [Postgres](https://www.postgresql.org/) and the connection string to be configured (see configuration section below).

### Configuration

#### Secrets

The secrets configuration mechanism differs when running the project directly or running inside a container.

- If running the project directly:  
You need to configure the [user secrets](https://blogs.msdn.microsoft.com/mihansen/2017/09/10/managing-secrets-in-net-core-2-0-apps/) for the project.
The contents of the `secrets.json` configuration file should match the [expected required configuration](src/Ironclad/Extensions/ConfigurationExtensions.cs?at=2&fileviewer=file-view-default#ConfigurationExtensions.cs-21).  
eg.  
    ```json
    {
        "ConnectionStrings": {
            "Ironclad": "Host=localhost;Database=ironclad;Username=username;Password=password;"
        },
        "Google-ClientId": "client_id",
        "Google-Secret": "secret"
    }
    ```

- If running the project inside a container:  
You need to configure the [environment variables](https://docs.docker.com/compose/environment-variables/#the-env_file-configuration-option) used to run the docker container.
To do this you need to create an `.env` file in the `src/Docker` folder and enter key/value pairs in the format `KEY=VALUE` for each secret.
The contents of the `.env` configuration file should match the [expected required configuration](src/Ironclad/Extensions/ConfigurationExtensions.cs?at=2&fileviewer=file-view-default#ConfigurationExtensions.cs-21).  
eg.
    ```cmd
    IRONCLAD_CONNECTIONSTRING=Host=localhost;Database=ironclad;Username=username;Password=password;
    GOOGLE_CLIENT_ID=client_id
    GOOGLE_SECRET=secret
    ```

#### Machine Specific

In addition, when running inside of a container: you need to add any _machine specific_ configuration to the `.env` file.  
eg.
    ```cmd
    LOG_PATH=S:\Logs
    ```

### How to run

Navigate to ```/src/Docker/cmd``` and execute ```start.cmd```.  
To stop, execute ```stop.cmd```.


### TODO

migrations: ```dotnet ef database update```
