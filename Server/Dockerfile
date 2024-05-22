FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /App
COPY . .

RUN dotnet restore
RUN dotnet tool install --global dotnet-ef --version 7.0.11

EXPOSE 5199
CMD ["dotnet", "run"]
