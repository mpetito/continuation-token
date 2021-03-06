using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static ContinuationToken.Tests.TestRecords;

namespace ContinuationToken.Tests.Integration
{
    public class QueryTranslationTests
    {
        [Theory]
        [MemberData(nameof(ScenarioGenerator))]
        public void QueriesCanTranslate(TestScenario scenario, DbContextOptions options)
        {
            var (continuationToken, _) = scenario;
            var data = GenerateRecords(1).Last();

            using var dbContext = new TestDbContext(options);

            var token = continuationToken.GetToken(data);

            var initQuery = scenario.ContinuationToken.ResumeQuery(dbContext.Records);
            var nextQuery = scenario.ContinuationToken.ResumeQuery(dbContext.Records, token);

            Assert.NotNull(initQuery.ToQueryString());
            Assert.NotNull(nextQuery.ToQueryString());
        }

        public static IEnumerable<object[]> ScenarioGenerator()
        {
            var scenarios = MakeScenarios();

            var dbContextOptions = new[]
            {
                new DbContextOptionsBuilder()
                    .UseSqlServer("Server=(local);Database=Test;Trusted_Connection=True;"),

                new DbContextOptionsBuilder()
                    .UseSqlite("Data Source=:memory:;"),

                new DbContextOptionsBuilder()
                    .UseMySQL("Server=localhost;Database=Test;Uid=user;Pwd=pass;"),
            };

            // cross-join all parameters
            foreach (var scenario in scenarios)
                foreach (var options in dbContextOptions)
                    yield return new object[] { scenario, options.Options };
        }

        public class TestDbContext : DbContext
        {
            public DbSet<TestRecord> Records { get; set; }

            public TestDbContext(DbContextOptions options) 
                : base(options)
            {
            }
        }
    }
}
