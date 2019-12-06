using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI
{
    public class AccountHolderInformation
    {
        public String email;
        public String cardType;
        public String accountNumber;

        [JsonProperty(PropertyName = "Email")]
        public String Email
        {
            get { return email; }
            set { email = value; }
        }

        [JsonProperty(PropertyName = "CardType")]
        public String CardType
        {
            get { return cardType; }
            set { cardType = value; }
        }

        [JsonProperty(PropertyName = "AccountNumber")]
        public String AccountNumber
        {
            get { return accountNumber; }
            set { accountNumber = value; }
        }

        public AccountHolderInformation()
        {

        }

        public AccountHolderInformation(string email, string cardType, string accountNumber)
        {
            this.email = email;
            this.cardType = cardType;
            this.accountNumber = accountNumber;
        }
    }
}
