using BurnForMoney.Functions.Presentation.Views.Poco;
using DapperExtensions.Mapper;

namespace BurnForMoney.Functions.Presentation.Views.Mappers
{
    public class IndividualResultMapper : ClassMapper<IndividualResult>
    {
        public new const string TableName = "IndividualResults";
        
        public IndividualResultMapper()
        {
            Map(x => x.AthleteId).Key(KeyType.Assigned);
            Map(x => x.Category).Key(KeyType.Assigned);
            Map(x => x.Month).Key(KeyType.Assigned);
            Map(x => x.Year).Key(KeyType.Assigned);
            AutoMap();
            Table(TableName);
        }
    } 
}