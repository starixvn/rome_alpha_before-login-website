<%@ Page Language="C#" AutoEventWireup="false" EnableSessionState="False" %>
<%@ Import Namespace="Sunplus.Agent.Model.Services" %>
<%@ Import Namespace="Sunplus.Agent.Model.BL.Definitions" %>

<script runat="server">

    // Under maintenance checking
    static object _locker = new object();
    static DateTime _lastCheckUM = DateTime.MinValue;
    static bool _isUM = false;  
   
</script>
<% 

    // Check under maintenance time
    string startTime = string.Empty, endTime = string.Empty;
    // Double check and lock
    if (DateTime.Now.Subtract(_lastCheckUM) > TimeSpan.FromSeconds(5)) // Check down time each 5 seconds
    {
        lock (_locker)
        {
            if (DateTime.Now.Subtract(_lastCheckUM) > TimeSpan.FromSeconds(5))
            {

                _isUM = MaintenanceService.GetDownTime(out startTime, out endTime);
                _lastCheckUM = DateTime.Now;
            }
        }
    }

    if (_isUM)
    {
        Server.Transfer("~/UnderMaintenance.aspx");
        return;
    }
    else
    {
        Response.Redirect("SignIn.aspx", true);
    }

%>