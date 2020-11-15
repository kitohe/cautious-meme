using System.Linq;
using Xunit;
using Messaging.Test.Helper;
using Xunit.Abstractions;

namespace Messaging.Test
{
    public class CassandraTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CassandraTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test()
        {
            // Arrange
            var cluster = CassandraHelper.ConnectToDatabase();
            var session = cluster.Connect();
            var keyspaceName = "test_keyspace";
            var tableName = "test_table";

            session.ChangeKeyspace(keyspaceName);
            session.CreateKeyspaceIfNotExists(keyspaceName);
            CassandraHelper.DropTableIfExists(session, keyspaceName, tableName);

            session.Execute("CREATE TABLE test_table (id int PRIMARY KEY, first_name text, last_name text)");
            session.Execute("INSERT INTO test_table (id, first_name, last_name) VALUES (1, 'test', 'user')");

            // Act
            var rs = session.Execute("SELECT * FROM test_table");
            var row = rs.GetRows().ElementAt(0);

            var id = row.GetValue<int>("id");
            var firstName = row.GetValue<string>("first_name");
            var lastName = row.GetValue<string>("last_name");

            CassandraHelper.DropTableIfExists(session, keyspaceName, tableName);
            CassandraHelper.DropKeyspaceIfExists(session, keyspaceName);

            // Assert
            Assert.Equal(1, id);
            Assert.Equal("test", firstName);
            Assert.Equal("user", lastName);
        }
    }
}
