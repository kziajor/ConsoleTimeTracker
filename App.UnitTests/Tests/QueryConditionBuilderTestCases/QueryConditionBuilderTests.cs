using App.Repositories.Helpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace App.UnitTests.Tests.QueryConditionBuilderTestCases
{
   [TestFixture]
   public class QueryConditionBuilderTests
   {
      [TestCaseSource(typeof(SingleSimpleConditionTestData))]
      public void When_SingleCondition_CorrectSqlConditionReturned(ConditionOperators conditionOperator, string condition, string expectedValue)
      {
         #region Arrange

         var queryConditionBuilder = new QueryConditionBuilder(conditionOperator, condition);

         #endregion

         #region Act

         var result = queryConditionBuilder.ToString();

         #endregion

         #region Assert

         Assert.AreEqual(expectedValue, result);

         #endregion
      }

      [TestCaseSource(typeof(MultipleSimpleConditionTestData))]
      public void When_MultipleSimpleConditions_CorrectSqlConditionReturned(IEnumerable<QueryCondition> conditions, string expectedValue)
      {
         #region Arrange

         var queryConditionBuilder = new QueryConditionBuilder();
         queryConditionBuilder.AddRange(conditions.ToArray());

         #endregion

         #region Act

         var result = queryConditionBuilder.ToString();

         #endregion

         #region Assert

         Assert.AreEqual(expectedValue, result);

         #endregion
      }

      [TestCaseSource(typeof(MultipleComplexConditionTestData))]
      public void When_MultipleComplexConditions_CorrectSqlConditionReturned(IEnumerable<QueryCondition> conditions, string expectedValue)
      {
         #region Arrange

         var queryConditionBuilder = new QueryConditionBuilder();
         queryConditionBuilder.AddRange(conditions.ToArray());

         #endregion

         #region Act

         var result = queryConditionBuilder.ToString();

         #endregion

         #region Assert

         Assert.AreEqual(expectedValue, result);

         #endregion
      }
   }
}