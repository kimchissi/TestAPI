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
        String SqlConnectString = "server=127.0.0.1,5555;Database=fa19_3344_tug91045;User id=tug91045;Password=ShuY4yoo";
        //String SqlConnectString = "server=cis-mssql1.temple.edu;Database=fa19_3344_tug91045;User id=tug91045;Password=ShuY4yoo";

        [HttpPost("CreateVirtualWallet")]
        public void CreateVirtualWallet(String name, String bankAccountType, String bankAccountNumber, int MerchantAccountId, String apiKey)
        {
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CreateVirtualWallet";
            myDBCommand.Parameters.AddWithValue("@theName", name);
            myDBCommand.Parameters.AddWithValue("@theBankAccountType", bankAccountType);
            myDBCommand.Parameters.AddWithValue("@theBankAccountNumber", bankAccountNumber);
            myDBConnect.DoUpdateUsingCmdObj(myDBCommand);
        }

        [HttpGet("GetTransactions")]
        public List<TransactionsTestAPI> GetTransactions(int VirtualWalletID, int MerchantAccountID, String apiKey)
        {
            List<TransactionsTestAPI> list = new List<TransactionsTestAPI>();
            myDBCommand.Parameters.Clear();
            myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
            myDBCommand.CommandText = "tp_CheckForAPIKey";
            myDBCommand.Parameters.AddWithValue("@theAPIKey", apiKey);
            DataSet selectedAPIKey = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
            if (selectedAPIKey.Tables.Count > 0 && selectedAPIKey != null)
            {
                myDBCommand.Parameters.Clear();
                myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                myDBCommand.CommandText = "tp_CheckForVirtualWalletID";
                myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", VirtualWalletID);
                DataSet selectedVirtualWalletID = myDBConnect.GetDataSetUsingCmdObj(myDBCommand);
                if (selectedVirtualWalletID.Tables.Count > 0 && selectedVirtualWalletID != null)
                {
                    myDBCommand.Parameters.Clear();
                    myDBCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    myDBCommand.CommandText = "tp_GetTransactionsByMerchantAccountID";
                    myDBCommand.Parameters.AddWithValue("@theVirtualWalletID", VirtualWalletID);
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
        public void ProcessPayment(int virtualWalletId, double amount, String type, int merchantAccountId, String apiKey)
        {

        }

        [HttpPut]
        public void UpdatePaymentAccount(int VirtualWalletId, object AccountHolderInformation, int MerchantAccountId, String apiKey)
        {

        }

        [HttpPut]
        public void FundAccount(int VirtualWalletId, double amount, int MerchantAccountId, int apiKey)
        {

        }


    }
}
