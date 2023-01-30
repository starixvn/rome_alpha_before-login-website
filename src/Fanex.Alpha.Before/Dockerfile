FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /source
COPY src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj -v n

COPY src/Fanex.Alpha.Before src/Fanex.Alpha.Before

ARG TRANSFORM_CONFIG
ARG CI_JOB_TOKEN
RUN echo $TRANSFORM_CONFIG
RUN mkdir /source/tmp
RUN if [ "$TRANSFORM_CONFIG" = "PRO" ] \
		|| [ "$TRANSFORM_CONFIG" = "PRO2" ] \
		|| [ "$TRANSFORM_CONFIG" = "LOCAL" ] \
		|| [ "$TRANSFORM_CONFIG" = "PRO3" ]; \
	then cd /source/tmp && curl --fail -o $TRANSFORM_CONFIG.domainmappings.json --header "PRIVATE-TOKEN:$CI_JOB_TOKEN" https://gitlab.nexdev.net/api/v4/projects/alpha-config%2Flogin-domains/repository/files/src%2F$TRANSFORM_CONFIG.domainmappings.json/raw?ref=182789-UpgradeNetCore \
		&& cd - && cp -r tmp/$TRANSFORM_CONFIG.domainmappings.json src/Fanex.Alpha.Before/domainmappings.$TRANSFORM_CONFIG.json; fi

RUN if [ "$TRANSFORM_CONFIG" != "" ]; \
then \
  cp "src/Fanex.Alpha.Before/domainmappings.$TRANSFORM_CONFIG.json" "src/Fanex.Alpha.Before/domainmappings.json"; \
fi

RUN if [ "$TRANSFORM_CONFIG" = "LOCAL2" ] \
  || [ "$TRANSFORM_CONFIG" = "SIT2" ] \
  || [ "$TRANSFORM_CONFIG" = "UAT2" ] \
  || [ "$TRANSFORM_CONFIG" = "PRO2" ] \
  || [ "$TRANSFORM_CONFIG" = "AWS2" ] \
  || [ "$TRANSFORM_CONFIG" = "backupMNLi7" ]; \
then \
	cp "src/Fanex.Alpha.Before/wwwroot/favicon2.ico" "src/Fanex.Alpha.Before/wwwroot/favicon.ico"; \
  cp "src/Fanex.Alpha.Before/bundleconfig2.json" "src/Fanex.Alpha.Before/bundleconfig.json";\
fi

WORKDIR /source/src/Fanex.Alpha.Before
RUN dotnet publish -r linux-x64 --self-contained false -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0 AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
#

# final stage/image
WORKDIR /app
COPY --from=builder /app ./
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Alpha.Before.dll"]