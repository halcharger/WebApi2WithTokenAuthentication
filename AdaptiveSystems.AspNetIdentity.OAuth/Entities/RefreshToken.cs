using System;
using Microsoft.WindowsAzure.Storage.Table;
using NExtensions;

namespace AdaptiveSystems.AspNetIdentity.OAuth.Entities
{
    public class RefreshToken : TableEntity
    {
        public const string PartitionKeyValue = "RefreshTokens";

        public string Id { get; set; }
        public string User { get; set; }
        public string ClientId { get; set; }
        public DateTime? IssuedUtc { get; set; }
        public DateTime? ExpiresUtc { get; set; }
        public string ProtectedTicket { get; set; }

        public void SetPartionAndRowKeys()
        {
            Id.ThrowIfNullOrEmpty("Id");

            PartitionKey = PartitionKeyValue;
            RowKey = Id;
        }
    }
}