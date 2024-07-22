using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ExpertCenter.Models.PriceListDb
{
    public class SqlServerPriceListDbContext : PriceListDbContext
    {
        public SqlServerPriceListDbContext(string connectionString):base(connectionString) { }


        public override void CreateTable(string name, IEnumerable<PriceColumnInfo> columnsInfo)
        {
            string command = $"""
                CREATE TABLE {name}
                (Article INT PRIMARY KEY,
                Title NVARCHAR NOT NULL,
                """;

            columnsInfo.Select(i => command += $"{i.Name} {ColumnTypeConvert(i.Type)},\n");
            command += """
                );
                GO
                """;
            ExecuteReaderSqlCommand(command);

            PriceListSheet.Add(new PriceListInfo()
            {
                Name = name
            });
            SaveChanges();
        }

        public override void DeleteTable(PriceListInfo table)
        {
            ExecuteReaderSqlCommand($"""
                DELETE TABLE {table.Name}
                GO
                """);

            PriceListSheet.Remove(table);
            SaveChanges();
        }

        public override IEnumerable<PriceColumnInfo> GetColumnsInfo(PriceListInfo table)
        {
            var res = ExecuteReaderSqlCommand($"""
                SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS
                WHERE TABLE_NAME = "{table.Name}"
                GO
                """);
            foreach (var item in res)
            {
                
            }
            throw new NotImplementedException();

        }

        public override IEnumerable<PriceItem> GetValues(PriceListInfo table)
        {
            var columns = GetColumnsInfo(table);
            var res = ExecuteReaderSqlCommand($"""
                SELECT * FROM {table.Name}
                GO
                """);
            foreach (var item in res)
            {

            }
            throw new NotImplementedException();
        }

        public override void AddValue(PriceListInfo table, PriceItem value)
        {
            string command = $"""
                INSERT INTO {table.Name}(Title, Article 

                """;
            value.CustomValues.Select(i => command += $", {i.Key}");
            command += $") VALUES ({value.Title}, {value.Article}";
            value.CustomValues.Select(i => command += $", {i.Value}");
            command += ") GO";

            ExecuteNonQuerySqlCommand(command);
        }

        public override void RemoveValue(PriceListInfo table, int article)
        {
            ExecuteNonQuerySqlCommand($"""
                DELETE ${table.Name}
                WHERE Article=${article}
                """);
        }

        private SqlDataReader ExecuteReaderSqlCommand(string command)
        {
            using SqlConnection connection = new(this._connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = command;
            connection.Open();
            return sqlCommand.ExecuteReader();

        }

        private void ExecuteNonQuerySqlCommand(string command)
        {
            using SqlConnection connection = new(this._connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = command;
            connection.Open();
            sqlCommand.ExecuteNonQuery();

        }

        private string ColumnTypeConvert(PriceColumnType type) => type switch
        {
            PriceColumnType.Int => "INT",
            PriceColumnType.OneString => "NVARCHAR",
            PriceColumnType.MultiString => "NVARCHAR",
            _=>"NVARCHAR"
        };
    }

    internal record class ColumnInfo(string COLUMN_NAME, string DATA_TYPE);
}
