using ContinuationToken.Formatting;
using System;
using System.Collections.Generic;

namespace ContinuationToken.Tests
{
    public record TestRecord(int Id, string Name, DateTime Time, Guid Uuid, int? Amount);

    public static class TestRecords
    {
        public static IEnumerable<TestRecord> GenerateRecords(int count) => new Bogus.Faker<TestRecord>()
                .UseSeed(0)
                .CustomInstantiator(f => new TestRecord(default, default, default, default, default))
                .RuleFor(r => r.Id, f => f.IndexGlobal)
                .RuleFor(r => r.Name, f => f.Name.FullName())
                .RuleFor(r => r.Time, f => f.Date.Future().ToUniversalTime())
                .RuleFor(r => r.Uuid, f => Guid.NewGuid())
                .RuleFor(r => r.Amount, f => f.Random.Int(0, 10))
                .Generate(count);

        public static IEnumerable<TestScenario> MakeScenarios()
        {
            TestScenario Scenario()
            {
                var scenario = new TestScenario();
                // for easier debugging
                scenario.Builder.UseFormatter(new JsonTokenFormatter());
                return scenario;
            }

            var scenarios = new List<TestScenario>
            {
                Scenario().Ascending(r => r.Id),
                Scenario().Descending(r => r.Id),
                Scenario().Ascending(r => r.Uuid),
                Scenario().Descending(r => r.Uuid),
                Scenario().Ascending(r => r.Id).Descending(r => r.Uuid),
                Scenario().Ascending(r => r.Name).Ascending(r => r.Id),
                Scenario().Descending(r => r.Name).Ascending(r => r.Id),
                Scenario().Ascending(r => r.Time).Ascending(r => r.Id),
                Scenario().Descending(r => r.Time).Ascending(r => r.Id),
                Scenario().Ascending(r => r.Amount).Ascending(r => r.Id),
                Scenario().Descending(r => r.Amount).Ascending(r => r.Id),
                Scenario().Ascending(r => r.Name).Descending(r => r.Time).Ascending(r => r.Id),
            };

            return scenarios;
        }

        public static List<TestRecord[]> Paginate(int pageSize, IEnumerable<TestRecord> records)
        {
            var result = new List<TestRecord[]>();
            var page = new List<TestRecord>();

            foreach (var record in records)
            {
                if (page.Count == pageSize)
                {
                    result.Add(page.ToArray());
                    page.Clear();
                }

                page.Add(record);
            }

            if (page.Count > 0)
            {
                result.Add(page.ToArray());
            }

            return result;
        }
    }
}
