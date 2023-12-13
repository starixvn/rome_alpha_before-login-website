FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /src

COPY src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj ./Fanex.Alpha.Before/Fanex.Alpha.Before.csproj
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore Fanex.Alpha.Before/Fanex.Alpha.Before.csproj -v n

COPY src/Fanex.Alpha.Before ./Fanex.Alpha.Before

ARG TRANSFORM_CONFIG
ARG CI_JOB_TOKEN
RUN echo $TRANSFORM_CONFIG
RUN mkdir /src/tmp
RUN if [ "$TRANSFORM_CONFIG" = "PRO" ] \
		|| [ "$TRANSFORM_CONFIG" = "PRO2" ] \
    || [ "$TRANSFORM_CONFIG" = "PRO3" ] \
    || [ "$TRANSFORM_CONFIG" = "PRO5" ] \
    || [ "$TRANSFORM_CONFIG" = "DR" ] \
    || [ "$TRANSFORM_CONFIG" = "DR2" ] \
    || [ "$TRANSFORM_CONFIG" = "DR3" ] \
    || [ "$TRANSFORM_CONFIG" = "DR4" ] \
    || [ "$TRANSFORM_CONFIG" = "DR5" ] \
    || [ "$TRANSFORM_CONFIG" = "STG" ] \
    || [ "$TRANSFORM_CONFIG" = "STG2" ] \
		|| [ "$TRANSFORM_CONFIG" = "STG3" ]; \
	then cd /src/tmp && curl --fail -o $TRANSFORM_CONFIG.domainmappings.json --header "PRIVATE-TOKEN:$CI_JOB_TOKEN" https://gitlab.nexdev.net/api/v4/projects/alpha-config%2Flogin-domains/repository/files/src%2F$TRANSFORM_CONFIG.domainmappings.json/raw?ref=main \
		&& cd - && cp -r tmp/$TRANSFORM_CONFIG.domainmappings.json Fanex.Alpha.Before/domainmappings.$TRANSFORM_CONFIG.json; fi

RUN if [ "$TRANSFORM_CONFIG" != "" ]; \
then \
  cp "Fanex.Alpha.Before/domainmappings.$TRANSFORM_CONFIG.json" "Fanex.Alpha.Before/domainmappings.json"; \
fi

RUN if [ "$TRANSFORM_CONFIG" = "LOCAL2" ] \
  || [ "$TRANSFORM_CONFIG" = "SIT2" ] \
  || [ "$TRANSFORM_CONFIG" = "UAT2" ] \
  || [ "$TRANSFORM_CONFIG" = "STG2" ] \
  || [ "$TRANSFORM_CONFIG" = "PRO2" ] \
  || [ "$TRANSFORM_CONFIG" = "AWS2" ] \
  || [ "$TRANSFORM_CONFIG" = "DR2" ]; \
then \
	cp "Fanex.Alpha.Before/wwwroot/favicon2.ico" "Fanex.Alpha.Before/wwwroot/favicon.ico"; \
fi

WORKDIR /src/Fanex.Alpha.Before
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0-slim AS base

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

# final stage/image
ENV COMPlus_EnableDiagnostics=0
WORKDIR /app
COPY --from=builder /app ./
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Alpha.Before.dll"]