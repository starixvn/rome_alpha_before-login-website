# escape=`
FROM hub.nexdev.net/base/rocketor/buildservice AS builder

ARG ROCKETOR_API
ARG REDMINE_IDS
ARG TRANSFORM_CONFIG

SHELL ["cmd", "/S", "/C"]

WORKDIR /build

# Copy files *.sln, *.csproj, packages.config to restore nuget packages
COPY *.sln .
COPY src src\
COPY packages packages\

# Build project
RUN `
	nuget.exe restore Fanex.Alpha.Before.sln `
	&& MSBuild.exe "src\Fanex.Alpha.Before.Web\Fanex.Alpha.Before.Web.csproj" `
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

RUN FOR %P IN ( `
		LOCAL2, `
		SIT2, `
		UAT2, `
		PRO2, `
		AWS2, `
		backupMNLi7, `
    ) `
    DO ( `
        if "%P" == "%TRANSFORM_CONFIG%"; `
		echo "Copying files..." && xcopy "favicon2.ico" "favicon.ico" /Y `
		&& robocopy "assets2" "assets" /e /mir `
    )

# Change SP Version
RUN if defined REDMINE_IDS `
    FOR %P IN ( `
        Configurations\StoreProcedure\DBCust.CustDB.BODBDownlineNet.xml, `
		Configurations\StoreProcedure\DBCUST.IPDB.BODBDownlineNet.xml, `
		Configurations\StoreProcedure\DBONUSER.ON_USER.BODBDownlineNet.xml `
    ) `
    DO ( `
        curl --fail -o C:\alpha-login-sp-result.xml --form file=@"%P" --form "redmineIds=%REDMINE_IDS%" -X POST %ROCKETOR_API% `
        && xcopy C:\alpha-login-sp-result.xml "%P" /Y `
    )

# Get private domains from git
RUN FOR %F IN ( `
        PRO, `
		PRO2, `
		PRO3 `
    ) `
    DO ( `
        if "%F" == "%TRANSFORM_CONFIG%" `
            git clone git@gitlab.nexdev.net:alpha-config/login-domains.git C:\login-domains `
            && xcopy "C:\login-domains\src\%TRANSFORM_CONFIG%.DomainGroupSection.config" "Configurations\WebDeployment.ConfigTransform\%TRANSFORM_CONFIG%.DomainGroupSection.config" /Y `
    )

# Transform config
RUN echo "%TRANSFORM_CONFIG%" `
    && xcopy "Configurations\WebDeployment.ConfigTransform\%TRANSFORM_CONFIG%.Web.config" "Web.config" /Y `
    && xcopy "Configurations\WebDeployment.ConfigTransform\%TRANSFORM_CONFIG%.ConnectionStrings.config" "ConnectionStrings.config" /Y `
	&& xcopy "Configurations\WebDeployment.ConfigTransform\%TRANSFORM_CONFIG%.DomainGroupSection.config" "DomainGroupSection.config" /Y `
    && xcopy "Configurations\WebDeployment.ConfigTransform\%TRANSFORM_CONFIG%.LimtedCountryAccessSection.config" "LimtedCountryAccessSection.config" /Y `
	&& xcopy "Configurations\WebDeployment.ConfigTransform\%TRANSFORM_CONFIG%.Web.Setting.config" "Web.Setting.config" /Y `
	&& xcopy "App_Data\activitytracking.%TRANSFORM_CONFIG%.json" "App_Data\activitytracking.json" /Y 
	
FROM hub.nexdev.net/base/dotnet/framework/aspnet:4.7.2-windowsservercore-ltsc2019 AS base

COPY --from=builder C:\BeforeLogin .

RUN dir .\bin\

ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]