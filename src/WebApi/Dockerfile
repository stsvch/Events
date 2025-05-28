FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "WebApi/Events.WebApi.csproj"
RUN dotnet publish "WebApi/Events.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5144
EXPOSE 7188

ENTRYPOINT ["dotnet", "Events.WebApi.dll"]
