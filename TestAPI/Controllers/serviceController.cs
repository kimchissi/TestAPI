using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APITest;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAPI;

namespace PaymentAPI.Controllers
{
    //[Produces("application/json")]
    [Route("api/PaymentGateway")]
    public class serviceController : Controller
    {
        DBConnect myDBConnect = new DBConnect();
        SqlCommand myDBCommand = new SqlCommand();
        //String SqlConnectString = "server=127.0.0.1,5555;Database=fa19_3342_tug91045;User id=tug91045;Password=ShuY4yoo";
        //String SqlConnectString = "server=cis-mssql1.temple.edu;Database=fa19_3342_tug91045;User id=tug91045;Password=ShuY4yoo";

        [HttpPost("Create/{merchantAccountID}/{apiKey}")]
        public string CreateVirtualWallet([FromBody]AccountHolderInformation accountHolderInformation, int merchantAccountID, String apiKey)
        {
            VirtualWalletTestAPI wallet = new VirtualWalletTestAPI();
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
   
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey != null && selectedAPIKey.Tables[0].Rows.Count > 0 )
            {
                
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandText = "tp_CreateVirtualWallet";
                myDBCommand.Parameters.AddWithValue("@theEmail", accountHolderInformation.Email);
                myDBCommand.Parameters.AddWithValue("@theCardType", accountHolderInformation.CardType);
                myDBCommand.Parameters.AddWithValue("@theAccountNumber", accountHolderInformation.AccountNumber);
                //SqlParameter outputParameter = new SqlParameter("@theVirtualWalletID", 0);
                myDBCommand.Parameters.Add("@theVirtualWalletID", SqlDbType.Int).Direction = ParameterDirection.Output;
                //outputParameter.Direction = ParameterDirection.Output;
                //myDBCommand.Parameters.Add(outputParameter);
                myDBConnect.DoUpdateUsingCmdObj(myDBCommand);

                
                int id = Convert.ToInt32(myDBCommand.Parameters["@theVirtualWalletID"].Value);
                return "Created new Virtual Wallet with id: " + id;
            }

            return "API Key and Merchant Account ID do not match";

        }

