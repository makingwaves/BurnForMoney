using BurnForMoney.Functions.Presentation.Views.Poco;
using DapperExtensions.Mapper;

namespace BurnForMoney.Functions.Presentation.Views.Mappers
{
    public class AthleteMapper: ClassMapper<Athlete>
    {
        public new const string TableName = "Athletes";

        public AthleteMapper()
        {
            Map(x => x.Id).Key(KeyType.Assigned);
            AutoMap();
            Table(TableName);
        }
    } 
}