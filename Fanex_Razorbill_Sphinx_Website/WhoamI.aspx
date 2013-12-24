<%@ Page Language="C#" EnableSessionState="False" %>

<%@ Import Namespace="System.Web" %>
<html xmlns="http://www.w3.org/1999/xhml">
<head id="Head1" runat="server">
    <title>Who am i?</title>

    <script type="text/C#" runat="server">
        protected string _s = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            _s += "WebName: " + Request.ServerVariables["Server_Name"] + "<br/>";            
            _s += "Server IP : " + Request.ServerVariables["LOCAL_ADDR"].ToString() + "<br/>";
            _s += "Port : " + Request.ServerVariables["SERVER_PORT"].ToString() + "<br/>";
            _s += "Now : " + DateTime.Now.ToString() + "<br/>";
        }
    </script>

</head>
<body style="font-size: 16px; background-color: #ffffff;">
    <%Response.Write(_s); %>
</body>
</html>
