using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ExpertCenter.Models.PriceListDb
{
    public record PriceListDbSettings(string ConnectString);
    public abstract class PriceListDbContext : DbContext
    {
        public PriceListDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DbSet<PriceListInfo> PriceListSheet { get; set; } = null!;
        public DbSet<ColumnInfoTable> ColumnInfoTable { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ColumnInfoTable>()
                .HasOne(i=>i.PriceListInfo)
                .WithMany(i=>i.ColumnInfoTables)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }

        #region TableAPI
        public IEnumerable<PriceColumnInfo> GetCustomColumnsInfo(PriceListInfo table)
        {
            return ColumnInfoTable.Include(i => i.PriceListInfo)
                .Where(i => i.PriceListInfo == table)
                .Select(i => new PriceColumnInfo(i.Name, i.Type));
        }
        public abstract TableData GetTable(PriceListInfo table);
        public abstract Task<TableData> GetTableAsync(PriceListInfo table);
        public abstract void AddValue(PriceListInfo table, PriceItem value);
        public abstract Task AddValueAsync(PriceListInfo table, PriceItem value);

        public abstract void RemoveValue(PriceListInfo table, int article);
        public abstract Task RemoveValueAsync(PriceListInfo table, int article);

        public abstract void CreateTable(string name, IEnumerable<PriceColumnInfo> columnsInfo);
        public abstract Task CreateTableAsync(string name, IEnumerable<PriceColumnInfo> columnsInfo);
        public abstract void DeleteTable(PriceListInfo table);
        public abstract Task DeleteTableAsync(PriceListInfo table);

        #endregion

        protected readonly string _connectionString;
    }

    public class PriceListInfo
    {
        [Key]
        public int Number { get; set; }

        [NotNull]
        [MinLength(5)]
        public string Name { get; set; }

        public List<ColumnInfoTable> ColumnInfoTables { get; set; }
    }

    public record class TableData(IEnumerable<PriceColumnInfo> Headers, IEnumerable<PriceItem> Values);

    public class ColumnInfoTable
    {
        public int Id { get; set; }
        public PriceColumnType Type { get; set; }
        public string Name { get; set; }

        public int PriceListInfoId { get; set; }
        public PriceListInfo PriceListInfo { get; set; }
    }

    public class PriceItem
    {
        [Key]
        public int Article { get; set; }

        [NotNull]
        [MinLength(5)]
        public string Title { get; set; }
        public Dictionary<string, object> CustomValues { get; set; }
    }
    public enum PriceColumnType
    {
        Int, OneString, MultiString
    }
    public record class PriceColumnInfo(string Name, PriceColumnType Type);
}
