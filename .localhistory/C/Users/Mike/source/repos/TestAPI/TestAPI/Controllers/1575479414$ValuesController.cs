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
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        DBConnect myDBConnect = new DBConnect();
        SqlCommand myDBCommand = new SqlCommand();
        //String SqlConnectString = "server=127.0.0.1,5555;Database=fa19_3344_tug91045;User id=tug91045;Password=ShuY4yoo";
        String SqlConnectString = "server=cis-mssql1.temple.edu;Database=fa19_3344_tug91045;User id=tug91045;Password=ShuY4yoo";

        [HttpPost("CreateVirtualWallet")]
        public void CreateVirtualWallet(String name, String bankAccountType, String bankAccountNumber, int merchantAccountID, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKey";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckForMerchantAccountID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", merchantAccountID);
                DataSet selectedVirtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                //check if merchant account exists
                if (selectedVirtualWalletID.Tables.Count > 0 && selectedVirtualWalletID != null)
                {
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_CreateVirtualWallet";
                    myDBCommand.Parameters.AddWithValue("@theName", name);
                    myDBCommand.Parameters.AddWithValue("@theBankAccountType", bankAccountType);
                    myDBCommand.Parameters.AddWithValue("@theBankAccountNumber", bankAccountNumber);
                    myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
                }
            }
        }

        [HttpGet("GetTransactions")]
        public List<TransactionsTestAPI> GetTransactions(int virtualWalletID, int merchantAccountID, String apiKey)
        {
            List<TransactionsTestAPI> list = new List<TransactionsTestAPI>();
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKey";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //checks if api key exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckForVirtualWalletID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", virtualWalletID);
                DataSet selectedVirtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                //checks if virtual wallet id exists
                if (selectedVirtualWalletID.Tables.Count > 0 && selectedVirtualWalletID != null)
                {
                    
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_GetTransactionsByMerchantAccountID";
                    myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", virtualWalletID);
                    myDBCommand.Parameters.AddWithValue("@theMerchantAccountID", merchantAccountID);
                    DataSet transactions = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                    DataTable transTable = transactions.Tables[0];
                    
                    for (int i = 0; i < transTable.Rows.Count; i++)
                    {
                        DataRow transRows = transTable.Rows[i];
                        TransactionsTestAPI transaction = new TransactionsTestAPI();
                        transaction.TransactionsID = Convert.ToInt32(transRows["TransactionsID"].ToString());
                        transaction.TransactionsType = transRows["TransactionsType"].ToString();
                        transaction.SendingVirtualWalletID = transRows["SendingVirtualWalletID"].ToString();
                        transaction.ReceivingVirtualWalletID = transRows["ReceivingVirtualWalletID"].ToString();
                        transaction.Amount = Convert.ToDouble(transRows["Amount"].ToString());
                        list.Add(transaction);
                    }
                }
            }
            return list;
        }

        [HttpPost]
        public void ProcessPayment(int senderWalletID, int receiverWalletID, double amount, String type, int merchantAccountID, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKey";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckForMerchantAccountID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", merchantAccountID);
                DataSet selectedVirtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                //check if merchant account exists
                if (selectedVirtualWalletID.Tables.Count > 0 && selectedVirtualWalletID != null)
                {
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_CheckVirtualWalletAmount";
                    myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", senderWalletID);
                    DataSet senderWalletBalance = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                    DataTable transTable = senderWalletBalance.Tables[0];
                    Double walletBalance = 0;
                    for (int i = 0; i < transTable.Rows.Count; i++)
                    {
                        DataRow transRows = transTable.Rows[i];
                        walletBalance = Convert.ToDouble(transRows["Amount"].ToString());
                    }
                    if (walletBalance >= amount)
                    {
                        myDBCommand.Parameters.Clear();
                        myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                        myDBCommand.CommandText = "tp_ProcessPayment";
                        myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
                    }


                        
                    
                }
            }
        }

        [HttpPut]
        public void UpdatePaymentAccount(int virtualWalletID, String name, String bankAccountType, String bankAccountNumber, int merchantAccountID, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKey";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckForMerchantAccountID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", merchantAccountID);
                DataSet selectedVirtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                //check if merchant account exists
                if (selectedVirtualWalletID.Tables.Count > 0 && selectedVirtualWalletID != null)
                {
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_UpdatePaymentAccount";
                    myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", virtualWalletID);
                    myDBCommand.Parameters.AddWithValue("@theName", name);
                    myDBCommand.Parameters.AddWithValue("@theBankAccountType", bankAccountType);
                    myDBCommand.Parameters.AddWithValue("@theBankAccountNumber", bankAccountNumber);
                    myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
                }
            }
        }

        [HttpPut]
        public void FundAccount(int virtualWalletID, double amount, int merchantAccountID, int apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKey";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            //check if APIKey exists
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckForMerchantAccountID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", merchantAccountID);
                DataSet selectedVirtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                //check if merchant account exists
                if (selectedVirtualWalletID.Tables.Count > 0 && selectedVirtualWalletID != null)
                {
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_FundAccount";
                    myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", virtualWalletID);
                    myDBCommand.Parameters.AddWithValue("@theAmmount", amount);
                    myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
                }
            }
        }


    }
}
