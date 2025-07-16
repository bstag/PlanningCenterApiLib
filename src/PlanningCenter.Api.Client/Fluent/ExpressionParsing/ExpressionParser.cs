using System.Linq.Expressions;
using System.Reflection;
using PlanningCenter.Api.Client.Models.Core;

namespace PlanningCenter.Api.Client.Fluent.ExpressionParsing;

/// <summary>
/// Advanced expression parser that converts LINQ expressions to Planning Center API parameters.
/// Supports complex expressions including comparisons, method calls, and property access.
/// </summary>
public static class ExpressionParser
{
    /// <summary>
    /// Converts a boolean expression to API filter parameters.
    /// </summary>
    public static FilterResult ParseFilter(Expression<Func<Person, bool>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParseBooleanExpression(expression.Body);
        }
        catch (Exception ex)
        {
            // If parsing fails, return empty result rather than throwing
            // This allows graceful degradation to client-side filtering
            return new FilterResult();
        }
    }

    /// <summary>
    /// Converts a property access expression to API include parameter.
    /// </summary>
    public static string ParseInclude(Expression<Func<Person, object>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParsePropertyAccess(expression.Body);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Converts a property access expression to API sort parameter.
    /// </summary>
    public static string ParseSort(Expression<Func<Person, object>> expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));

        try
        {
            return ParsePropertyAccess(expression.Body);
        }
        catch
        {
            return string.Empty;
        }
    }

    private static FilterResult ParseBooleanExpression(Expression expression)
    {
        switch (expression)
        {
            case BinaryExpression binary:
                return ParseBinaryExpression(binary);
            
            case MethodCallExpression method:
                return ParseMethodCallExpression(method);
            
            case UnaryExpression unary when unary.NodeType == ExpressionType.Not:
                var innerResult = ParseBooleanExpression(unary.Operand);
                return innerResult.Negate();
            
            default:
                return new FilterResult();
        }
    }

    private static FilterResult ParseBinaryExpression(BinaryExpression binary)
    {
        // Handle logical operators (AND/OR)
        if (binary.NodeType == ExpressionType.AndAlso)
        {
            var left = ParseBooleanExpression(binary.Left);
            var right = ParseBooleanExpression(binary.Right);
            return FilterResult.Combine(left, right, LogicalOperator.And);
        }

        if (binary.NodeType == ExpressionType.OrElse)
        {
            var left = ParseBooleanExpression(binary.Left);
            var right = ParseBooleanExpression(binary.Right);
            return FilterResult.Combine(left, right, LogicalOperator.Or);
        }

        // Handle comparison operators
        var propertyName = ExtractPropertyName(binary.Left);
        var value = ExtractValue(binary.Right);

        if (string.IsNullOrEmpty(propertyName) || value == null)
            return new FilterResult();

        var apiFieldName = ConvertPropertyNameToApiField(propertyName);
        var filterOperator = ConvertExpressionTypeToFilterOperator(binary.NodeType);

        return new FilterResult
        {
            Field = apiFieldName,
            Operator = filterOperator,
            Value = value
        };
    }

    private static FilterResult ParseMethodCallExpression(MethodCallExpression method)
    {
        var propertyName = ExtractPropertyName(method.Object);
        if (string.IsNullOrEmpty(propertyName))
            return new FilterResult();

        var apiFieldName = ConvertPropertyNameToApiField(propertyName);

        switch (method.Method.Name)
        {
            case "Contains":
                var containsValue = ExtractValue(method.Arguments[0]);
                return new FilterResult
                {
                    Field = apiFieldName,
                    Operator = FilterOperator.Contains,
                    Value = containsValue
                };

            case "StartsWith":
                var startsWithValue = ExtractValue(method.Arguments[0]);
                return new FilterResult
                {
                    Field = apiFieldName,
                    Operator = FilterOperator.StartsWith,
                    Value = startsWithValue
                };

            case "EndsWith":
                var endsWithValue = ExtractValue(method.Arguments[0]);
                return new FilterResult
                {
                    Field = apiFieldName,
                    Operator = FilterOperator.EndsWith,
                    Value = endsWithValue
                };

            default:
                return new FilterResult();
        }
    }

    private static string ParsePropertyAccess(Expression expression)
    {
        // Handle unboxing for object return types
        if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            expression = unary.Operand;
        }

        if (expression is MemberExpression member)
        {
            var propertyName = member.Member.Name;
            return ConvertPropertyNameToApiField(propertyName);
        }

        return string.Empty;
    }

    private static string? ExtractPropertyName(Expression? expression)
    {
        if (expression is MemberExpression member)
        {
            return member.Member.Name;
        }

        return null;
    }

    private static object? ExtractValue(Expression expression)
    {
        try
        {
            // Handle constants
            if (expression is ConstantExpression constant)
            {
                return constant.Value;
            }

            // Handle member access (e.g., DateTime.Now, variables)
            if (expression is MemberExpression member)
            {
                if (member.Expression is ConstantExpression memberConstant)
                {
                    var container = memberConstant.Value;
                    if (member.Member is FieldInfo field)
                    {
                        return field.GetValue(container);
                    }
                    if (member.Member is PropertyInfo property)
                    {
                        return property.GetValue(container);
                    }
                }
            }

            // Handle method calls (e.g., DateTime.Now.AddDays(-30))
            if (expression is MethodCallExpression methodCall)
            {
                var compiled = Expression.Lambda(methodCall).Compile();
                return compiled.DynamicInvoke();
            }

            // Fallback: compile and execute the expression
            var lambda = Expression.Lambda(expression);
            var compiledLambda = lambda.Compile();
            return compiledLambda.DynamicInvoke();
        }
        catch
        {
            return null;
        }
    }

    private static string ConvertPropertyNameToApiField(string propertyName)
    {
        // Convert C# property names to Planning Center API field names
        return propertyName switch
        {
            nameof(Person.FirstName) => "first_name",
            nameof(Person.LastName) => "last_name",
            nameof(Person.FullName) => "name",
            nameof(Person.PrimaryEmail) => "primary_email",
            nameof(Person.Birthdate) => "birthdate",
            nameof(Person.Anniversary) => "anniversary",
            nameof(Person.Gender) => "gender",
            nameof(Person.Grade) => "grade",
            nameof(Person.Status) => "status",
            nameof(Person.CreatedAt) => "created_at",
            nameof(Person.UpdatedAt) => "updated_at",
            nameof(Person.Addresses) => "addresses",
            nameof(Person.Emails) => "emails",
            nameof(Person.PhoneNumbers) => "phone_numbers",
            _ => propertyName.ToLowerInvariant()
        };
    }

    private static FilterOperator ConvertExpressionTypeToFilterOperator(ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.Equal => FilterOperator.Equal,
            ExpressionType.NotEqual => FilterOperator.NotEqual,
            ExpressionType.GreaterThan => FilterOperator.GreaterThan,
            ExpressionType.GreaterThanOrEqual => FilterOperator.GreaterThanOrEqual,
            ExpressionType.LessThan => FilterOperator.LessThan,
            ExpressionType.LessThanOrEqual => FilterOperator.LessThanOrEqual,
            _ => FilterOperator.Equal
        };
    }
}

