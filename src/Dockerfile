FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY . ./
WORKDIR /src/Ironclad
RUN dotnet publish -c Release -r linux-x64 -o ../../build /p:ShowLinkerSizeComparison=true

FROM microsoft/aspnetcore:2.0 AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT Docker
COPY --from=build /build/ .
ENTRYPOINT ["dotnet", "Ironclad.dll"]