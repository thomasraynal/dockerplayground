FROM microsoft/aspnetcore-build:2.0
COPY ./publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "dockerplayground.proximity.service.dll"]