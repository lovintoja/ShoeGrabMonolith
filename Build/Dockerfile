FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ./../ShoeGrabMonolith/*.csproj ./ShoeGrabMonolith/
COPY ./../ShoeGrab*/ ./shoe-grab-common/
COPY ./../shoe-grab-user/ ./shoe-grab-user/

RUN dotnet restore ./shoe-grab-user/*.csproj
RUN dotnet publish ./shoe-grab-user/*.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /out .
ENV DB_CONNECTION_STRING="Host=192.168.0.121;Port=30002;Database=db2;Username=admin;Password=admin"
ENTRYPOINT ["dotnet", "ShoeGrabUserManagement.dll"]



# Use a base image for .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./../ShoeGrabMonolith.sln ./

COPY ./../ShoeGrabMonolith/ ShoeGrabMonolith/
COPY ./../ShoeGrabAdminService/ ShoeGrabAdminService/
COPY ./../ShoeGrabCommonModels/ ShoeGrabCommonModels/
COPY ./../ShoeGrabCRMService/ ShoeGrabCRMService/
COPY ./../ShoeGrabOrderManagement/ ShoeGrabOrderManagement/
COPY ./../ShoeGrabProductManagement/ ShoeGrabProductManagement/
COPY ./../ShoeGrabUserManagement/ ShoeGrabUserManagement/

RUN dotnet restore ShoeGrabMonolith.sln

RUN dotnet build ShoeGrabMonolith/ShoeGrabMonolith.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ShoeGrabMonolith/ShoeGrabMonolith.csproj -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoeGrabMonolith.dll"]
