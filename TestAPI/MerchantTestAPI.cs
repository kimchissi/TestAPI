using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI
{
    public class MerchantTestAPI
    {
        private String merchantName;
        private String userName;
        private String password;
        private String apiKey;

        public String MerchantName
        {
            get { return merchantName; }
            set { merchantName = value; }
        }

        public String UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public String Password
        {
            get { return password; }
            set { password = value; }
        }

        public String APIKey
        {
            get { return apiKey; }
            set { apiKey = value; }
        }
    }
}
