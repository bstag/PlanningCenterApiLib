using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlanningCenter.Api.Client.Fluent;
using PlanningCenter.Api.Client.Models;
using PlanningCenter.Api.Client.Tests.TestHelpers;

namespace PlanningCenter.Api.Client.Tests.Fluent
{
    [TestClass]
    public class AggregationSupportTests
    {
        private TestFluentQueryBuilder<Person> _builder = null!;
        private List<Person> _testData;

        [TestInitialize]
        public void Setup()
        {
            _testData = new List<Person>
            {
                new Person { Id = "1", Name = "John Doe", Age = 30, Salary = 50000 },
                new Person { Id = "2", Name = "Jane Smith", Age = 25, Salary = 60000 },
                new Person { Id = "3", Name = "Bob Johnson", Age = 35, Salary = 55000 },
                new Person { Id = "4", Name = "Alice Brown", Age = 28, Salary = 65000 },
                new Person { Id = "5", Name = "Charlie Wilson", Age = 32, Salary = 58000 }
            };
            
            _builder = new TestFluentQueryBuilder<Person>(_testData);
        }

        [TestMethod]
        public async Task CountAsync_WithPredicate_ReturnsCorrectCount()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age > 30;

            // Act
            var result = await _builder.CountAsync(predicate);

            // Assert
            Assert.AreEqual(2, result); // Bob (35) and Charlie (32)
        }

        [TestMethod]
        public async Task CountDistinctAsync_ByField_ReturnsCorrectCount()
        {
            // Arrange
            var testDataWithDuplicates = new List<Person>
            {
                new Person { Id = "1", Name = "John", Age = 30 },
                new Person { Id = "2", Name = "Jane", Age = 25 },
                new Person { Id = "3", Name = "John", Age = 35 },
                new Person { Id = "4", Name = "Alice", Age = 25 }
            };
            var builder = new TestFluentQueryBuilder<Person>(testDataWithDuplicates);

            // Act
            var result = await builder.CountDistinctAsync("Name");

            // Assert
            Assert.AreEqual(3, result); // John, Jane, Alice (3 distinct names)
        }

        [TestMethod]
        public async Task CountDistinctAsync_BySelector_ReturnsCorrectCount()
        {
            // Arrange
            var testDataWithDuplicates = new List<Person>
            {
                new Person { Id = "1", Name = "John", Age = 30 },
                new Person { Id = "2", Name = "Jane", Age = 25 },
                new Person { Id = "3", Name = "Bob", Age = 30 },
                new Person { Id = "4", Name = "Alice", Age = 25 }
            };
            var builder = new TestFluentQueryBuilder<Person>(testDataWithDuplicates);

            // Act
            var result = await builder.CountDistinctAsync(p => p.Age);

            // Assert
            Assert.AreEqual(2, result); // 30 and 25 (2 distinct ages)
        }

        [TestMethod]
        public async Task SumAsync_WithPredicate_ReturnsCorrectSum()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age > 30;

            // Act
            var result = await _builder.SumAsync("Salary", predicate);

