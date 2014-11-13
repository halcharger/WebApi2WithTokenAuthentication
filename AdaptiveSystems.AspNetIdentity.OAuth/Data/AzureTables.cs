using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AdaptiveSystems.AspNetIdentity.OAuth.Data
{
    public class AzureTables
    {
        public const string ClientTablename = "clients";
        public const string RefreshTokenTablename = "refreshtokens";
        public const string PartitionKeyFieldName = "PartitionKey";
        public const string RowKeyFieldName = "RowKey";

        private readonly CloudTable clientsTable;
        private readonly CloudTable refreshTokenTable;

        public AzureTables() : this("UserStore-ConnectionString") { }
        public AzureTables(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            ClientsTable = tableClient.GetTableReference(ClientTablename);
            RefreshTokenTable = tableClient.GetTableReference(RefreshTokenTablename);

            //need to configure this so we don't run it each and everytime
            ClientsTable.CreateIfNotExists();
            RefreshTokenTable.CreateIfNotExists();
        }

        public CloudTable ClientsTable { get; set; }
        public CloudTable RefreshTokenTable { get; set; }
    }
}