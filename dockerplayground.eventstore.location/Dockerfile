FROM microsoft/aspnetcore-build:2.0
COPY ./publish /app
WORKDIR /app
ENTRYPOINT ["dotnet", "dockerplayground.eventstore.location.dll"]