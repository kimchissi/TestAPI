using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using APITest;
using Microsoft.AspNetCore.Mvc;
using TestAPI;

namespace PaymentApi.Controllers
{
    [Produces("application/json")]
    [Route("api/service/PaymentGateway")]
    public class ValuesController : Controller
    {
        DBConnect myDBConnect = new DBConnect();
        SqlCommand myDBCommand = new SqlCommand();
        //String SqlConnectString = "server=127.0.0.1,5555;Database=fa19_3342_tug91045;User id=tug91045;Password=ShuY4yoo";
        String SqlConnectString = "server=cis-mssql1.temple.edu;Database=fa19_3342_tug91045;User id=tug91045;Password=ShuY4yoo";

        [HttpPost("CreateVirtualWallet/{merchantAccountID}/{apiKey}")]
        public int CreateVirtualWallet([FromBody]AccountHolderInformation accountHolderInformation, int merchantAccountID, String apiKey)
        {
            VirtualWalletTestAPI wallet = new VirtualWalletTestAPI();
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CreateVirtualWallet";
                myDBCommand.Parameters.AddWithValue("@theEmail", accountHolderInformation.Email);
                myDBCommand.Parameters.AddWithValue("@theCardType", accountHolderInformation.CardType);
                myDBCommand.Parameters.AddWithValue("@theAccountNumber", accountHolderInformation.AccountNumber);
                SqlParameter outputParameter = new SqlParameter("@theVirtualWalletID", 0);
                outputParameter.Direction = ParameterDirection.Output;
                myDBCommand.Parameters.Add(outputParameter);
                myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
             
                int id = 0;
                id = Convert.ToInt32(myDBCommand.Parameters["@theVirtualWalletEmailID"].Value);
                return id;
                }
            
            return 0;

        }

        [HttpGet("GetTransactions/{virtualWalletEmail}/{merchantAccountID}/{apiKey}")]
        public List<TransactionsTestAPI> GetTransactions(int virtualWalletEmail, int merchantAccountID, String apiKey)
        {
            List<TransactionsTestAPI> list = new List<TransactionsTestAPI>();
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //checks if api key exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_GetTransactionsByMerchantAccountID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletEmail", virtualWalletEmail);
                myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
                DataSet transactions = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable transTable = transactions.Tables[0];
                    
                for (int i = 0; i < transTable.Rows.Count; i++)
                {
                    DataRow transRows = transTable.Rows[i];
                    TransactionsTestAPI transaction = new TransactionsTestAPI();
                    transaction.TransactionsID = Convert.ToInt32(transRows["TransactionsID"].ToString());
                    transaction.TransactionsType = transRows["TransactionsType"].ToString();
                    transaction.SendingVirtualWalletEmail = transRows["SendingVirtualWalletEmail"].ToString();
                    transaction.ReceivingVirtualWalletEmail = transRows["ReceivingVirtualWalletEmail"].ToString();
                    transaction.Amount = Convert.ToDouble(transRows["Amount"].ToString());
                    list.Add(transaction);
                }
                
            }
            return list;
        }

        [HttpPost]
        public void ProcessPayment(int senderWalletEmail, int receiverWalletEmail, double amount, int merchantAccountID, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists and matches Merchant Account ID
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckVirtualWalletAmount";
                myDBCommand.Parameters.AddWithValue("@theEmail", senderWalletEmail);
                DataSet senderWalletBalance = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable transTable = senderWalletBalance.Tables[0];
                double walletBalance = 0;
                for (int i = 0; i < transTable.Rows.Count; i++)
                {
                    DataRow transRows = transTable.Rows[i];
                    walletBalance = Convert.ToDouble(transRows["Amount"].ToString());
                }
                if (walletBalance >= amount)
                {
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_ProcessPayment";
                    myDBCommand.Parameters.AddWithValue("@theTransactionsType", "Payment");
                    myDBCommand.Parameters.AddWithValue("@theSendingVirtualWalletEmail", senderWalletEmail);
                    myDBCommand.Parameters.AddWithValue("@theReceivingVirtualWalletEmail", receiverWalletEmail);
                    myDBCommand.Parameters.AddWithValue("@theAmount", amount);
                    myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
                    myDBConnect.DoUpdateUsingCmdObj(myDBCommand);


                    //first get users id using their email
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_GetVirtualWalletIDByEmail";
                    myDBCommand.Parameters.AddWithValue("@thEmail", senderWalletEmail);
                    DataSet virtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                    DataTable virtualWalletIDTable = virtualWalletID.Tables[0];
                    int usersVirtualWalletID = 0;
                    for (int i = 0; i < virtualWalletIDTable.Rows.Count; i++)
                    {
                        DataRow transRows = virtualWalletIDTable.Rows[i];
                        usersVirtualWalletID = Convert.ToInt32(transRows["VirtualWalletID"].ToString());
                    }

                    
                    double newBalance = walletBalance - amount;

                    //update virtual wallets balance
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_SetAccountBalance";
                    myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", usersVirtualWalletID);
                    myDBCommand.Parameters.AddWithValue("@theBalance", newBalance);
                    myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
                }
            }
        }

        [HttpPut]
        public void UpdatePaymentAccount(String email, String cardType, String accountNumber, int merchantAccountID, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                //first get users id using their email
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_GetVirtualWalletIDByEmail";
                myDBCommand.Parameters.AddWithValue("@thEmail", email);
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
                myDBCommand.Parameters.AddWithValue("@theEmail", email);
                myDBCommand.Parameters.AddWithValue("@theCardType", cardType);
                myDBCommand.Parameters.AddWithValue("@theAccountNumber", accountNumber);
                myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
            }
        }

        [HttpPut]
        public void FundAccount(string email, double amount, int merchantAccountID, int apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKeyAndMerchantAccountID";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists and matches Merchant Account ID
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                //first get users id using their email
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_GetVirtualWalletIDByEmail";
                myDBCommand.Parameters.AddWithValue("@thEmail", email);
                DataSet virtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable virtualWalletIDTable = virtualWalletID.Tables[0];
                int usersVirtualWalletID = 0;
                for (int i = 0; i < virtualWalletIDTable.Rows.Count; i++)
                {
                    DataRow transRows = virtualWalletIDTable.Rows[i];
                    usersVirtualWalletID = Convert.ToInt32(transRows["VirtualWalletID"].ToString());
                }

                //then retrieve current balance and add new amount to it
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckVirtualWalletAmount";
                myDBCommand.Parameters.AddWithValue("@thEmail", email);
                DataSet virtualWalletBalance = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                DataTable virtualWalletBalanceTable = virtualWalletBalance.Tables[0];
                double usersBalance = 0;
                for (int i = 0; i < virtualWalletIDTable.Rows.Count; i++)
                {
                    DataRow transRows = virtualWalletBalanceTable.Rows[i];
                    usersBalance = Convert.ToDouble(transRows["Balance"].ToString());
                }
                double newBalance = usersBalance + amount;
                //update balance with new amount
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_SetAccountBalance";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", virtualWalletID);
                myDBCommand.Parameters.AddWithValue("@theAmount", newBalance);
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
            }
        }


    }
}
