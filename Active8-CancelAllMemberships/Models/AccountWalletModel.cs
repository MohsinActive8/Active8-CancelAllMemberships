using System;
using System.Collections.Generic;
using System.Text;

namespace Active8_CancelAllMemberships.Models
{
    class AccountWalletModel
    {
        public int AccountWalletID { get; set; } = 0;
        public int AccountID { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string CCName { get; set; } = string.Empty;
        public string CCLastFour { get; set; } = string.Empty;
        public string CCExpirationMonth { get; set; } = string.Empty;
        public int CCExpirationYear { get; set; } = 0;
        public string CCNumber { get; set; } = string.Empty;

        public string CCType { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedDate { get; set; } = null;
        public bool AllowedForPurchases { get; set; } = false;
        public string CCAuthorizeTransactionID { get; set; } = string.Empty;
        public int AddressID { get; set; } = 0;
        public bool InStoreProcessor { get; set; }
    }
}
