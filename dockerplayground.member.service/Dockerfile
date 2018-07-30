FROM microsoft/aspnetcore-build:2.0
COPY ./publish /app
WORKDIR /app
#EXPOSE 5000
#ENV ASPNETCORE_ENVIRONMENT Development
#ENV ASPNETCORE_URLS http://*:5000
ENTRYPOINT ["dotnet", "dockerplayground.member.service.dll"]