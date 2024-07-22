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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public abstract IEnumerable<PriceColumnInfo> GetColumnsInfo(PriceListInfo table);
        public abstract IEnumerable<PriceItem> GetValues(PriceListInfo table);

        public abstract void AddValue(PriceListInfo table, PriceItem value);
        public abstract void RemoveValue(PriceListInfo table, int article);

        public abstract void CreateTable(string name, IEnumerable<PriceColumnInfo> columnsInfo);
        public abstract void DeleteTable(PriceListInfo table);


        protected readonly string _connectionString;
    }

    public class PriceListInfo
    {
        [Key]
        public int Number { get; set; }

        [NotNull]
        [MinLength(5)]
        public string Name { get; set; }
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
