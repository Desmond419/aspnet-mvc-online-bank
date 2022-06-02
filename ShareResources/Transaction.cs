using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShareResources
{
    public class Transaction
    {
        public int Id { get; set; }

        [Display(Name = "Recipient's Name")]
        public string BankAccName { get; set; }

        [Display(Name = "Sender's Name")]
        public string SenderName { get; set; }

        public string Type { get; set; }

        [Display(Name = "Transaction Amount")]
        public int TransactionAmount { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TransactionDate { get; set; }

        public decimal Credit { get; set; }
        public decimal Debit { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

    }
}