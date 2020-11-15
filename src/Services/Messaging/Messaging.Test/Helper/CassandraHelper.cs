using Cassandra;

namespace Messaging.Test.Helper
{
    public class CassandraHelper
    {
        public static Cluster ConnectToDatabase(string hostAddress = "127.0.0.1", int port = 9042)
        {
            return Cluster.Builder()
                .AddContactPoints(hostAddress)
                .WithPort(port)
                .Build();
        }

        // Returns true upon successful keyspace creation
        public static bool CreateKeyspace(ISession session, string keyspaceName)
        {
            if (string.IsNullOrEmpty(keyspaceName))
                return false;

            DropKeyspaceIfExists(session, keyspaceName);
            
            session.Execute("CREATE KEYSPACE IF NOT EXISTS " + keyspaceName + " WITH replication = {"
                            + " 'class': 'SimpleStrategy', "
                            + " 'replication_factor': '3' "
                            + "};" );

            return true;
        }

        public static void DropKeyspaceIfExists(ISession session, string keyspaceName)
        {
            if (string.IsNullOrEmpty(keyspaceName))
                return;

            session.Execute("DROP KEYSPACE IF EXISTS " + keyspaceName);
        }

        public static void DropTableIfExists(ISession session, string keyspaceName, string tableName)
        {
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(keyspaceName))
                return;

            session.Execute("DROP TABLE IF EXISTS " + keyspaceName + "." + tableName);
        }
    }
}
