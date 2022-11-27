using System.Text;

namespace App.Repositories.Helpers;

public sealed class QueryConditionBuilder
{
   private readonly List<QueryCondition> _conditions = new();

   public QueryConditionBuilder() { }

   public QueryConditionBuilder(ConditionOperators conditionOperator, string condition)
   {
      _conditions.Add(new QueryCondition(conditionOperator, condition));
   }

   public QueryConditionBuilder Add(ConditionOperators conditionOperator, string condition)
   {
      _conditions.Add(new QueryCondition(conditionOperator, condition));
      return this;
   }

   public QueryConditionBuilder Add(QueryCondition condition)
   {
      _conditions.Add(condition);
      return this;
   }

   public QueryConditionBuilder(QueryCondition condition)
   {
      _conditions.Add(condition);
   }

   public QueryConditionBuilder AddRange(params QueryCondition[] conditions)
   {
      _conditions.AddRange(conditions);
      return this;
   }

   public QueryConditionBuilder AddRange(ConditionOperators conditionOperator, params string[] conditions)
   {
      foreach (var simpleCondition in conditions)
      {
         _conditions.Add(new QueryCondition(conditionOperator, simpleCondition));
      }
      return this;
   }

   public override string ToString()
   {
      if (_conditions.Count == 0) { return string.Empty; }

      var sb = new StringBuilder();

      sb.Append(GetQueryConditionString(_conditions[0], true));

      if (_conditions.Count == 1) { return sb.ToString(); }

      foreach (var condition in _conditions.Skip(1))
      {
         sb.Append(' ').Append(GetQueryConditionString(condition));
      }

      return sb.ToString();
   }

   private static string GetQueryConditionString(QueryCondition condition, bool withoutOperator = false)
   {
      if (!string.IsNullOrEmpty(condition.Single))
      {
         return withoutOperator ? condition.Single : $"{condition.Operator} {condition.Single}";
      }

      var conditionGroup = condition.Group?.ToArray();

      if (conditionGroup is null || conditionGroup.Length == 0) { return string.Empty; }

      var queryConditionBuilder = new QueryConditionBuilder();
      queryConditionBuilder.AddRange(conditionGroup);

      var conditionGroupString = queryConditionBuilder.ToString();

      if (string.IsNullOrEmpty(conditionGroupString)) { return string.Empty; }

      return withoutOperator ? $"({conditionGroupString})" : $"{condition.Operator} ({conditionGroupString})";
   }
}

public enum ConditionOperators
{
   AND,
   OR,
}

public class QueryCondition
{
   public ConditionOperators Operator { get; }
   public string? Single { get; }
   public IEnumerable<QueryCondition>? Group { get; }

   public QueryCondition(ConditionOperators conditionOperator, string condition)
   {
      Operator = conditionOperator;
      Single = condition;
   }

   public QueryCondition(ConditionOperators conditionOperator, IEnumerable<QueryCondition> conditionGroup)
   {
      Operator = conditionOperator;
      Group = conditionGroup;
   }
}
