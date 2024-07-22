namespace ExpertCenter.Models.PriceListDb
{
    public class SqlServerPriceListDbContext : PriceListDbContext
    {
        public override void AddValue(PriceListInfo table, PriceItem value)
        {
            throw new NotImplementedException();
        }

        public override PriceListInfo CreateTable(string name)
        {
            throw new NotImplementedException();
        }

        public override PriceListInfo DeleteTable(PriceListInfo table)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PriceColumnInfo> GetColumnsInfo(PriceListInfo table)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<PriceItem> GetValues(PriceListInfo table)
        {
            throw new NotImplementedException();
        }

        public override void RemoveValue(PriceListInfo table, int article)
        {
            throw new NotImplementedException();
        }
    }
}
