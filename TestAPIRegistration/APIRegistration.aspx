<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="APIRegistration.aspx.cs" Inherits="TestAPIRegistration.APIRegistration" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Payment Process API Registration</h1>
            <asp:Label ID="lblMerchantName" runat="server">Merchant Name: </asp:Label>
            <asp:TextBox ID="txtMerchantName" runat="server"></asp:TextBox>
            <br /><br />
            <asp:Label ID="lblUserName" runat="server">User Name: </asp:Label>
            <asp:TextBox ID="txtUserName" runat="server"></asp:TextBox>
            <br /><br />
            <asp:Label ID="lblPassword" runat="server">Password: </asp:Label>
            <asp:TextBox ID="txtPassword" runat="server"></asp:TextBox>
            <br /><br />
            <asp:Label ID="lblPasswordConfirm" runat="server">Confirm Password: </asp:Label>
            <asp:TextBox ID="txtPasswordConfirm" runat="server"></asp:TextBox>
            <br /><br />
            <asp:Button ID="btnSubmit" runat="server" Text="Submit"/>
        </div>
    </form>
</body>
</html>
