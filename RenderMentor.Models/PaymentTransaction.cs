using RenderMentor.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RenderMentor.Models
{
    public class PaymentTransaction
    {
        [Key]
        public int Id { get; set; }        
        public string OrderNumber { get; set; }
        public string TransactionNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public string CustomerIpAddress { get; set; }
        public string UserAgent { get; set; }
        public string CardPrefix { get; set; }
        public string CardHolderName { get; set; }
        public double TotalAmount { get; set; }
        public string BankErrorMessage { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime CreateDate { get; set; }
        public int StatusId { get; set; }
        public bool Deleted { get; set; }
        public string BankRequest { get; set; }
        public string BankResponse { get; set; }
        public string ApplicationUserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public PaymentStatus Status
        {
            get => (PaymentStatus)StatusId;
            set => StatusId = (int)value;
        }

        public void MarkAsCreated()
        {
            CreateDate = DateTime.Now;
            Status = PaymentStatus.Pending;
        }

        public void MarkAsPaid(string transactionId, string referenceNumber)
        {
            Status = PaymentStatus.Paid;
            PaidDate = DateTime.Now;
            TransactionNumber = transactionId;
            ReferenceNumber = referenceNumber;
        }

        public void MarkAsPaid(DateTime paidDate)
        {
            Status = PaymentStatus.Paid;
            PaidDate = paidDate;
        }

        public void MarkAsFailed(string bankErrorMessage, string bankResponse)
        {
            Status = PaymentStatus.Failed;
            BankErrorMessage = bankErrorMessage;
            BankResponse = bankResponse;
        }
    }
}