/// <summary>
/// Represents the result of parsing a filter expression.
/// </summary>
public class FilterResult
{
    public string Field { get; set; } = string.Empty;
    public FilterOperator Operator { get; set; } = FilterOperator.Equal;
    public object? Value { get; set; }
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
    public List<FilterResult> SubFilters { get; set; } = new();

    public bool IsEmpty => string.IsNullOrEmpty(Field) && !SubFilters.Any();

    public FilterResult Negate()
    {
        var negated = new FilterResult
        {
            Field = Field,
            Value = Value,
            LogicalOperator = LogicalOperator,
            SubFilters = SubFilters.Select(sf => sf.Negate()).ToList()
        };

        negated.Operator = Operator switch
        {
            FilterOperator.Equal => FilterOperator.NotEqual,
            FilterOperator.NotEqual => FilterOperator.Equal,
            FilterOperator.GreaterThan => FilterOperator.LessThanOrEqual,
            FilterOperator.GreaterThanOrEqual => FilterOperator.LessThan,
            FilterOperator.LessThan => FilterOperator.GreaterThanOrEqual,
            FilterOperator.LessThanOrEqual => FilterOperator.GreaterThan,
            FilterOperator.Contains => FilterOperator.NotContains,
            FilterOperator.NotContains => FilterOperator.Contains,
            _ => Operator
        };

        return negated;
    }

    public static FilterResult Combine(FilterResult left, FilterResult right, LogicalOperator logicalOperator)
    {
        if (left.IsEmpty) return right;
        if (right.IsEmpty) return left;

        return new FilterResult
        {
            LogicalOperator = logicalOperator,
            SubFilters = new List<FilterResult> { left, right }
        };
    }

    public string ToApiFilterString()
    {
        if (!SubFilters.Any())
        {
            // Simple filter
            var operatorString = Operator switch
            {
                FilterOperator.Equal => "",
                FilterOperator.NotEqual => "!",
                FilterOperator.GreaterThan => ">",
                FilterOperator.GreaterThanOrEqual => ">=",
                FilterOperator.LessThan => "<",
                FilterOperator.LessThanOrEqual => "<=",
                FilterOperator.Contains => "*",
                FilterOperator.NotContains => "!*",
                FilterOperator.StartsWith => "",
                FilterOperator.EndsWith => "",
                _ => ""
            };

            var valueString = Value?.ToString() ?? "";

            if (Operator == FilterOperator.Contains || Operator == FilterOperator.NotContains)
            {
                valueString = $"*{valueString}*";
            }
            else if (Operator == FilterOperator.StartsWith)
            {
                valueString = $"{valueString}*";
            }
            else if (Operator == FilterOperator.EndsWith)
            {
                valueString = $"*{valueString}";
            }

            return $"{operatorString}{valueString}";
        }

        // Complex filter with sub-filters
        // Note: Planning Center API has limited support for complex logical operations
        // For now, we'll just use the first sub-filter
        return SubFilters.First().ToApiFilterString();
    }
}

/// <summary>
/// Supported filter operators.
/// </summary>
public enum FilterOperator
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    NotContains,
    StartsWith,
    EndsWith
}

/// <summary>
/// Logical operators for combining filters.
/// </summary>
public enum LogicalOperator
{
    And,
    Or
}