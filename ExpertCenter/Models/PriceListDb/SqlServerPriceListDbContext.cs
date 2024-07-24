using Microsoft.Data.SqlClient;

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


        public override TableData GetTable(PriceListInfo table)
        {
            var columns = GetCustomColumnsInfo(table);

            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = $"SELECT {string.Join(", ", columns.Select(i=>i.Name))} FROM {table.Name}";
            connection.Open();
            var reader = sqlCommand.ExecuteReader();

            List<PriceItem> values = new();
            int titleIndex = columns.TakeWhile(i=>i.Name != "Title").Count();
            int articleIndex = columns.TakeWhile(i=>i.Name != "Article").Count();

            while (reader.Read())
            {
                Dictionary<string, object> customValues = new();
                for (int i = 0; i < reader.FieldCount; i++)
                    if(i != titleIndex && i!= articleIndex)
                        customValues.Add(columns.ElementAt(i).Name, reader.GetValue(i));

                values.Add(new()
                {
                    Title = reader.GetString(titleIndex),
                    Article = reader.GetInt32(articleIndex),
                    CustomValues = customValues
                });
            }

            return new(columns, values);
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
        private async Task ExecuteNonQuerySqlCommandAsync(string command)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = command;
            connection.Open();
            await sqlCommand.ExecuteNonQueryAsync();
        }

        private Dictionary<PriceColumnType, string> _columnEqualsDic = new()
        {
            {PriceColumnType.Int, "INT"},
            {PriceColumnType.OneString, "NVARCHAR(50)"},
            { PriceColumnType.MultiString, "NVARCHAR(MAX)" }
        };
        private string ConvertColumnTypeToString(PriceColumnType type) => _columnEqualsDic[type];

        public override async Task<TableData> GetTableAsync(PriceListInfo table)
        {
            var columns = GetCustomColumnsInfo(table);

            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = $"SELECT {string.Join(", ", columns.Select(i => i.Name))} FROM {table.Name}";
            connection.Open();
            var reader = await sqlCommand.ExecuteReaderAsync();

            List<PriceItem> values = new();
            int titleIndex = columns.TakeWhile(i => i.Name != "Title").Count();
            int articleIndex = columns.TakeWhile(i => i.Name != "Article").Count();

            while (reader.Read())
            {
                Dictionary<string, object> customValues = new();
                for (int i = 0; i < reader.FieldCount; i++)
                    if (i != titleIndex && i != articleIndex)
                        customValues.Add(columns.ElementAt(i).Name, reader.GetValue(i));

                values.Add(new()
                {
                    Title = reader.GetString(titleIndex),
                    Article = reader.GetInt32(articleIndex),
                    CustomValues = customValues
                });
            }

            return new(columns, values);

        }

        public override async Task AddValueAsync(PriceListInfo table, PriceItem value)
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

            await ExecuteNonQuerySqlCommandAsync(command);
        }

        public override async Task RemoveValueAsync(PriceListInfo table, int article)
        {
            await ExecuteNonQuerySqlCommandAsync($"DELETE {table.Name} WHERE Article='{article}'");
        }

        public override async Task CreateTableAsync(string name, IEnumerable<PriceColumnInfo> columnsInfo)
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
            await sqlCommand.ExecuteReaderAsync();

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

            await SaveChangesAsync();
        }

        public override async Task DeleteTableAsync(PriceListInfo table)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand sqlCommand = connection.CreateCommand();
            sqlCommand.CommandText = $"DROP TABLE {table.Name}";
            connection.Open();
            await sqlCommand.ExecuteReaderAsync();

            PriceListSheet.Remove(table);

            SaveChanges();

        }
    }

    internal record class ColumnInfo(string COLUMN_NAME, string DATA_TYPE);
}
