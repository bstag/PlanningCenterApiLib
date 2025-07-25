using System;
using System.Linq.Expressions;
using PlanningCenter.Api.Client.Models.Core;
using PlanningCenter.Api.Client.Fluent.ExpressionParsing;
using System.Reflection;

namespace DebugTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing ExpressionParser...");
            
            // Test expression: p.Emails.Count() > 0
            Expression<Func<Person, bool>> expr1 = p => p.Emails.Count() > 0;
            Console.WriteLine($"\nExpression: p.Emails.Count() > 0");
            AnalyzeExpression(expr1.Body);
            var result1 = ExpressionParser.ParseFilter(expr1);
            Console.WriteLine($"Field: '{result1.Field}'");
            Console.WriteLine($"Operator: {result1.Operator}");
            Console.WriteLine($"Value: {result1.Value}");
            Console.WriteLine($"IsEmpty: {result1.IsEmpty}");
            
            // Test expression: p.Emails.Any()
            Expression<Func<Person, bool>> expr2 = p => p.Emails.Any();
            Console.WriteLine($"\nExpression: p.Emails.Any()");
            AnalyzeExpression(expr2.Body);
            var result2 = ExpressionParser.ParseFilter(expr2);
            Console.WriteLine($"Field: '{result2.Field}'");
            Console.WriteLine($"Operator: {result2.Operator}");
            Console.WriteLine($"Value: {result2.Value}");
            Console.WriteLine($"IsEmpty: {result2.IsEmpty}");
            
            // Test simple property access
            Expression<Func<Person, bool>> expr3 = p => p.FirstName == "John";
            Console.WriteLine($"\nExpression: p.FirstName == \"John\"");
            AnalyzeExpression(expr3.Body);
            var result3 = ExpressionParser.ParseFilter(expr3);
            Console.WriteLine($"Field: '{result3.Field}'");
            Console.WriteLine($"Operator: {result3.Operator}");
            Console.WriteLine($"Value: {result3.Value}");
            Console.WriteLine($"IsEmpty: {result3.IsEmpty}");
            
            // Test complex boolean expressions
            Console.WriteLine("\n=== Testing Complex Boolean Expressions ===");
            Expression<Func<Person, bool>> expr4 = p => p.Grade == "9" || p.Grade == "12";
            Console.WriteLine($"\nExpression: p.Grade == \"9\" || p.Grade == \"12\"");
            AnalyzeExpression(expr4.Body);
            var result4 = ExpressionParser.ParseFilter(expr4);
            Console.WriteLine($"Field: '{result4.Field}'");
            Console.WriteLine($"Operator: {result4.Operator}");
            Console.WriteLine($"Value: {result4.Value}");
            Console.WriteLine($"IsEmpty: {result4.IsEmpty}");
            
            Expression<Func<Person, bool>> expr5 = p => p.Grade == "9";
            Console.WriteLine($"\nExpression: p.Grade == \"9\"");
            AnalyzeExpression(expr5.Body);
            var result5 = ExpressionParser.ParseFilter(expr5);
            Console.WriteLine($"Field: '{result5.Field}'");
            Console.WriteLine($"Operator: {result5.Operator}");
            Console.WriteLine($"Value: {result5.Value}");
            Console.WriteLine($"IsEmpty: {result5.IsEmpty}");
            
            Expression<Func<Person, bool>> expr6 = p => p.Grade == "12";
            Console.WriteLine($"\nExpression: p.Grade == \"12\"");
            AnalyzeExpression(expr6.Body);
            var result6 = ExpressionParser.ParseFilter(expr6);
            Console.WriteLine($"Field: '{result6.Field}'");
            Console.WriteLine($"Operator: {result6.Operator}");
            Console.WriteLine($"Value: {result6.Value}");
            Console.WriteLine($"IsEmpty: {result6.IsEmpty}");
        }
        
        static void AnalyzeExpression(Expression expr)
        {
            Console.WriteLine($"Expression Type: {expr.NodeType}");
            Console.WriteLine($"Expression Class: {expr.GetType().Name}");
            
            if (expr is BinaryExpression binary)
            {
                Console.WriteLine($"Binary Left: {binary.Left.GetType().Name} - {binary.Left}");
                Console.WriteLine($"Binary Right: {binary.Right.GetType().Name} - {binary.Right}");
                
                if (binary.Left is MethodCallExpression leftMethod)
                {
                    Console.WriteLine($"Left Method: {leftMethod.Method.Name}");
                    Console.WriteLine($"Left Method Declaring Type: {leftMethod.Method.DeclaringType?.Name}");
                    Console.WriteLine($"Left Object: {leftMethod.Object?.GetType().Name} - {leftMethod.Object}");
                    Console.WriteLine($"Left Arguments Count: {leftMethod.Arguments.Count}");
                    
                    if (leftMethod.Arguments.Count > 0)
                    {
                        Console.WriteLine($"First Argument: {leftMethod.Arguments[0].GetType().Name} - {leftMethod.Arguments[0]}");
                        
                        if (leftMethod.Arguments[0] is MemberExpression argMember)
                        {
                            Console.WriteLine($"Arg Member: {argMember.Member.Name}");
                            Console.WriteLine($"Arg Member Expression: {argMember.Expression?.GetType().Name}");
                        }
                    }
                    
                    if (leftMethod.Object is MemberExpression leftMember)
                    {
                        Console.WriteLine($"Left Member: {leftMember.Member.Name}");
                        Console.WriteLine($"Left Member Expression: {leftMember.Expression?.GetType().Name}");
                    }
                }
            }
            else if (expr is MethodCallExpression method)
            {
                Console.WriteLine($"Method: {method.Method.Name}");
                Console.WriteLine($"Method Declaring Type: {method.Method.DeclaringType?.Name}");
                Console.WriteLine($"Object: {method.Object?.GetType().Name} - {method.Object}");
                Console.WriteLine($"Arguments Count: {method.Arguments.Count}");
                
                if (method.Arguments.Count > 0)
                {
                    Console.WriteLine($"First Argument: {method.Arguments[0].GetType().Name} - {method.Arguments[0]}");
                    
                    if (method.Arguments[0] is MemberExpression argMember)
                    {
                        Console.WriteLine($"Arg Member: {argMember.Member.Name}");
                        Console.WriteLine($"Arg Member Expression: {argMember.Expression?.GetType().Name}");
                    }
                }
                
                if (method.Object is MemberExpression member)
                {
                    Console.WriteLine($"Member: {member.Member.Name}");
                    Console.WriteLine($"Member Expression: {member.Expression?.GetType().Name}");
                }
            }
        }
    }
}