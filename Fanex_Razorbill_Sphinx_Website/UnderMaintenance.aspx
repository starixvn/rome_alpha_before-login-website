<%@ Page Language="C#" AutoEventWireup="false" EnableViewState="false" EnableSessionState="False" CodeFile="UnderMaintenance.aspx.cs" Inherits="UnderMaintenance" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Under Maintenance</title>
    <style type="text/css">
        body
        {
            margin-top: 10px;
            font-family: Arial;
            font-size: 15px;
            text-align: center;
            letter-spacing: 1px;
            line-height: 17px;
            cursor: default;
        }
        .text_time
        {
            color: #F00;
            font-weight: bold;
            text-align: center;
            width: 100%;
            left: 00px;
            right: 0px;
            position: absolute;
        }
        .contain
        {
            background: url(GlobalResources/Images/UnderMaintenance.gif) no-repeat scroll center center;
            width: 90%;
            left: 0px;
            right: 0px;
            top: 0px;
            height: 286px;
            position: absolute;
        }
    </style>
</head>
<body>
    <div class="contain">
        <div class="text_time" style="margin-top: 100px; padding-left: 9px; top: 0px;">
            <%=startTime%>
            <font color="#555">until</font>
            <%=endTime%></div>
        <div class="text_time" style="margin-top: 77px; padding-left: 85px; top: 118px;">
            <%=startTime%>
            <font color="#555">至</font>
            <%=endTime%></div>
    </div>
</body>
</html>
