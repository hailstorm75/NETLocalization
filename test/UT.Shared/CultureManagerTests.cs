using Localization.Shared;
using Shouldly;
using Xunit;

namespace UT.Shared;

public class CultureManagerTests
{
    [Fact]
    public void Initialize_WithNullServiceProvider_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Should.Throw<ArgumentNullException>(() => CultureManager.Initialize((IServiceProvider)null!));
    }
}
