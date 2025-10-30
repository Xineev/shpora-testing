
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HomeExercise.Tasks.NumberValidator;

/// <summary>
/// Тесты для класса NumberValidator
/// Все тесты сгруппированы в 3 отдельных классов:
/// 1.Тесты для конструктора класса
/// 2.Общие тесты на функциональность метода IsValid
/// 3.Тесты на верную обработку форматов приходящих чисел
/// </summary>
[TestFixture]
public class NumberValidatorTests
{
    [TestFixture]
    public class NumberValidatorConstructorTests
    {
        [TestCase(-1, 2, true, "precision must be a positive number", TestName = "Negative precision given")]
        [TestCase(0, 2, true, "precision must be a positive number", TestName = "Zero precision given")]
        public void NumberValidator_ThrowsException_WithInvalidPrecision(int precision, int scale, bool onlyPositive, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new NumberValidator(precision, scale, onlyPositive));

            Assert.That(exception.Message, Contains.Substring(expectedMessage));
        }

        [TestCase(3, 3, true, "precision must be a non-negative number less or equal than precision", 
            TestName = "Scale equals precision")]
        [TestCase(3, 10, true, "precision must be a non-negative number less or equal than precision",
            TestName = "Scale bigger than precision")]
        public void NumberValidator_ThrowsException_WithInvalidScale(int precision, int scale, bool onlyPositive, string expectedMessage)
        {
            var exception = Assert.Throws<ArgumentException>(() =>
                new NumberValidator(precision, scale, onlyPositive));

            Assert.That(exception.Message, Contains.Substring(expectedMessage));
        }

        [Test]
        public void NumberValidator_DoesNotThrowException_WithPositivePrecision()
        {
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidMethodTests
    {
        [TestCase("0.00", 4, 3, true, TestName = "Zero with decimal places within limits")]
        [TestCase("0", 3, 2, true, TestName = "Simple zero without decimal")]
        [TestCase("+0", 3, 2, true, TestName = "Positive zero without decimal")]
        [TestCase("-0", 3, 2, false, TestName = "Negative zero when negatives allowed")]
        public void IsValidNumber_ReturnsTrue_WithZeroOfDifferentFormats(string number, int precision, int scale, bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            Assert.That(validator.IsValidNumber(number), Is.True,
                    $"Failed for: '{number}' with precision={precision}, scale={scale}, onlyPositive={onlyPositive}");
        }

        [TestCase("12.34", 4, 3, true, TestName = "Number at precision limit with decimals")]
        [TestCase("123.4", 4, 2, true, TestName = "Integer at precision limit")]
        [TestCase("1.234", 5, 3, true, TestName = "Number at scale limit")]
        [TestCase("123.45", 5, 2, true, TestName = "Number using full precision and scale")]
        [TestCase("+1.234", 6, 3, true, TestName = "Positive number with sign at scale limit")]
        [TestCase("-1.234", 6, 3, false, TestName = "Negative number with sign at scale limit")]
        [TestCase("+12.34", 5, 2, true, TestName = "Positive number with sign at precision limit")]
        [TestCase("-12.34", 5, 2, false, TestName = "Negative number with sign at precision limit")]
        [TestCase("+12.34", 7, 3, true, TestName = "Positive number with sign within precision and scale")]
        [TestCase("-12.34", 7, 3, false, TestName = "Negative number with sign within precision and scale")]
        public void IsValidNumber_ReturnsTrue_WithNumberWithinPrecisionAndScale(string number, int precision, int scale, bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            Assert.That(validator.IsValidNumber(number), Is.True,
                $"Should validate: '{number}' with precision={precision}, scale={scale}, onlyPositive={onlyPositive}");
        }

        [TestCase("1234", 3, 0, true, TestName = "Exceeds precision by 1 for integer")]
        [TestCase("12.34", 3, 2, true, TestName = "Exceeds precision by 1 for decimal")]
        [TestCase("+123", 3, 0, true, TestName = "Exceeds precision due to positive sign")]
        [TestCase("-123", 3, 0, false, TestName = "Exceeds precision due to negative sign")]
        public void IsValidNumber_ReturnsFalse_WithNumberWithExceedingPrecision(string number, int precision, int scale, bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            Assert.That(validator.IsValidNumber(number), Is.False,
                $"Should detect precision exceed: '{number}' with precision={precision}, scale={scale}");
        }

        [TestCase("12.345", 5, 2, true, TestName = "Number exceeds scale by 1")]
        [TestCase("0.1234", 5, 3, true, TestName = "Number exceeds scale by 1 with leading zero")]
        [TestCase("1.23456", 10, 4, true, TestName = "Number exceeds scale by multiple digits")]
        [TestCase("+12.345", 6, 2, true, TestName = "Number exceeds scale by 1 with positive sign")]
        [TestCase("-12.345", 6, 2, true, TestName = "Number exceeds scale by 1 with negative sign")]
        public void IsValidNumber_ReturnsFalse_WithNumberWithExceedingScale(string number, int precision, int scale, bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            Assert.That(validator.IsValidNumber(number), Is.False,
                $"Should detect scale exceed: '{number}' with precision={precision}, scale={scale}");
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNegativeNumberWhenOnlyPositivesAllowed()
        {
            Assert.That(new NumberValidator(5, 2, true).IsValidNumber("-0.00"), Is.False);
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidMethodStringFormatTests
    {
        [TestCase("ab.cd", 5, 2, true, TestName = "String of characters")]
        [TestCase("12.O4", 5, 2, true, TestName = "String with one inserted character")]
        [TestCase(null, 5, 3, true, TestName = "Given string is null")]
        [TestCase("", 10, 4, true, TestName = "Empty string was given")]
        [TestCase("12.34.56", 6, 2, true, TestName = "String with several dividers")]
        public void IsValidNumber_ReturnsFalse_WithStringsOfInvalidFormat(string number, int precision, int scale, bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            Assert.That(validator.IsValidNumber(number), Is.False,
                $"Should detect invalid number format: '{number}'");
        }

        [TestCase("0,00", 5, 2, true, TestName = "Number with the comma divider")]
        public void IsValidNumber_ReturnsTrue_WithNumberWithDifferentDividers(string number, int precision, int scale, bool onlyPositive)
        {
            var validator = new NumberValidator(precision, scale, onlyPositive);
            Assert.That(validator.IsValidNumber(number), Is.True,
                "Should validate number of distinct format: '{number}'");
        }
    }
}