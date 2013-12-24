<%@ Page Language="C#" EnableSessionState="False" AutoEventWireup="false" CodeFile="Error.aspx.cs" Inherits="Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Error!!!!!</title>
</head>
<body>
    <center>
        <div style="height: 150px; width: 150px; margin-top: 100px; background: url(<%= GetBaseUrl() + "_GlobalResources/Images/error2.png" %>) no-repeat">
            &nbsp;
        </div>
        <div style="width: 90%;">
            <h3 style="color: #555">
                <%= errorMessage %>
            </h3>
        </div>
    </center>
</body>
</html>
