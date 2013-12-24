using System;

public partial class Error : System.Web.UI.Page
{
    protected string errorMessage = null;

    public string GetBaseUrl()
    {
        string hostName = Request.Headers["Host"];
        string appPath = Request.ApplicationPath;

        string url = /*Protocol + hostName +*/ appPath;

        if (appPath.Length > 1)
        {
            url += '/';
        }

        return url;
    }

    protected override void OnLoad(EventArgs e)
    {
        string errCode = Request["err"];
	
        switch (errCode)
        {
            case "403":
                this.errorMessage = Resources.Lang.HttpError_403;
                break;
            case "404":
                this.errorMessage = Resources.Lang.HttpError_404;
                break;
            default:
                this.errorMessage = Resources.Lang.HttpError_Default;
                break;
        }

        base.OnLoad(e);
    }
}
