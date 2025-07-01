using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Xml.Linq;

namespace Localization.Generator.Translation;

public static class ParserHelper
{
    private const string SCHEMA = """
                                  <?xml version="1.0" encoding="UTF-8" ?>
                                  <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
                                    <xs:element name="translations">
                                      <xs:complexType>
                                        <xs:sequence>
                                          <xs:element name="set" maxOccurs="unbounded">
                                            <xs:complexType>
                                              <xs:sequence>
                                                <xs:element name="item" minOccurs="1" maxOccurs="unbounded">
                                                  <xs:complexType>
                                                    <xs:simpleContent>
                                                      <xs:extension base="xs:string">
                                                        <xs:attribute name="lang" use="required">
                                                          <xs:simpleType>
                                                            <xs:restriction base="xs:string"/>
                                                          </xs:simpleType>
                                                        </xs:attribute>
                                                      </xs:extension>
                                                    </xs:simpleContent>
                                                  </xs:complexType>
                                                </xs:element>
                                              </xs:sequence>
                                              <xs:attribute name="key" type="xs:string" use="required" />
                                              <xs:attribute name="description" type="xs:string" use="required" />
                                            </xs:complexType>
                                          </xs:element>
                                        </xs:sequence>
                                        <xs:attribute name="namespace" type="xs:string" use="required" />
                                      </xs:complexType>
                                    </xs:element>
                                  </xs:schema>
                                  """;

    public static bool TryParse(string data, [NotNullWhen(true)] out TranslationsCollection? result)
    {
        result = null;

        if (!TryLoadValidDocument(data, out var document))
            return false;

        var root = document.Root;
        if (root == null || root.Name != "translations")
            return false;

        var namespaceAttr = root.Attribute("namespace");
        if (namespaceAttr == null || string.IsNullOrEmpty(namespaceAttr.Value))
            return false;

        var sets = root
            .Elements("set")
            .Select(static element => (key: element.Attribute("key"), description: (string?)element.Attribute("description"), element))
            .Where(static tuple => tuple is { key: not null, description: not null })
            .Select(static tuple => new Translations(
                tuple.key!.Value,
                tuple.description!,
                tuple.element
                    .Elements("item")
                    .Select(static element => (language: (string?)element.Attribute("lang"), value: (string?)element.Value))
                    .Where(static tuple => tuple is { language: not null, value: not null })
                    .Select(static tuple => new Translation(tuple.language!, tuple.value!))
                    .ToDictionary(static item => item.Language, static item => item, StringComparer.OrdinalIgnoreCase)));

        result = new TranslationsCollection(namespaceAttr.Value, sets);
        return true;
    }

    private static bool TryLoadValidDocument(string data, out XDocument document)
    {
        using var schemaStream = new StringReader(SCHEMA);
        var schema = XmlSchema.Read(schemaStream, (_, _) => { });
        var schemaSet = new XmlSchemaSet();
        schemaSet.Add(schema);

        document = XDocument.Parse(data);
        var isValid = true;

        document.Validate(schemaSet, (_, _) => { isValid = false; });

        return isValid;
    }
}
