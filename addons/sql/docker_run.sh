docker run -e 'ACCEPT_EULA=Y' --name sql_server -e 'MSSQL_PID=Express' -p 1433:1433 -d microsoft/mssql-server-linux:latest