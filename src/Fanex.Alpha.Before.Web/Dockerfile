
FROM hub.nexdev.net/base/dotnet/core/sdk:7.0 AS builder
WORKDIR /source

COPY "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj" "src/Fanex.Alpha.Before/Fanex.Alpha.Before.csproj"
COPY "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj" "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj"

COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "src/Fanex.Alpha.Before.Web/Fanex.Alpha.Before.Web.csproj"

COPY . .
WORKDIR /source/src/Fanex.Alpha.Before.Web

#Installing nodejs
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_18.x | bash \
    && apt-get install nodejs -yq

#Installing gulp
RUN npm install -g gulp
RUN npm install
RUN gulp sass

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
COPY --from=builder /app ./
ENV TZ="America/La_Paz"


ENTRYPOINT ["dotnet", "Fanex.Alpha.Before.Web.dll"]