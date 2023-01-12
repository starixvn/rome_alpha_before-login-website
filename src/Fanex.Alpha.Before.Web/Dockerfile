FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /source
COPY "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj" "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj"
COPY "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj" "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj"
COPY "NuGet.config" "NuGet.config"
RUN dotnet restore --configfile "NuGet.config" "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj"

COPY . .

ARG TRANSFORM_CONFIG
RUN echo $TRANSFORM_CONFIG

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
RUN dotnet publish -c release -o /app --self-contained false --no-restore

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
FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app ./
ENV TZ="America/La_Paz"

ENTRYPOINT ["dotnet", "Fanex.Alpha.Before.Web.dll"]