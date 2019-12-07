using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI
{
    public class AccountHolderInformation
    {
        private String email;
        //private String receiverEmail;
        private String cardType;
        private String accountNumber;


        [JsonProperty(PropertyName = "Email")]
        public String Email
        {
            get { return email; }
            set { email = value; }
        }

        /*[JsonProperty(PropertyName = "ReceiverEmail")]
        public String ReceiverEmail
        {
            get { return ReceiverEmail; }
            set { ReceiverEmail = value; }
        }*/

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

        public AccountHolderInformation(string email, string receiverEmail, string cardType, string accountNumber, double amount)
        {
            this.email = email;
            this.cardType = cardType;
            this.accountNumber = accountNumber;
        }
    }
}
