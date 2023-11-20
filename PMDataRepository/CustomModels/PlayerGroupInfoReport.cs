using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMDataRepository.CustomModels
{
    [Table("PlayerGroupInfoReport")]
    public class PlayerGroupInfoReport
    {
        public int PlayerID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? PlayerSessionAccountingDate { get; set; }

        public DateTime? CouponRedeemAccountingDate { get; set; }

        public DateTime? CompAccountingDate { get; set; }

    }
}
