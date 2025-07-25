using System;
using System.Linq.Expressions;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Fluent.ExpressionParsing;

class Program
{
    static void Main()
    {
        // Test the expression that's failing
        Expression<Func<Person, bool>> expression = p => p.Emails.Count() > 0;
        
        Console.WriteLine("Testing expression: p => p.Emails.Count() > 0");
        
        var result = ExpressionParser.ParseFilter(expression);
        
        Console.WriteLine($"Field: '{result.Field}'");
        Console.WriteLine($"Operator: {result.Operator}");
        Console.WriteLine($"Value: {result.Value}");
        Console.WriteLine($"IsEmpty: {result.IsEmpty}");
        
        // Test simpler expression
        Expression<Func<Person, bool>> simpleExpression = p => p.Emails.Any();
        Console.WriteLine("\nTesting expression: p => p.Emails.Any()");
        
        var simpleResult = ExpressionParser.ParseFilter(simpleExpression);
        
        Console.WriteLine($"Field: '{simpleResult.Field}'");
        Console.WriteLine($"Operator: {simpleResult.Operator}");
        Console.WriteLine($"Value: {simpleResult.Value}");
        Console.WriteLine($"IsEmpty: {simpleResult.IsEmpty}");
    }
}