set -e
echo "start entrypoint"
/opt/mssql-tools18/bin/sqlcmd -No -S sql -U sa -P MyPass@word -d master -i DbInit.sql
sleep 10
dotnet TranslatorApp.dll
echo "end entrypoint"
