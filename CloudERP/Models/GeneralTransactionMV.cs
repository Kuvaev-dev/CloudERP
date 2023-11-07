using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudERP.Models
{
    public class GeneralTransactionMV
    {
        [Required(ErrorMessage = "*Required!")]
        public int DebitAccountControlID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public int CreditAccountControlID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        [DataType(DataType.Currency)]
        public float TransferAmount { get; set; }
        [Required(ErrorMessage = "*Required!")]
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }
    }
}