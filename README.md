# Requirements
  - .NET Core 2.1 SDK [https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.300-preview1]
    
# Run from Visual Studio
  - Open *TodoListApi.sln* file in Visual Studio. 
  - Run project
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