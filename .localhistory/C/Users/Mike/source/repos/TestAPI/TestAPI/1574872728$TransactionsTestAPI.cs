using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI
{
    public class TransactionsTestAPI
    {
        private String transactionsType;
        private String sendingVirtualWalletID;
        private String receivingVirtualWalletID;
        private Double amount;

        public String TransactionsType
        {
            get { return transactionsType; }
            set { transactionsType = value; }
        }

        public String SendingVirtualWalletID
        {
            get { return sendingVirtualWalletID; }
            set { sendingVirtualWalletID = value; }
        }

        public String ReceivingVirtualWalletID
        {
            get { return receivingVirtualWalletID; }
            set { receivingVirtualWalletID = value; }
        }

        public Double Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        

    }
}
