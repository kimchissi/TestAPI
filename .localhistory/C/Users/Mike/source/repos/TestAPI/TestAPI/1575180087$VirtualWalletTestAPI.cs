using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI
{
    public class VirtualWalletTestAPI
    {
        private String name;
        private double balance;
        private String bankType;
        private String bankAccountNumber;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public Double Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public String BankType
        {
            get { return bankType; }
            set { bankType = value; }
        }

        public String BankAccountNumber
        {
            get { return bankAccountNumber; }
            set { bankAccountNumber = value; }
        }
    }
}
