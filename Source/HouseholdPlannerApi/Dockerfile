FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["HouseholdPlannerApi/HouseholdPlannerApi.csproj", "HouseholdPlannerApi/"]
RUN dotnet restore "HouseholdPlannerApi/HouseholdPlannerApi.csproj"
COPY . .
WORKDIR "/src/HouseholdPlannerApi"
RUN dotnet build "HouseholdPlannerApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "HouseholdPlannerApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HouseholdPlannerApi.dll"]