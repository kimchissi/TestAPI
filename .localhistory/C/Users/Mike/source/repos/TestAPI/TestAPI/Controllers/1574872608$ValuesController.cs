using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestAPI;

namespace PaymentApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        DBConnectTestAPI myDBConnect = new DBConnectTestAPI();
        SqlCommand myDBCommand = new SqlCommand();

        [HttpPost]
        public void CreateVirtualWallet(object AccountHolderInformation, int MerchantAccountId, String apiKey)
        {

        }

        [HttpGet("GetTransactions")]
        public List<TransactionsTestAPI> GetTransactions(int VirtualWalletID, int MerchantAccountID, String apiKey)
        {
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

                }
            }
            List<TransactionsTestAPI> list = new List<TransactionsTestAPI>();

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
