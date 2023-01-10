
FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /source

COPY "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj" "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj"
COPY "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj" "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj"

COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj"

COPY . .
WORKDIR /source/src/Fanex.Alpha.Before.Web

FROM hub.nexdev.net/base/node:17.9-alpine3.14 AS deps
RUN npm install -g gulp
RUN npm install
RUN gulp

FROM builder as publish
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
FROM hub.nexdev.net/base/dotnet/core/aspnet:7.0
WORKDIR /app
COPY --from=publish /app ./
ENV TZ="America/La_Paz"


ENTRYPOINT ["dotnet", "Fanex.Alpha.Before.Web.dll"]