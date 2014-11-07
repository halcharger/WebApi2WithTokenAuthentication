using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;
using NExtensions;

namespace WebApi2WithTokenAuthorization.Entities
{
    public class RefreshToken : TableEntity
    {
        public const string PartitionKeyValue = "RefreshTokens";

        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string User { get; set; }
        [Required]
        [MaxLength(50)]
        public string ClientId { get; set; }
        public DateTime? IssuedUtc { get; set; }
        public DateTime? ExpiresUtc { get; set; }
        [Required]
        public string ProtectedTicket { get; set; }

        public void SetPartionAndRowKeys()
        {
            Id.ThrowIfNullOrEmpty("Id");

            PartitionKey = PartitionKeyValue;
            RowKey = Id;
        }
    }
}