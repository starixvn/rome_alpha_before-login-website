<%@ Page Language="C#" ValidateRequest="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
     protected string[] GetClientIP(){
         string IP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.ServerVariables["REMOTE_ADDR"];
         string[] IPs = string.IsNullOrEmpty(IP)?new string[]{""}:IP.Split(',');

         return IPs;
     }

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
Server IP: <%=Request.ServerVariables["LOCAL_ADDR"] %><br />
HTTP_X_FORWARDED_FOR: <%=Request.ServerVariables["HTTP_X_FORWARDED_FOR"]%><br />
REMOTE_ADDR: <%=Request.ServerVariables["REMOTE_ADDR"]%><br />
<%string[] IPs = GetClientIP(); %>
Client IP:<%=IPs[0] %><br />
<%
    if (IPs.Length > 1)
    {
        for (int i = 1; i < IPs.Length; ++i)
        {
            Response.Write("Proxy " + i + ": " + IPs[i] + "<br/>");
        }
    } %>
<hr />
<strong>All headers:</strong><br />
<%
    for (int i = 0; i < Request.Headers.Count; i++)
    {
        string key = Request.ServerVariables.GetKey(i);
        string value = Request.ServerVariables.Get(i);
        base.Response.Write(key + " = " + value + "<br/>");
    }
     %>
</body>
</html>
