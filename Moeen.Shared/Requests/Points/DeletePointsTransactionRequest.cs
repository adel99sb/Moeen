using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Points
{
    public class DeletePointsTransactionRequest
    {
        [Required(ErrorMessage = "Transaction ID is required")]
        public Guid TransactionId { get; set; }
    }
}