        [HttpGet("GetTransactions/{email}/{merchantAccountID}/{apiKey}")]
        public List<TransactionsTestAPI> GetTransactions(string email, int merchantAccountID, String apiKey)
        {
            List<TransactionsTestAPI> list = new List<TransactionsTestAPI>();
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //checks if api key exists
            if (selectedAPIKey != null && selectedAPIKey.Tables[0].Rows.Count > 0)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_GetTransactionsByMerchantAccountID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletEmail", email);
                myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
                DataSet transactions = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable transTable = transactions.Tables[0];

                foreach (DataTable table in transactions.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        DataRow transRows = row;
                        TransactionsTestAPI transaction = new TransactionsTestAPI();
                        transaction.TransactionsID = Convert.ToInt32(transRows["TransactionsID"].ToString());
                        transaction.TransactionsType = transRows["TransactionsType"].ToString();
                        transaction.SendingVirtualWalletEmail = transRows["SendingVirtualWalletEmail"].ToString();
                        transaction.ReceivingVirtualWalletEmail = transRows["ReceivingVirtualWalletEmail"].ToString();
                        transaction.Amount = Convert.ToDouble(transRows["Amount"].ToString());
                        list.Add(transaction);
                    }
                }
            }
            return list;
        }

        [HttpPost("Process/{email}/{receiverEmail}/{type}/{amount}/{merchantAccountID}/{apiKey}")]
        public string ProcessPayment(string email, string receiverEmail, string type, double amount, int merchantAccountID, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists and matches Merchant Account ID
            if (selectedAPIKey != null && selectedAPIKey.Tables[0].Rows.Count > 0)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckVirtualWalletAmount";
                myDBCommand.Parameters.AddWithValue("@theEmail", email);
                DataSet senderWalletBalance = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable transTable = senderWalletBalance.Tables[0];
                double walletBalance = 0;
                for (int i = 0; i < transTable.Rows.Count; i++)
                {
                    DataRow transRows = transTable.Rows[i];
                    walletBalance = Convert.ToDouble(transRows["Balance"].ToString());
                }
                if (walletBalance >= amount)
                {
                    //create transaction
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_ProcessPayment";
                    myDBCommand.Parameters.AddWithValue("@theTransactionsType", type);
                    myDBCommand.Parameters.AddWithValue("@theSenderWalletEmail", email);
                    myDBCommand.Parameters.AddWithValue("@theReceiverWalletEmail", receiverEmail);
                    myDBCommand.Parameters.AddWithValue("@theAmount", amount);
                    myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
                    myDBConnect.DoUpdateUsingCmdObj(myDBCommand);


                    //first get users id using their email
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_GetVirtualWalletIDByEmail";
                    myDBCommand.Parameters.AddWithValue("@theEmail", email);
                    DataSet virtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                    DataTable virtualWalletIDTable = virtualWalletID.Tables[0];
                    int usersVirtualWalletID = 0;
                    for (int i = 0; i < virtualWalletIDTable.Rows.Count; i++)
                    {
                        DataRow transRows = virtualWalletIDTable.Rows[i];
                        usersVirtualWalletID = Convert.ToInt32(transRows["VirtualWalletID"].ToString());
                    }

                    //second get other users id using their email
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_GetVirtualWalletIDByEmail";
                    myDBCommand.Parameters.AddWithValue("@theEmail", email);
                    DataSet secondVirtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                    DataTable secondVirtualWalletIDTable = virtualWalletID.Tables[0];
                    int secondUsersVirtualWalletID = 0;
                    for (int i = 0; i < virtualWalletIDTable.Rows.Count; i++)
                    {
                        DataRow transRows = virtualWalletIDTable.Rows[i];
                        secondUsersVirtualWalletID = Convert.ToInt32(transRows["VirtualWalletID"].ToString());
                    }
                    

                    //update virtual wallets balance
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_SetAccountBalance";
                    myDBCommand.Parameters.AddWithValue("@theSenderEmail", email);
                    myDBCommand.Parameters.AddWithValue("@theReceiverEmail", receiverEmail);
                    myDBCommand.Parameters.AddWithValue("@theAmount", amount);
                    myDBConnect.DoUpdateUsingCmdObj(myDBCommand);

                    return "Payment successfully processed";
                }
                return "Insufficient funds";
            }
            return "API Key and Merchant Account ID do not match";
        }

        [HttpPut("Update/{merchantAccountID}/{apiKey}")]
        public string UpdatePaymentAccount([FromBody]AccountHolderInformation accountHolderInformation, int merchantAccountID, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey != null && selectedAPIKey.Tables[0].Rows.Count > 0)
            {
                //first get users id using their email
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_GetVirtualWalletIDByEmail";
                myDBCommand.Parameters.AddWithValue("@theEmail", accountHolderInformation.Email);
                DataSet virtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable virtualWalletIDTable = virtualWalletID.Tables[0];
                int usersVirtualWalletID = 0;
                for (int i = 0; i < virtualWalletIDTable.Rows.Count; i++)
                {
                    DataRow transRows = virtualWalletIDTable.Rows[i];
                    usersVirtualWalletID = Convert.ToInt32(transRows["VirtualWalletID"].ToString());
                }

                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_UpdatePaymentAccount";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", usersVirtualWalletID);
                myDBCommand.Parameters.AddWithValue("@theEmail", accountHolderInformation.Email);
                myDBCommand.Parameters.AddWithValue("@theCardType", accountHolderInformation.CardType);
                myDBCommand.Parameters.AddWithValue("@theAccountNumber", accountHolderInformation.AccountNumber);
                myDBConnect.DoUpdateUsingCmdObj(myDBCommand);

                return "updated card type to: " + accountHolderInformation.CardType + " updated account number to: " + accountHolderInformation.AccountNumber + " for user: " + accountHolderInformation.Email;
            }
            return "API Key and Merchant Account ID do not match";
        }

        [HttpPut("Fund/{email}/{amount}/{merchantAccountID}/{apiKey}")]
        public String FundAccount(string email, double amount, int merchantAccountID, string apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);

            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists and matches Merchant Account ID
            if (selectedAPIKey.Tables[0].Rows.Count > 0)
            {
                //first get users id using their email
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_GetVirtualWalletIDByEmail";
                myDBCommand.Parameters.AddWithValue("@theEmail", email);
                DataSet virtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable virtualWalletIDTable = virtualWalletID.Tables[0];
                int usersVirtualWalletID = 0;
                for (int i = 0; i < virtualWalletIDTable.Rows.Count; i++)
                {
                    DataRow transRows = virtualWalletIDTable.Rows[i];
                    usersVirtualWalletID = Convert.ToInt32(transRows["VirtualWalletID"].ToString());
                }

                //update balance with new amount
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_FundAccounts";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", usersVirtualWalletID);
                myDBCommand.Parameters.AddWithValue("@theAmount", amount);
                myDBConnect.DoUpdateUsingCmdObj(myDBCommand);

                //create record of transaction
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_LogAccountFunding";
                myDBCommand.Parameters.AddWithValue("@theTransactionsType", "Fund Account");
                myDBCommand.Parameters.AddWithValue("@theSendingVirtualWalletEmail", "Financial Institution");
                myDBCommand.Parameters.AddWithValue("@theReceivingVirtualWalletEmail", email);
                myDBCommand.Parameters.AddWithValue("@theAmount", amount);
                myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
                myDBConnect.DoUpdateUsingCmdObj(myDBCommand);

                return "Added " + amount + " to wallet.";
            }
            return "API Key and Merchant Account ID do not match";
        }
    }
}