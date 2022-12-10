FROM dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443



FROM node:12.18.2 AS npm
WORKDIR /app
COPY Template/.npmrc .
COPY Template/ClientApp/*.js ./
COPY Template/ClientApp/package.json ./
COPY Template/ClientApp/src ./src
COPY Template/ClientApp/public ./public

RUN npm install
RUN npm run build

FROM sdk:5.0-buster-slim AS build
COPY Template/NuGet.config /root/.nuget/NuGet/NuGet.Config
WORKDIR /src
COPY Template/Template.csproj Template/
RUN dotnet restore Template/Template.csproj
COPY . .
WORKDIR /src/Template
COPY --from=npm /wwwroot/artifacts/Index.cshtml ./Views/Home/Index.cshtml
RUN dotnet build Template.csproj -c Release -o /app/build
RUN dotnet publish Template.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=npm /wwwroot ./wwwroot
ENTRYPOINT ["dotnet", "Template.dll"]