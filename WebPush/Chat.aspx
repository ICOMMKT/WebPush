<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="WebPush.Chat" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Signalr Chat Messenger</title>
    <script src="Scripts/jquery-1.6.4.min.js" type="text/javascript"></script>
    <script src="Scripts/jquery.signalR-1.0.0.js" type="text/javascript"></script>
    <script src="signalr/hubs" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">

    <script type="text/javascript">
        $(function () {

            var IWannaChat = $.connection.iComMktHub;

            IWannaChat.client.addMessage = function (message) {
                $('#listMessages').append('<li>' + message + '</li>');
            };

            IWannaChat.client.showAlerts = function (oAlerts, test) {
                alert(oAlerts);
            };
            $("#SendMessage").click(function () {
                IWannaChat.server.send($('#txtMessage').val(), $('#toUsr').val());
            });

            $('#btnRegistrar').click(function (e) {
                //e.preventDefault();                
                IWannaChat.server.registerClient($("#txtRegister").val(), "ODQtMTc2LWljb21t0:MTc20", "1", "10");
                $("#register").hide("fast");
                $("#chat").show("fast");
            });

            $.connection.hub.start();
        });
    </script>

    <div>
        <div id="register">
            <input type="text" id="txtRegister" name="txtRegister" />
            <input type="button" id="btnRegistrar" value="Registrar" />
            
        </div>
        <div id="chat" style="display: none">  
            <label>To (Username): </label>
            <input type="text" id="toUsr" /><br />              
            <input type="text" id="txtMessage" />
            <input type="button" id="SendMessage" value="broadcast" />
            <ul id="listMessages">
            </ul>
        </div>
    </div>
    </form>
</body>
</html>
