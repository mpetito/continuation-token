using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static ContinuationToken.Tests.TestRecords;

namespace ContinuationToken.Tests.Integration
{
    public class PaginationTests
    {
        [Theory]
        [MemberData(nameof(ScenarioGenerator))]
        public void ExhaustsPagesInOrder(TestScenario scenario, int recordCount, int pageSize)
        {
            var (continuationToken, sorter) = scenario;
            var data = GenerateRecords(recordCount).AsQueryable();

            var pageIndex = 0;
            var pageList = Paginate(pageSize, sorter(data));
            pageList.Add(Array.Empty<TestRecord>());    // continuation always has one last empty page to verify

            string token = default;
            do
            {
                var page = continuationToken.ResumeQuery(data, token).Take(pageSize).ToList();

                Assert.Equal(pageList[pageIndex++], page);

                token = continuationToken.GetToken(page.LastOrDefault());
            } while (token != default);
        }

        public static IEnumerable<object[]> ScenarioGenerator()
        {
            var scenarios = MakeScenarios();
            var recordCounts = new[] { 0, 1, 10, 100 };
            var pageSizes = new[] { 1, 7, 10 };

            // cross-join all parameters
            foreach (var scenario in scenarios)
                foreach (var recordCount in recordCounts)
                    foreach (var pageSize in pageSizes)
                        yield return new object[] { scenario, recordCount, pageSize };
        }
    }
}
