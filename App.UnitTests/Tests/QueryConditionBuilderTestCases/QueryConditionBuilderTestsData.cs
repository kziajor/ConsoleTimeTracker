using App.Repositories.Helpers;
using System.Collections;
using System.Collections.Generic;

namespace App.UnitTests.Tests.QueryConditionBuilderTestCases;

public class SingleSimpleConditionTestData : IEnumerable
{
   public IEnumerator GetEnumerator()
   {
      yield return new object[] { ConditionOperators.AND, "test = '1'", "test = '1'" };
      yield return new object[] { ConditionOperators.OR, "test = '1'", "test = '1'" };
      yield return new object[] { ConditionOperators.AND, "test LIKE '%1%'", "test LIKE '%1%'" };
   }
}

public class MultipleSimpleConditionTestData : IEnumerable
{
   public IEnumerator GetEnumerator()
   {
      yield return new object[]
      {
         new List<QueryCondition>
         {
            new QueryCondition(ConditionOperators.AND, "test1 = '1'"),
            new QueryCondition(ConditionOperators.AND, "test2 = '2'"),
            new QueryCondition(ConditionOperators.AND, "test3 = '3'"),
         },
         "test1 = '1' AND test2 = '2' AND test3 = '3'"
      };

      yield return new object[]
      {
         new List<QueryCondition>
         {
            new QueryCondition(ConditionOperators.AND, "test1 = '1'"),
            new QueryCondition(ConditionOperators.OR, "test2 = '2'"),
            new QueryCondition(ConditionOperators.AND, "test3 = '3'"),
         },
         "test1 = '1' OR test2 = '2' AND test3 = '3'"
      };
   }
}

public class MultipleComplexConditionTestData : IEnumerable
{
   public IEnumerator GetEnumerator()
   {
      yield return new object[]
      {
         new List<QueryCondition>
         {
            new QueryCondition(ConditionOperators.AND, "test1 = '1'"),
            new QueryCondition(ConditionOperators.AND, new QueryCondition[]
            {
               new QueryCondition(ConditionOperators.AND, "subtest1_1 = '1.1'"),
               new QueryCondition(ConditionOperators.OR, "subtest1_2 = '1.2'"),
               new QueryCondition(ConditionOperators.AND, "subtest1_3 = '1.3'"),
            }),
         },
         "test1 = '1' AND (subtest1_1 = '1.1' OR subtest1_2 = '1.2' AND subtest1_3 = '1.3')"
      };

      yield return new object[]
      {
         new List<QueryCondition>
         {
            new QueryCondition(ConditionOperators.AND, "test1 = '1'"),
            new QueryCondition(ConditionOperators.OR, new QueryCondition[]
            {
               new QueryCondition(ConditionOperators.AND, "subtest1_1 LIKE '%1.1'"),
               new QueryCondition(ConditionOperators.AND, new QueryCondition[]
               {
                  new QueryCondition(ConditionOperators.AND, "subtest1_1_1 IS NOT NULL"),
                  new QueryCondition(ConditionOperators.AND, "subtest1_1_2 IS NULL"),
                  new QueryCondition(ConditionOperators.OR, "subtest1_1_3 IN ('1', '2', '3')"),
               }),
               new QueryCondition(ConditionOperators.AND, "subtest1_2 = '1.2'"),
               new QueryCondition(ConditionOperators.AND, new QueryCondition[]
               {
                  new QueryCondition(ConditionOperators.AND, "subtest1_2_1 IS NOT NULL"),
                  new QueryCondition(ConditionOperators.AND, "subtest1_2_2 IS NULL"),
                  new QueryCondition(ConditionOperators.OR, "subtest1_2_3 IN ('1', '2', '3')"),
               }),
            }),
            new QueryCondition(ConditionOperators.AND, "test2 = '2'"),
            new QueryCondition(ConditionOperators.OR, new QueryCondition[]
            {
               new QueryCondition(ConditionOperators.AND, "subtest2_1 LIKE '%1.1'"),
               new QueryCondition(ConditionOperators.AND, new QueryCondition[]
               {
                  new QueryCondition(ConditionOperators.AND, "subtest2_1_1 IS NOT NULL"),
                  new QueryCondition(ConditionOperators.AND, "subtest2_1_2 IS NULL"),
                  new QueryCondition(ConditionOperators.OR, "subtest2_1_3 IN ('1', '2', '3')"),
               }),
            }),
         },
         "test1 = '1' OR (subtest1_1 LIKE '%1.1' AND (subtest1_1_1 IS NOT NULL AND subtest1_1_2 IS NULL OR subtest1_1_3 IN ('1', '2', '3')) AND subtest1_2 = '1.2' AND (subtest1_2_1 IS NOT NULL AND subtest1_2_2 IS NULL OR subtest1_2_3 IN ('1', '2', '3'))) AND test2 = '2' OR (subtest2_1 LIKE '%1.1' AND (subtest2_1_1 IS NOT NULL AND subtest2_1_2 IS NULL OR subtest2_1_3 IN ('1', '2', '3')))"
      };
   }
}
