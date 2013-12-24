<%@ Page Language="C#" %>

<%
    if (Session["time"] == null)
        Session["time"] = DateTime.Now;
    Response.Write("<h2>Session DateTime: " + Session["time"] + "</h2>");
       %>
<h4>Message from server 02</h4>
<hr />
<%
Response.Write("<h3>State Server Status</h5>");
    foreach (string str in Fanex.SessionPartition.StateServers.ServersStatus)
    {
        Response.Write("<h5>" + str + "</h5>");
    }
%>