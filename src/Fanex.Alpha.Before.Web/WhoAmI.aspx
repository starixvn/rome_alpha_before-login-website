<%@ Page Language="C#" EnableSessionState="False" %>

<html>
<head runat="server">
    <title>Who Am I?</title>
</head>
<body>
    <div>
        Web Name:
        <%= Request.ServerVariables["Server_Name"] %></div>
    <div class="port">
        Port:
        <%= Request.Url.Port %></div>
    <div class="backend">
        Server IP:
        <%= Request.ServerVariables["LOCAL_ADDR"]%></div>
    <div class="servertime">
        Server Time:
        <%= DateTime.Now%></div>
    <div class="haproxy">
        Client IP:
        <%= Request.UserHostAddress%></div>    
    <div>
         HTTP_X_FORWARDED_FOR:
        <%= Request.ServerVariables["HTTP_X_FORWARDED_FOR"]%>
    </div>
    <div>
         HTTP_X_REAL_IP:
        <%= Request.ServerVariables["HTTP_X_REAL_IP"]%>
    </div>
    <div>
         HTTP_VIA:
        <%= Request.ServerVariables["HTTP_VIA"]%>
    </div>
    <div>
         HTTP_PROXY_CONNECTION:
        <%= Request.ServerVariables["HTTP_PROXY_CONNECTION"]%>
    </div>
</body>
</html>
