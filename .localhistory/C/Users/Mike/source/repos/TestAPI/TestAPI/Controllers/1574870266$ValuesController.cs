using System;
using System.Collections.Generic;
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
        SqlCommand objCommand = new SqlCommand();

        [HttpPost]
        public void CreateVirtualWallet(object AccountHolderInformation, int MerchantAccountId, String apiKey)
        {

        }

        [HttpGet("GetTransactions")]
        public List<TransactionsTestAPI> GetTransactions(int VirtualWalletId, int MerchantAccountId, String apiKey)
        {

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
