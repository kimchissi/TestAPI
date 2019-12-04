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
        private String bankAccountType;
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

        public String BankAccountType
        {
            get { return bankAccountType; }
            set { bankAccountType = value; }
        }

        public String BankAccountNumber
        {
            get { return bankAccountNumber; }
            set { bankAccountNumber = value; }
        }
    }
}
