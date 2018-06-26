FROM microsoft/dotnet:2.1-sdk AS builder
WORKDIR /sources

# Copy csproj and restore as distinct layers
COPY ./TodoListApi.sln ./
COPY ./docker-compose.dcproj ./
COPY ./Core/Core.csproj Core/
COPY ./InMemoryStorage/InMemoryStorage.csproj InMemoryStorage/
COPY ./TodoListApi/TodoListApi.csproj TodoListApi/

RUN dotnet restore TodoListApi.sln

# Copy everything else and build
COPY . .
RUN dotnet publish TodoListApi.sln -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=builder /sources/TodoListApi/out .
ENTRYPOINT ["dotnet", "TodoListApi.dll"]