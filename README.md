# Requirements
  - .NET Core 2.1 SDK [https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.300-preview1]
  - Sql Server
    
# Run from Visual Studio
  - Open TodoListApi.sln file in Visual Studio. 
  - Specify connection string "DbApi" in appsettings.json file.
  - In Package Management Console run command "Update-Database"
  - Run project
  
# Difference between swagger specification and current implementation
  - use "lists" insted of "list" word in all rest routes
  - use "tasks" insted of "task" word in all rest routes
