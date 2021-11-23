using System;

namespace Active8_CancelAllMemberships.Models
{
    public class MembershipUsersModel
    {
        public int? MembershipID { get; set; }
        public int? PersonID { get; set; }
        public int? MembershipStatusID { get; set; }
        public int? SalesDocumentItemID { get; set; }
        public int? ProductMembershipID { get; set; }
        public int? EventID { get; set; }
        public int? CardNumber { get; set; }
        public int? Magstripe { get; set; }
        public int? Pin { get; set; }
        public int? CreatePersonID { get; set; }
        public DateTime? CreateDate { get; set; }
        public int? UpdatePersonID { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? ContractSalesDocumentID { get; set; }
        public DateTime? CancelDate { get; set; }
        public int? CurrentMembershipContractID { get; set; }
    }
}
