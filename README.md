# TranslatorApp

TranslatorApp is a dockerized demo ASP.NET Core MVC application which uses [external API](https://funtranslations.com/api/) to translate texts and save translations in a dockerized database created on Sql Express server. 
It also supports adding additonal translator API links from mentioned API.
## Supported OS

Windows

## Requirements

Installed [.NET](https://learn.microsoft.com/en-us/dotnet/core/install/windows?tabs=net80)

Installed [Docker Engine](https://docs.docker.com/engine/install/)

## Running the app
Copy the repository

Open PowerShell in the main TranslatorApp folder (has to contain docker-compose.yml)

Run following commands
```bash
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\TranslatorApp\aspnetapp.pfx -p password
dotnet dev-certs https --trust
```
The password is just temporary password for the sake of development. In production certs would have to be created and password take from for example a key vault.

Then in the same folder run
```bash
docker compose build
docker compose up
```

After about 20 seconds you should be able to access the application in your browser at https://localhost:5001.
