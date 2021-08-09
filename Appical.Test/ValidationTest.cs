using Appical.Persistence.Validator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appical.Test
{
    [TestFixture]
    public class ValidationTest
    {
        [TestCase]
        public void EmptyGuid()
        {
            List<string> errors = BaseValidator.ValidateGuid(Guid.Empty);
            Assert.IsFalse(errors.Count == 0);
        }

        [TestCase("", 2, true)]
        [TestCase("aaa", 2, false)]
        [TestCase("aaaaaa", 20, true)]
        public void StringLength(string testValue, int maxLength, bool isValid)
        {
            List<string> errors = BaseValidator.ValidateStringLength(testValue, maxLength, "testCase");

            if (isValid) 
                Assert.IsTrue(errors.Count == 0);
            else 
                Assert.IsFalse(errors.Count == 0);
        }

        [TestCase(20, true)]
        [TestCase(-20, false)]
        public void DecimalNotNegative(decimal testValue, bool isValid)
        {
            List<string> errors = BaseValidator.ValidateDecimalNotNegative(testValue, "testCase");

            if (isValid) 
                Assert.IsTrue(errors.Count == 0);
            else 
                Assert.IsFalse(errors.Count == 0);
        }        
        
        public void DateNotFuture()
        {
            List<string> errors = BaseValidator.ValidateDateNotFuture(DateTime.Parse("2021-08-07"), "testCase");
            Assert.IsTrue(errors.Count == 0);

            errors = BaseValidator.ValidateDateNotFuture(DateTime.Parse("2022-08-07"), "testCase");
            Assert.IsFalse(errors.Count == 0);
        }
    }
}
