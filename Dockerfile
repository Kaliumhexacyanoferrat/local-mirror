FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG TARGETARCH
WORKDIR /source

COPY --link LocalMirror/*.csproj .
RUN dotnet restore -a "$TARGETARCH"

COPY --link LocalMirror/. .
RUN dotnet publish --no-restore -a "$TARGETARCH" -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
EXPOSE 8080
WORKDIR /app
COPY --link --from=build /app .

RUN mkdir /app/cache
RUN chown -R $APP_UID:$APP_UID /app/cache
RUN chmod 755 /app/cache

USER $APP_UID

ENTRYPOINT ["./LocalMirror"]
