using Microsoft.WindowsAzure.Storage.Table;
using NExtensions;
using WebApi2WithTokenAuthorization.Models;

namespace WebApi2WithTokenAuthorization.Entities
{
    public class Client : TableEntity
    {
        public const string PartitionKeyValue = "Clients";

        public string Id { get; set; }
        
        public string Secret { get; set; }
        
        public string Name { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool Active { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public string AllowedOrigin { get; set; }

        public void SetPartionAndRowKeys()
        {
            Id.ThrowIfNullOrEmpty("Id");

            PartitionKey = PartitionKeyValue;
            RowKey = Id;
        }
    }
}