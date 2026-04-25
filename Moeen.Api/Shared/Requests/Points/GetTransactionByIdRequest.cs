using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Points
{
    public class GetTransactionByIdRequest
    {
        [Required(ErrorMessage = "Transaction ID is required")]
        public Guid TransactionId { get; set; }
    }
}