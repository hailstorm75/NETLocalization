using Bogus;
using Localization.Shared.Models;
using Shouldly;
using Xunit;

namespace UT.Shared;

public sealed class LEnumTests
{
    private enum TestEnum { Value1, Value2 }

    [Fact]
    public void LEnum_Invalid_IsSingleton()
    {
        // Arrange & Act
        var invalid1 = LEnum.INVALID;
        var invalid2 = LEnum.INVALID;
        // Assert
        invalid1.ShouldBeSameAs(invalid2);
    }

    [Fact]
    public void LEnum_Constructor_SetsEnumField()
    {
        // Arrange
        var faker = new Faker();
        var enumValue = TestEnum.Value1;
        // Act
        var lEnum = new LEnum(enumValue);
        // Assert
        lEnum.EnumField.ShouldBe(enumValue);
    }
}
