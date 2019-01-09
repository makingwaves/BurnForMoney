using BurnForMoney.Functions.Presentation.Views.Poco;
using DapperExtensions.Mapper;

namespace BurnForMoney.Functions.Presentation.Views.Mappers
{
    public class ActivityMapper: ClassMapper<Activity>
    {
        public new const string TableName = "Activities";

        public ActivityMapper()
        {
            Map(x => x.Id).Key(KeyType.Assigned);
            AutoMap();
            Table(TableName);
        }
    } 
}