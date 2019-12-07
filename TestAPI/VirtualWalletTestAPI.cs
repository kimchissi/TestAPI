using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI
{
    public class VirtualWalletTestAPI
    {
        private String email;
        private double balance;
        private String cardType;
        private String accountNumber;
        
        public String Email
        {
            get { return email; }
            set { email = value; }
        }
        
        public Double Balance
        {
            get { return balance; }
            set { balance = value; }
        }
        
        public String CardType
        {
            get { return cardType; }
            set { cardType = value; }
        }
        
        public String AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }
    }
}
