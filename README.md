# General
This api was created as Todo List example. You could look api specification [here](https://app.swaggerhub.com/apis/dsinelnikov/TodoListApi/1.0.0#/default/completeTask) 
Also you can try api by link http://testapp-todolistapi.azurewebsites.net/v1/lists

Application use custom in memory storage, to demonstrate multithreading experience.

# Requirements
  - .NET Core 2.1 SDK [https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.301]
    
# Run from Visual Studio
  - Open *TodoListApi.sln* file in Visual Studio. 
  - Run project

# Azure
Api project contains *TestApp-TodoListApi - Web Deploy* profile to publish application to Azure.
Last application version is already published and is accessible by link http://testapp-todolistapi.azurewebsites.net/v1/lists

# Run in docker
Open root directory in console (cmd.exe) and execute followin command:
  ```  
  docker-compose up
  ```
When cotainer will be run, REST API will be available by URL http://localhost:5000. You can specify other port in *docker-compose.override.yml* file.

# Build in Docker
To build TodoListApi run following commain in root directory. It builds application and put artifacts to directory *.\TodoListApi\obj\Docker\publish*
   ```
   docker-compose -f docker-compose.ci.build.yml up
   ```
This command is usefull to build application on build machine.

# Unit Test
Solution contains unit test examples for Controller, In Memory Storage and Api Services. You could run it by Visual Studio Test Explorer.