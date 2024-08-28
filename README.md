# TranslatorApp

TranslatorApp is a dockerized demo ASP.NET Core MVC application which uses [external API](https://funtranslations.com/api/) to translate texts and save transslations in a dockerized database created on Sql Express server. 
It also supports adding additonal translator API links from mentioned API.

## Requirements

Installed [.NET](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net80)

Installed [Docker Engine](https://docs.docker.com/engine/install/)

## Running the app
Copy the repository

Open CMD/PowerShell in the main TranslatorApp folder (has to contain docker-compose.yml)

Run following commands
```bash
docker compose build
docker compose up
```

After about 20 seconds you should be able to access the application in your browser at https://localhost:5001.
