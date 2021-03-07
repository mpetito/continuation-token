using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static ContinuationToken.Tests.TestRecords;

namespace ContinuationToken.Tests.Integration
{
    public class TranslationTests
    {
        internal void QueriesCanTranslate(TestScenario scenario, DbContextOptions options)
        {
            var (continuationToken, _) = scenario;
            var data = GenerateRecords(1).ToList();

            using var dbContext = new TestDbContext(options);

            var first = continuationToken.ResumeQuery(dbContext.Records);
            Assert.NotNull(first.Query.ToQueryString());

            var token = first.GetNextToken(data);
            Assert.NotNull(token);

            var next = continuationToken.ResumeQuery(dbContext.Records, token);
            Assert.NotNull(next.Query.ToQueryString());
        }

        [Theory]
        [MemberData(nameof(ScenarioGenerator))]
        public void BookmarkQueriesCanTranslate(TestScenario scenario, DbContextOptions options)
        {
            scenario.Builder.BookmarkToken();

            QueriesCanTranslate(scenario, options);
        }

        [Theory]
        [MemberData(nameof(ScenarioGenerator))]
        public void OffsetQueriesCanTranslate(TestScenario scenario, DbContextOptions options)
        {
            scenario.Builder.OffsetToken();

            QueriesCanTranslate(scenario, options);
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
