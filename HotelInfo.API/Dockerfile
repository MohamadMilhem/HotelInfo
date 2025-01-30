FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5076

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HotelInfo.API/HotelInfo.API.csproj", "HotelInfo.API/"]
RUN dotnet restore "HotelInfo.API/HotelInfo.API.csproj"
COPY . .
WORKDIR "/src/HotelInfo.API"
RUN dotnet build "HotelInfo.API.csproj" -c $BUILD_CONFIGURATION -o /out

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HotelInfo.API.csproj" -c $BUILD_CONFIGURATION -o /out /p:UseAppHost=false

FROM base AS final
WORKDIR /out
COPY --from=publish /out .
ENV ASPNETCORE_URLS=http://+:5076
ENTRYPOINT ["dotnet", "HotelInfo.API.dll"]
