using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ExpertCenter.Models.PriceListDb
{
    public abstract class PriceListDbContext : DbContext
    {
        DbSet<PriceListInfo> PriceListSheet { get; set; } = null!;


        public abstract IEnumerable<PriceColumnInfo> GetColumnsInfo(PriceListInfo table);
        public abstract IEnumerable<PriceItem> GetValues(PriceListInfo table);

        public abstract void AddValue(PriceListInfo table, PriceItem value);
        public abstract void RemoveValue(PriceListInfo table, int article);

        public abstract PriceListInfo CreateTable(string name);
        public abstract PriceListInfo DeleteTable(PriceListInfo table);
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
