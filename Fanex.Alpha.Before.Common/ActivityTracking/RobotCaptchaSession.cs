namespace Fanex.Alpha.Before.Common.ActivityTracking
{
    using System.Web;
    using Fanex.ActivityTracking.Models;
    using Fanex.Alpha.Before.Common.Configurations;

    public sealed class RobotCaptchaSession : IUserSession
    {
        public int AdminId => 0;

        public string AdminName => null;

        public string SessionID => HttpContext.Current.Session.SessionID;

        public string Language => AppSession.Language;

        public int CustId => AppSession.CustId;

        public string CustName => AppSession.Username;

        public int SubAccId => AppSession.SubAccId;

        public string SubUsername => AppSession.SubAccName;

        public string Nickname => AppSession.Nickname;

        public string SiteId => AppSession.Site;

        public byte LevelId => AppSession.RoleId;

        public string RobotResult => AppSession.RobotResult;

        public int RobotCaptchaFailedCount => AppSession.RobotCaptchaFailedCount;
    }
}
