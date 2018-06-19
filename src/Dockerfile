FROM microsoft/dotnet:2.1-sdk AS build
ARG version=0.0.1-developer
WORKDIR /src
COPY . ./
WORKDIR /src/Ironclad
RUN dotnet publish -c Release -r linux-x64 -o ../../build /p:ShowLinkerSizeComparison=true /p:Version=$version

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT Docker
COPY --from=build /build/ .
ENTRYPOINT ["dotnet", "Ironclad.dll"]