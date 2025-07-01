using System.Runtime.CompilerServices;

namespace Localization.Shared.Attributes;

/// <summary>
/// Attribute to localize an enum
/// </summary>
[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
public sealed class LocalizedEnumFieldAttribute : Attribute
{
  /// <summary>
  /// Namespace of the translation
  /// </summary>
  public string Namespace { get; }

  /// <summary>
  /// Key of the translation
  /// </summary>
  public string Key { get; set; }

  /// <summary>
  /// Default constructor
  /// </summary>
  public LocalizedEnumFieldAttribute(string @namespace, [CallerMemberName] string? key = "")
  {
    Key = key ?? string.Empty;
    Namespace = @namespace;
  }
}