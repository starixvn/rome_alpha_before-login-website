# escape=`
FROM hub.nexdev.net/base/rocketor/buildservice AS builder

ARG DEPLOY_ENV
ARG ROCKETOR_API
ARG REDMINE_IDS

SHELL ["cmd", "/S", "/C"]

WORKDIR /build

# Copy files *.sln, *.csproj, packages.config to restore nuget packages
COPY *.sln .
COPY src src\

RUN nuget.exe restore Fanex.Alpha.Before.sln

RUN MSBuild.exe "src\Fanex.Alpha.Before.Web\Fanex.Alpha.Before.Web.csproj" `
    /t:Build;WebPublish /m `
    /p:DebugType=None `
    /p:Configuration=Release `
    /p:WebPublishMethod=FileSystem `
    /p:TransformWebConfigEnabled=False `
    /p:LocalizedBuild=true `
    /p:AutoParameterizationWebConfigConnectionStrings=False `
    /p:MarkWebConfigAssistFilesAsExclude=false `
    /p:publishUrl="C:\BeforeLogin" `
    /consoleloggerparameters:ErrorsOnly

WORKDIR C:\BeforeLogin

# Change SP Version
RUN `
    if defined REDMINE_IDS `
    FOR %P IN ( `
        Configurations\PRO.StoreProcedure\DBCust.CustDB.BODBDownlineNet.xml, `
		Configurations\PRO.StoreProcedure\DBCUST.IPDB.BODBDownlineNet.xml, `
		Configurations\PRO.StoreProcedure\DBONUSER.ON_USER.BODBDownlineNet.xml `
    ) `
    DO curl --fail -o C:\alpha-login-sp-result.xml --form file=@"%P" --form "redmineIds=%REDMINE_IDS%" -X POST %ROCKETOR_API% `
        && xcopy C:\alpha-login-sp-result.xml "%P" /Y

# Transform config
RUN echo "%DEPLOY_ENV%" `
    && xcopy "Configurations\WebDeployment.ConfigTransform\%DEPLOY_ENV%.Web.config" "Web.config" /Y `
    && xcopy "Configurations\WebDeployment.ConfigTransform\%DEPLOY_ENV%.ConnectionStrings.config" "ConnectionStrings.config" /Y `
	&& xcopy "Configurations\WebDeployment.ConfigTransform\%DEPLOY_ENV%.DomainGroupSection.config" "DomainGroupSection.config" /Y `
    && xcopy "Configurations\WebDeployment.ConfigTransform\%DEPLOY_ENV%.LimtedCountryAccessSection.config" "LimtedCountryAccessSection.config" /Y `
	&& xcopy "Configurations\WebDeployment.ConfigTransform\%DEPLOY_ENV%.Web.Setting.config" "Web.Setting.config" /Y `
	&& xcopy "App_Data\activitytracking.%DEPLOY_ENV%.json" "App_Data\activitytracking.json" /Y 
	

FROM hub.nexdev.net/base/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019 AS base

RUN ["powershell", "New-Item", "C:\\inetpub\\BeforeLogin"]

COPY --from=builder C:\BeforeLogin .

RUN dir .\bin\

ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]