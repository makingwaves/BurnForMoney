using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BurnForMoney.Infrastructure.Persistence.Repositories.Dto;
using BurnForMoney.Infrastructure.Persistence.Sql;
using Dapper;

namespace BurnForMoney.Infrastructure.Persistence.Repositories
{
    public class RankingReadRepository
    {
        private readonly string _sqlConnectionString;

        public RankingReadRepository(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        public async Task<IEnumerable<RankingByPoints>> GetTopByPointsForCategoryAsync(string category, int take = 10, int? month = null, int? year = null)
        {
            if (take < 1)
            {
                throw new IndexOutOfRangeException(nameof(take));
            }
            if (month.HasValue && (month < 1 || month > 12))
            {
                throw new IndexOutOfRangeException(nameof(month));
            }
            if (year.HasValue && year < 2000)
            {
                throw new IndexOutOfRangeException(nameof(year));
            }

            using (var conn = SqlConnectionFactory.Create(_sqlConnectionString))
            {
                await conn.OpenWithRetryAsync();

                var activities = await conn.QueryAsync<RankingByPoints>(
                    @"
SELECT TOP (@Take) A.Id as AthleteId, A.FirstName as AthleteFirstName, A.LastName AS AthleteLastName, A.ProfilePictureUrl, SUM(IR.Points) AS Points 
FROM dbo.IndividualResults IR 
INNER JOIN dbo.Athletes A ON A.Id = IR.AthleteId
WHERE IR.Category = COALESCE(@Category, IR.Category) AND IR.Month = COALESCE(@Month, IR.Month) AND IR.Year = COALESCE(@Year, IR.Year)
GROUP BY A.Id, A.FirstName, A.LastName, A.ProfilePictureUrl
ORDER BY SUM(IR.Points) DESC", new
                    {
                        Take = take,
                        Category = string.IsNullOrWhiteSpace(category) ? null : category,
                        Month = month,
                        Year = year
                    });

                return activities;
            }
        }
    }
}