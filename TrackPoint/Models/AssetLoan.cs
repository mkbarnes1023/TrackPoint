using Microsoft.AspNetCore.Identity;

namespace TrackPoint.Models
{
    public class AssetLoan
    {
        public int loanId;
        public int assetId;
        public IdentityUser borrowerId;
        public DateTime? checkedoutDate;
        public DateTime? dueDate;
        public DateTime? returnedDate;
        public IdentityUser 

    }
}