            // Assert
            Assert.AreEqual(113000, result); // Bob (55000) + Charlie (58000)
        }

        [TestMethod]
        public async Task SumAsync_WithSelectorAndPredicate_ReturnsCorrectSum()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age < 30;

            // Act
            var result = await _builder.SumAsync(p => p.Salary, predicate);

            // Assert
            Assert.AreEqual(125000, result); // Jane (60000) + Alice (65000)
        }

        [TestMethod]
        public async Task SumDistinctAsync_ByField_ReturnsCorrectSum()
        {
            // Arrange
            var testDataWithDuplicates = new List<Person>
            {
                new Person { Id = "1", Name = "John", Salary = 50000 },
                new Person { Id = "2", Name = "Jane", Salary = 60000 },
                new Person { Id = "3", Name = "Bob", Salary = 50000 }, // Duplicate salary
                new Person { Id = "4", Name = "Alice", Salary = 70000 }
            };
            var builder = new TestFluentQueryBuilder<Person>(testDataWithDuplicates);

            // Act
            var result = await builder.SumDistinctAsync("Salary");

            // Assert
            Assert.AreEqual(180000, result); // 50000 + 60000 + 70000 (distinct salaries)
        }

        [TestMethod]
        public async Task SumDistinctAsync_BySelector_ReturnsCorrectSum()
        {
            // Arrange
            var testDataWithDuplicates = new List<Person>
            {
                new Person { Id = "1", Name = "John", Salary = 50000 },
                new Person { Id = "2", Name = "Jane", Salary = 60000 },
                new Person { Id = "3", Name = "Bob", Salary = 50000 }, // Duplicate salary
                new Person { Id = "4", Name = "Alice", Salary = 70000 }
            };
            var builder = new TestFluentQueryBuilder<Person>(testDataWithDuplicates);

            // Act
            var result = await builder.SumDistinctAsync(p => p.Salary);

            // Assert
            Assert.AreEqual(180000, result); // 50000 + 60000 + 70000 (distinct salaries)
        }

        [TestMethod]
        public async Task AverageAsync_WithPredicate_ReturnsCorrectAverage()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age >= 30;

            // Act
            var result = await _builder.AverageAsync("Salary", predicate);

            // Assert
            Assert.AreEqual(54333.33m, Math.Round(result, 2)); // (50000 + 55000 + 58000) / 3
        }

        [TestMethod]
        public async Task AverageAsync_WithSelectorAndPredicate_ReturnsCorrectAverage()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age < 30;

            // Act
            var result = await _builder.AverageAsync(p => p.Salary, predicate);

            // Assert
            Assert.AreEqual(62500, result); // (60000 + 65000) / 2
        }

        [TestMethod]
        public async Task MinAsync_WithPredicate_ReturnsCorrectMin()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age > 25;

            // Act
            var result = await _builder.MinAsync<decimal>("Salary", predicate);

            // Assert
            Assert.AreEqual(50000, result); // John's salary is the minimum among those > 25
        }

        [TestMethod]
        public async Task MinAsync_WithSelectorAndPredicate_ReturnsCorrectMin()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age > 30;

            // Act
            var result = await _builder.MinAsync(p => p.Age, predicate);

            // Assert
            Assert.AreEqual(32, result); // Charlie's age is the minimum among those > 30
        }

        [TestMethod]
        public async Task MaxAsync_WithPredicate_ReturnsCorrectMax()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Age < 35;

            // Act
            var result = await _builder.MaxAsync<decimal>("Salary", predicate);

            // Assert
            Assert.AreEqual(65000, result); // Alice's salary is the maximum among those < 35
        }

        [TestMethod]
        public async Task MaxAsync_WithSelectorAndPredicate_ReturnsCorrectMax()
        {
            // Arrange
            Expression<Func<Person, bool>> predicate = p => p.Salary > 55000;

            // Act
            var result = await _builder.MaxAsync(p => p.Age, predicate);

            // Assert
            Assert.AreEqual(32, result); // Charlie's age is the maximum among those with salary > 55000
        }

        [TestMethod]
        public async Task GroupBy_WithHaving_FiltersGroupsCorrectly()
        {
            // Arrange
            var testDataForGrouping = new List<Person>
            {
                new Person { Id = "1", Name = "John", Department = "IT", Salary = 50000 },
                new Person { Id = "2", Name = "Jane", Department = "IT", Salary = 60000 },
                new Person { Id = "3", Name = "Bob", Department = "HR", Salary = 45000 },
                new Person { Id = "4", Name = "Alice", Department = "IT", Salary = 65000 },
                new Person { Id = "5", Name = "Charlie", Department = "Finance", Salary = 55000 }
            };
            var builder = new TestFluentQueryBuilder<Person>(testDataForGrouping);

            // Act
            var result = await ((TestFluentQueryBuilder<Person>)builder
                .GroupBy("Department")
                .Having("count", 1)) // Only departments with more than 1 person
                .GroupedAsync();

            // Assert
            Assert.AreEqual(1, result.Count()); // Only IT department has more than 1 person
            var itGroup = result.First();
            Assert.AreEqual(3, itGroup.Count()); // IT has 3 people
        }

        [TestMethod]
        public async Task AggregationMethods_WithEmptyData_HandleGracefully()
        {
            // Arrange
            var emptyBuilder = new TestFluentQueryBuilder<Person>(new List<Person>());

            // Act & Assert
            Assert.AreEqual(0, await emptyBuilder.CountAsync(p => p.Age > 0));
            Assert.AreEqual(0, await emptyBuilder.SumAsync("Salary", p => p.Age > 0));
            Assert.AreEqual(0, await emptyBuilder.AverageAsync("Salary", p => p.Age > 0));
            Assert.AreEqual(default(decimal), await emptyBuilder.MinAsync<decimal>("Salary", p => p.Age > 0));
            Assert.AreEqual(default(decimal), await emptyBuilder.MaxAsync<decimal>("Salary", p => p.Age > 0));
        }
    }

    // Test model for aggregation tests
    public class Person
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public string Department { get; set; } = string.Empty;
    }
}