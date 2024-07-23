using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ExpertCenter.Models.PriceListDb
{
    public class SqlServerPriceListDbContext : PriceListDbContext
    {
        public SqlServerPriceListDbContext(string connectionString):base(connectionString) { }


        public override void CreateTable(string name, IEnumerable<PriceColumnInfo> columnsInfo)
        {
            string command = $"CREATE TABLE {name} ( Article INT PRIMARY KEY, Title NVARCHAR(50) NOT NULL";
            foreach (var item in columnsInfo)
            {
                command += $",{item.Name} {ConvertColumnTypeToString(item.Type)}";
            }
            command += ")";

            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = command;
            connection.Open();
            sqlCommand.ExecuteReader();

            var table = new PriceListInfo()
            {
                Name = name
            };
            PriceListSheet.Add(table);
            ColumnInfoTable.AddRange(columnsInfo.Select(i => new ColumnInfoTable()
            {
                PriceListInfo = table,
                Name = i.Name,
                Type = i.Type
            }));
            ColumnInfoTable.Add(new ColumnInfoTable()
            {
                PriceListInfo = table,
                Name = "Title",
                Type = PriceColumnType.OneString
            });
            ColumnInfoTable.Add(new ColumnInfoTable()
            {
                PriceListInfo = table,
                Name = "Article",
                Type = PriceColumnType.Int
            });

            SaveChanges();
        }

        public override void DeleteTable(PriceListInfo table)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = $"DROP TABLE {table.Name}";
            connection.Open();
            sqlCommand.ExecuteReader();

            PriceListSheet.Remove(table);

            SaveChanges();
        }


        public override IEnumerable<PriceItem> GetValues(PriceListInfo table)
        {
            var columns = GetCustomColumnsInfo(table);

            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = $"SELECT * FROM {table.Name}";
            connection.Open();
            var reader = sqlCommand.ExecuteReader();

            throw new NotImplementedException();
        }

        public override void AddValue(PriceListInfo table, PriceItem value)
        {
            string command = $"""
                INSERT INTO {table.Name}(Title, Article 
                """;
            foreach (var item in value.CustomValues)
                command += $", {item.Key}";
            command += $") VALUES ('{value.Title}', '{value.Article}'";
            foreach (var item in value.CustomValues)
                
                command += $", '{item.Value}'";
            command += ")";

            ExecuteNonQuerySqlCommand(command);
        }

        public override void RemoveValue(PriceListInfo table, int article)
        {
            ExecuteNonQuerySqlCommand($"DELETE {table.Name} WHERE Article='{article}'");
        }

        private void ExecuteNonQuerySqlCommand(string command)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = command;
            connection.Open();
            sqlCommand.ExecuteNonQuery();

        }


        private Dictionary<PriceColumnType, string> _columnEqualsDic = new()
        {
            {PriceColumnType.Int, "INT"},
            {PriceColumnType.OneString, "NVARCHAR(50)"},
            { PriceColumnType.MultiString, "NVARCHAR(MAX)" }
        };
        private string ConvertColumnTypeToString(PriceColumnType type) => _columnEqualsDic[type];

    }

    internal record class ColumnInfo(string COLUMN_NAME, string DATA_TYPE);
}
