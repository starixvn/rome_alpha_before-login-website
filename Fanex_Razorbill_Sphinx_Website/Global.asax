<%@ Application Language="C#" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Routing" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Web" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Web.SessionState" %>
<%@ Import Namespace="System.Web.Configuration" %>

<%@ Import Namespace="Fanex.Razorbill.Sphinx.Web" %>
<%@ Import Namespace="Sunplus.Agent.Utilities" %>
<%@ Import Namespace="Sunplus.Agent.Model.Entities" %>
<%@ Import Namespace="Sunplus.Agent.Model.BL.Authorization" %>
<%@ Import Namespace="Sunplus.Agent.Configurations" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        AppSettings.LoadComplete += new AppSettings.UpdateEventHandler(AppSettings_LoadComplete);
        AppSettings.Configuration.Load();

        // Code that runs on application startup
        Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();

        AreaRegistration.RegisterAllAreas();

        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RegisterRoutes(RouteTable.Routes);
    }

    private static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
        );
    } 

    void AppSettings_LoadComplete(object sender, bool error)
    {
        if (error) return;
        
        string[] keys = AppSettings.Configuration.GetKeys();
        foreach (string key in keys)
        {
            ConfigurationManager.AppSettings[key] = AppSettings.Configuration.Get(key);
        }

        TplCore.TplSettings.UrlVersion = AppSettings.Configuration.Get("JsVersion");
    }

    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    {
        Fanex.SessionPartition.SessionIdManager.HandleUnconnectedSession();
        // Code that runs when an unhandled error occurs
        Exception ex = Server.GetLastError().GetBaseException();

        if (ex is HttpRequestValidationException)
        {
            return;
        }

        if (ex is HttpException)
        {
            return;
        }
        
        string message = ex.Message +
                        "\r\nSOURCE: " + ex.Source +
                        "\r\nDOMAIN: " + HttpContext.Current.Request.Headers["Host"] +
                        "\r\nREFERRER: " + HttpContext.Current.Request.UrlReferrer +
                        "\r\nFORM: " + Request.Form.ToString() +
                        "\r\nQUERYSTRING: " + Request.QueryString.ToString() +
                        "\r\nTARGETSITE: " + ex.TargetSite +
                        "\r\nSTACKTRACE: " + ex.StackTrace;

        if (HttpContext.Current.Handler is IRequiresSessionState)
        {
            IUserSession session = UserSession.Current;
            message += "\r\nCUSTID:" + session.CustId;
        }

        Logs.Write(message);

        if (!Sunplus.Agent.Model.BL.Definitions.AppSettings.LocalTest)
        {
            Server.ClearError();
            Server.Transfer("~/Error.aspx");
        }
    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }
       
</script>
