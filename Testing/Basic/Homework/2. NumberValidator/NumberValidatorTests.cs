
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

/// <summary>
/// Тесты для класса NumberValidator
/// Все тесты сгруппированы в 6 отдельных классов:
/// 1.Тесты для конструктора класса
/// 2.Тесты метода IsValid для нуля
/// 3.Тесты метода IsValid для проверки чисел на допустимость по точности
/// 4.Тесты метода IsValid для проверки чисел с положительным знаком
/// 5.Тесты метода IsValid для проверки чисел с отрицательным знаком
/// 6.Тесты метода IsValid для проверки чисел на допустимость формата
/// </summary>
[TestFixture]
public class NumberValidatorTests
{
    [TestFixture]
    public class NumberValidatorConstructorTests
    {
        [Test]
        public void NumberValidator_ThrowsException_WithNegativePrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(-1, 2, true));
        }

        [Test]
        public void NumberValidator_ThrowsException_WithZeroPrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(0, 2, true));
        }

        [Test]
        public void NumberValidator_DoesNotThrowException_WithPositivePrecision()
        {
            Assert.DoesNotThrow(() => new NumberValidator(1, 0, true));
        }

        [Test]
        public void NumberValidator_ThrowsException_WithScaleEqualToPrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(3, 3, false));
        }

        [Test]
        public void NumberValidator_ThrowsException_WithScaleExceedingPrecision()
        {
            Assert.Throws<ArgumentException>(() => new NumberValidator(3, 10, false));
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidMethodTestsWithZero
    {
        [Test]
        public void IsValidNumber_ReturnsTrue_WithZeroOfValidPrecision()
        {
            ClassicAssert.IsTrue(new NumberValidator(4, 3, true).IsValidNumber("0.00"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithZero()
        {
            ClassicAssert.IsTrue(new NumberValidator(3, 2, true).IsValidNumber("0"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithPositiveZero()
        {
            ClassicAssert.IsTrue(new NumberValidator(3, 2, true).IsValidNumber("+0"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithNegativeZero()
        {
            ClassicAssert.IsTrue(new NumberValidator(3, 2, false).IsValidNumber("-0"));
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidMethodPrecisionAndScaleTests
    {
        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberAtLimitPrecision()
        {
            ClassicAssert.IsTrue(new NumberValidator(4, 3, true).IsValidNumber("12.34"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberAtLimitScale()
        {
            ClassicAssert.IsTrue(new NumberValidator(5, 3, true).IsValidNumber("1.234"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNumberExceedingPrecision()
        {
            ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("12.34"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNumberFracPartExceedingScale()
        {
            ClassicAssert.IsFalse(new NumberValidator(17, 2, true).IsValidNumber("1.2345"));
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidMethodWithPositiveSign
    {
        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberWithPositiveSignOfValidPrecision()
        {
            ClassicAssert.IsTrue(new NumberValidator(5, 3, true).IsValidNumber("+1.23"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberWithPositiveSignAtLimitPrecision()
        {
            ClassicAssert.IsTrue(new NumberValidator(4, 2, true).IsValidNumber("+1.23"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberWithPositiveSignAtLimitScale()
        {
            ClassicAssert.IsTrue(new NumberValidator(4, 1, true).IsValidNumber("+1.2"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNumberWithPositiveSignExceedingPrecision()
        {
            ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber("+10.00"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNumberWithPositiveSignExceedingScale()
        {
            ClassicAssert.IsFalse(new NumberValidator(6, 1, true).IsValidNumber("+10.00"));
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidMethodWithNegativeSign
    {

        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberWithNegativeSignOfValidPrecision()
        {
            ClassicAssert.IsTrue(new NumberValidator(5, 3, false).IsValidNumber("-1.23"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberWithNegativeSignAtLimitPrecision()
        {
            ClassicAssert.IsTrue(new NumberValidator(4, 2, false).IsValidNumber("-1.2"));
        }

        [Test]
        public void IsValidNumber_ReturnsTrue_WithNumberWithNegativeSignAtLimitScale()
        {
            ClassicAssert.IsTrue(new NumberValidator(5, 2, false).IsValidNumber("-1.23"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNumberWithNegativeSignExceedingPrecision()
        {
            ClassicAssert.IsFalse(new NumberValidator(3, 2, false).IsValidNumber("-1.23"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNumberWithNegativeSignExceedingScale()
        {
            ClassicAssert.IsFalse(new NumberValidator(6, 2, false).IsValidNumber("-1.235"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNegativeNumberWhenOnlyPositivesAllowed()
        {
            ClassicAssert.IsFalse(new NumberValidator(5, 2, true).IsValidNumber("-0.00"));
        }
    }

    [TestFixture]
    public class NumberValidatorIsValidMethodStringFormatTests
    {
        [Test]
        public void IsValidNumber_ReturnsFalse_WithStringOfCharacters()
        {
            ClassicAssert.IsFalse(new NumberValidator(5, 2, true).IsValidNumber("ab.cd"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithCharacterInNumber()
        {
            //Вместо 0 записали большую букву 'о'
            ClassicAssert.IsFalse(new NumberValidator(5, 2, true).IsValidNumber("12.O4"));
        }

        [Test]
        public void IsValidNumber_ReturnsTure_WithNumberWithDifferentDividers()
        {
            ClassicAssert.IsTrue(new NumberValidator(5, 2, true).IsValidNumber("0,00"));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithNull()
        {
            ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(null));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithEmptyString()
        {
            ClassicAssert.IsFalse(new NumberValidator(3, 2, true).IsValidNumber(""));
        }

        [Test]
        public void IsValidNumber_ReturnsFalse_WithMultipleDividers()
        {
            ClassicAssert.IsFalse(new NumberValidator(10, 6, true).IsValidNumber("12.34.56"));
        }
    }
}