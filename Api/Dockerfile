FROM microsoft/dotnet:2.1-sdk AS builder
WORKDIR /sources

# Copy csproj and restore as distinct layers
COPY ./TodoList.sln ./
COPY ./docker-compose.dcproj ./
COPY ./Core/Core.csproj Core/
COPY ./InMemoryStorage/InMemoryStorage.csproj InMemoryStorage/
COPY ./Backend/Backend.csproj Backend/
COPY ./Api/Api.csproj Api/

# Copy tests csproj
COPY ./Tests/InMemoryStorage.Tests/InMemoryStorage.Tests.csproj Tests/InMemoryStorage.Tests/
COPY ./Tests/Backend.Tests/Backend.Tests.csproj Tests/Backend.Tests/
COPY ./Tests/Api.Tests/Api.Tests.csproj Tests/Api.Tests/


RUN dotnet restore TodoList.sln

# Copy everything else and build
COPY . .
RUN dotnet publish TodoList.sln -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=builder /sources/Api/out .
ENTRYPOINT ["dotnet", "TodoList.Api.dll"]