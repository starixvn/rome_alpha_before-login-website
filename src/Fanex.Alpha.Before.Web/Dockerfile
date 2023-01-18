FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /source
COPY "src/Fanex.Alpha.Before.NetThrottling/Fanex.Alpha.Before.NetThrottling.csproj" "src/Fanex.Alpha.Before.NetThrottling/Fanex.Alpha.Before.NetThrottling.csproj"
COPY "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj" "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj"
COPY "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj" "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj"
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj" -v n

COPY . .

ARG TRANSFORM_CONFIG
ARG CI_JOB_TOKEN
RUN echo $TRANSFORM_CONFIG
RUN mkdir /source/tmp
RUN if [ "$TRANSFORM_CONFIG" = "PRO" ] \
		|| [ "$TRANSFORM_CONFIG" = "PRO2" ] \
		|| [ "$TRANSFORM_CONFIG" = "LOCAL" ] \
		|| [ "$TRANSFORM_CONFIG" = "PRO3" ]; \
	then cd /source/tmp && curl --fail -o $TRANSFORM_CONFIG.domainmappings.json --header "PRIVATE-TOKEN:$CI_JOB_TOKEN" https://gitlab.nexdev.net/api/v4/projects/alpha-config%2Flogin-domains/repository/files/src%2F$TRANSFORM_CONFIG.domainmappings.json/raw?ref=182789-UpgradeNetCore \
		&& cd - && cp -r tmp/$TRANSFORM_CONFIG.domainmappings.json src/Fanex.Alpha.Before.Web/domainmappings.$TRANSFORM_CONFIG.json; fi

RUN if [ "$TRANSFORM_CONFIG" != "" ]; \
then \
  cp "src/Fanex.Alpha.Before.Web/domainmappings.$TRANSFORM_CONFIG.json" "src/Fanex.Alpha.Before.Web/domainmappings.json"; \
fi

RUN if [ "$TRANSFORM_CONFIG" = "LOCAL2" ] \
  || [ "$TRANSFORM_CONFIG" = "SIT2" ] \
  || [ "$TRANSFORM_CONFIG" = "UAT2" ] \
  || [ "$TRANSFORM_CONFIG" = "PRO2" ] \
  || [ "$TRANSFORM_CONFIG" = "AWS2" ] \
  || [ "$TRANSFORM_CONFIG" = "backupMNLi7" ]; \
then \
	cp "src/Fanex.Alpha.Before.Web/wwwroot/favicon2.ico" "src/Fanex.Alpha.Before.Web/wwwroot/favicon.ico"; \
  cp "src/Fanex.Alpha.Before.Web/bundleconfig2.json" "src/Fanex.Alpha.Before.Web/bundleconfig.json";\
fi

FROM hub.nexdev.net/base/node:lts as node-build
RUN npm install -g gulp
WORKDIR /source/src/Fanex.Alpha.Before.Web
RUN npm install
RUN gulp sass

FROM builder AS publish
WORKDIR /source/src/Fanex.Alpha.Before.Web
RUN dotnet publish -c release -o /app --self-contained false

FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0 AS final

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
COPY --from=publish /app ./
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Alpha.Before.Web.dll"]