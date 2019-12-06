using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI
{
    public class TransactionsTestAPI
    {
        private int transactionsID;
        private String transactionsType;
        private String sendingVirtualWalletEmail;
        private String receivingVirtualWalletEmail;
        private Double amount;

        public int TransactionsID
        {
            get { return transactionsID; }
            set { transactionsID = value; }
        }

        public String TransactionsType
        {
            get { return transactionsType; }
            set { transactionsType = value; }
        }

        public String SendingVirtualWalletEmail
        {
            get { return sendingVirtualWalletEmail; }
            set { sendingVirtualWalletEmail = value; }
        }

        public String ReceivingVirtualWalletEmail
        {
            get { return receivingVirtualWalletEmail; }
            set { receivingVirtualWalletEmail = value; }
        }

        public Double Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        

    }
}
