using System;
using Sunplus.Agent.Model.Services;

public partial class UnderMaintenance : System.Web.UI.Page
{
    // Under maintenance checking
    static object _locker = new object();
    static DateTime _lastCheckUM = DateTime.MinValue;
    static bool _isUM = false;

    protected static string startTime = string.Empty;
    protected static string endTime = string.Empty;

    protected override void OnLoad(EventArgs e)
    {
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

        if (!_isUM)
        {
            Server.Transfer("~/SignIn.aspx");
            return;
        }

        Session.Abandon();
        base.OnLoad(e);
    }
}
