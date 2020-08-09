#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY DummyCrudApi/DummyCrudApi.csproj DummyCrudApi/
COPY DbConnectionBuilderProvider/DbConnectionBuilderProvider.csproj DbConnectionBuilderProvider/
COPY DbConnectionBuilderProvider.SQLiteConnectionBuilder/DbConnectionBuilderProvider.SQLiteConnectionBuilder.csproj DbConnectionBuilderProvider.SQLiteConnectionBuilder/
COPY DbContextProvider/DbContextProvider.csproj DbContextProvider/
COPY DbContextProvider.FirebaseRealtimeDb/DbContextProvider.FirebaseRealtimeDb.csproj DbContextProvider.FirebaseRealtimeDb/
COPY Retrofit/Retrofit.csproj Retrofit/
RUN dotnet restore "DummyCrudApi/DummyCrudApi.csproj"
COPY . .
WORKDIR "/src/DummyCrudApi"
RUN dotnet build "DummyCrudApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DummyCrudApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DummyCrudApi.dll"]