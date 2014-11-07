using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;
using NExtensions;
using WebApi2WithTokenAuthorization.Models;

namespace WebApi2WithTokenAuthorization.Entities
{
    public class Client : TableEntity
    {
        public const string PartitionKeyValue = "Clients";
        [Key]
        public string Id { get; set; }
        [Required]
        public string Secret { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool Active { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        [MaxLength(100)]
        public string AllowedOrigin { get; set; }

        public void SetPartionAndRowKeys()
        {
            Id.ThrowIfNullOrEmpty("Id");

            PartitionKey = PartitionKeyValue;
            RowKey = Id;
        }
    }
}