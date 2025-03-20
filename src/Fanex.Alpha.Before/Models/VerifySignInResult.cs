namespace Fanex.Alpha.Before.Models;

public class VerifySignInResult
{
    public int CustId
    {
        get;
        set;
    }

    public int SubAccId
    {
        get;
        set;
    }

    public DateTime LoginTime
    {
        get;
        set;
    }

    public DateTime LastTimeToChangePassword
    {
        get;
        set;
    }

    public string Nickname
    {
        get;
        set;
    }

    public string Username
    {
        get;
        set;
    }

    public bool IsNickname
    {
        get;
        set;
    }

    public int SuperId { get; set; }

    public string Site { get; set; }

    public byte RoleId { get; set; }

    public string SubUserName { get; set; }

    public int ErrorCode
    {
        get;
        set;
    }

    public string ErrorMessage
    {
        get;
        set;
    }

    public int ServerId { get; set; }

    public bool IsKickoutDueToProtectedMode { get; set; }

    public int FailedCount { get; set; }
}
