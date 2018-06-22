FROM microsoft/dotnet:2.1-sdk-alpine AS build
ARG version=0.0.1-developer
WORKDIR /src
COPY . ./
WORKDIR /src/Ironclad
RUN dotnet publish -c Release -r linux-musl-x64 -o ../../build /p:ShowLinkerSizeComparison=true /p:Version=$version --self-contained

FROM microsoft/dotnet:2.1-runtime-deps-alpine AS final
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT Docker
COPY --from=build /build/ .
ENTRYPOINT ["/app/Ironclad"]