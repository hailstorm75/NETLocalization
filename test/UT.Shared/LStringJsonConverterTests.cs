using System.Text.Json;
using Localization.Shared.Models;
using Localization.Shared.JSON;
using Shouldly;
using Xunit;
using Bogus;

namespace UT.Shared;

public class LStringJsonConverterTests
{
    [Fact]
    public void Write_Read_RoundTrip_Works()
    {
        // Arrange
        var faker = new Faker();
        var ns = faker.Lorem.Words(1).First();
        var key = faker.Lorem.Words(1).First();
        var lstring = new LString { Namespace = ns, Key = key };
        var options = new JsonSerializerOptions();
        options.Converters.Add(new LStringJsonConverter());

        // Act
        var json = JsonSerializer.Serialize(lstring, options);
        var deserialized = JsonSerializer.Deserialize<LString>(json, options);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized!.Namespace.ShouldBe(ns);
        deserialized.Key.ShouldBe(key);
    }
}
