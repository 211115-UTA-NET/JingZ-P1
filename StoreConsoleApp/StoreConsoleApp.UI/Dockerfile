#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# The multistage build feature helps make the process of building containers more efficient, 
# and makes containers smaller by allowing them to contain only the bits that your app needs at run time.

# Create an intermediate image base that exposes ports 80 and 443, and sets the working directory to /app.
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# The sdk image has all the build tools, 
# and for that reason it's a lot bigger than the aspnet image
# which only contains runtime components. 
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["StoreConsoleApp.UI/StoreConsoleApp.UI.csproj", "StoreConsoleApp.UI/"]
RUN dotnet restore "StoreConsoleApp.UI/StoreConsoleApp.UI.csproj"
COPY . .
WORKDIR "/src/StoreConsoleApp.UI"
RUN dotnet build "StoreConsoleApp.UI.csproj" -c Release -o /app/build

# The final stage starts again from base, 
# and includes the COPY --from=publish to copy the published output to the final image. 
# This process makes it possible for the final image to be a lot smaller, 
# since it doesn't need to include all of the build tools that were in the sdk image.
FROM build AS publish
RUN dotnet publish "StoreConsoleApp.UI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StoreConsoleApp.UI.dll"]