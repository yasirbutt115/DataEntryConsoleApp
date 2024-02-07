using Dapper;
using Microsoft.Data.SqlClient;

public class Program
{
    private const string ConnectionString = "Server=(localdb)\\MSSQLLocalDB;Database=master;";
   

    public static void Main()
    {
        InitializeDatabase();

        Console.WriteLine("1. Execute SQL Query and Extract Base Dataset");
        var baseDataset = ExecuteSqlQuery("SELECT * FROM BaseData");
        DisplayResults(baseDataset);

        Console.WriteLine("\n2. User Authentication and Record Filtering");
        var userRecords = FilterUserRecords("asif");
        DisplayResults(userRecords);

        //Console.WriteLine("\n3. Create Entry in Another Table");
        //CreateEntry("Some text description", "yasir");

        Console.WriteLine("\n4. Data Filtering for User Entries");
        var filteredEntries = FilterEntries("yasir");
        DisplayResults(filteredEntries);

        Console.WriteLine("\n5. Manager Role: View All Entries");
        var allEntries = ViewAllEntries();
        DisplayResults(allEntries);
    }

    private static void InitializeDatabase()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();

        connection.Execute(@"
          IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BaseData')
            BEGIN
                CREATE TABLE BaseData (
                    UniqueKey INT IDENTITY(1,1) PRIMARY KEY,
                    Username NVARCHAR(MAX) -- Add other fields as needed
                );
            INSERT INTO BaseData (Username) VALUES ('yasir');
            INSERT INTO BaseData (Username) VALUES ('asif');
            INSERT INTO BaseData (Username) VALUES ('nasir');
            INSERT INTO BaseData (Username) VALUES ('sajid');            
            END;

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserEntries')
            BEGIN
                CREATE TABLE UserEntries (
                    EntryId INT IDENTITY(1,1) PRIMARY KEY,
                    PrimaryKey INT,
                    TextDescription NVARCHAR(MAX),
                    Date DATETIME,
                    Username NVARCHAR(MAX),
                    FOREIGN KEY (PrimaryKey) REFERENCES BaseData(UniqueKey)
                );

            -- Inserting data without specifying EntryId (it will be auto-generated)
            INSERT INTO UserEntries (PrimaryKey, TextDescription, Date, Username) VALUES (1, 'Some Text', GETDATE(), 'yasir');
            -- Inserting data without specifying EntryId (it will be auto-generated)
            INSERT INTO UserEntries (PrimaryKey, TextDescription, Date, Username) VALUES (2, 'Some Text', GETDATE(), 'asif');
            -- Inserting data without specifying EntryId (it will be auto-generated)
            INSERT INTO UserEntries (PrimaryKey, TextDescription, Date, Username) VALUES (3, 'Some Text', GETDATE(), 'nasir');

            END;
        ");
    }

    private static dynamic ExecuteSqlQuery(string query)
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection.Query(query);
    }

    private static dynamic FilterUserRecords(string username)
    {
        var query = $"SELECT * FROM BaseData WHERE Username = '{username}'";
        return ExecuteSqlQuery(query);
    }

    //private static void CreateEntry(string description, string username)
    //{
    //    var primaryKey = 1; // Replace with the actual primary key
    //    var date = DateTime.Now;

    //    using var connection = new SqlConnection(ConnectionString);
    //    connection.Open();
    //    connection.Execute("INSERT INTO UserEntries (PrimaryKey, TextDescription, Date, Username) VALUES (@PrimaryKey, @Description, @Date, @Username)",
    //        new { PrimaryKey = primaryKey, Description = description, Date = date, Username = username });
    //}

    private static dynamic FilterEntries(string username)
    {
        var query = $"SELECT * FROM UserEntries WHERE Username = '{username}'";
        return ExecuteSqlQuery(query);
    }

    private static dynamic ViewAllEntries()
    {
        var query = "SELECT * FROM UserEntries";
        return ExecuteSqlQuery(query);
    }

    private static void DisplayResults(dynamic results)
    {
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }
}
