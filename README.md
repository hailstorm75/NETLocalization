# .NET Localization

The library you've been looking for to localize your next UI project.
Simple to use, runtime language updates, no magic - just plain source generators.

## Defining translations

Reference the source generator and the `Localization.Shared` library in your project.
Your project can either be a UI application or a completely separate library that you share between your UI apps.

Within that project, create an XML file, and configure it to be an [`AdditionalFile`](https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Using%20Additional%20Files.md). This XML will store your translations.
Here's an example of the contents of such a file:

```xml
<?xml version="1.0" encoding="utf-8"?>
<translations namespace="Test">
  <set key="Greeting" description="Greeting message">
    <item lang="en">Hello</item>
    <item lang="de-de">Hallo</item>
  </set>
  <set key="Farewell" description="Farewell message">
    <item lang="en">Bye</item>
    <item lang="de-de">Auf Wiedersehen</item>
  </set>
</translations>
```

- The root element must be `translations`
  - The root element must have a unique `namespace` value across all of your XML files
- Place `set` elements to represent each localized entry
  - Each `set` contains a `key` identifier and a `description`
  - The `description` is useful for translators to understand the full context of the string
- Within a `set` element are `item` elements containing `lang` and the localized string
  - The `lang` is an [`RFC 4646`](https://datatracker.ietf.org/doc/html/rfc4646) standard language key

> The `namespace` and `key` are used to identify a localized string

The XML file must comply with this XSD schema:

```xml
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
```

Within your project also create a static class that shall provide type-safe access to localized strings:

```csharp
[TranslationProvider("XMLFileName")]
public static partial class ApplicationTranslations;
```

Decorate that class with the `TranslationProvider` attribute that takes in a string - the file name (without extension) of the XML file containing your localized strings.

Once the source generators kick in, your class will now have everything necessary generated.

## Initialization within UI app

If your application does not use [DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection), then make sure to call the following code at startup:

```csharp
var translator = new Translator(NullLogger<Translator>.Instance);

CultureManager.Initialize(translator);
```

If your application does use DI, e.g., from [CommunityToolkit.MVVM](https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/ioc), then make sure to call the following code at startup:

```csharp
var serviceCollection = new ServiceCollection();
serviceCollection
    .AddSingleton<ITranslator, Translator>()
    .AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

// Replace with your own DI provider
Ioc.Default.ConfigureServices(serviceCollection.BuildServiceProvider());

CultureManager.Initialize(Ioc.Default);
```

> In both cases, you might want to consider replacing `NullLogger<>` with proper logging

After this, you'll have to load up the `ITranslator` instance with translations from your providers.
Assuming you have the `ApplicationTranslations` provider, you can load the translations like so:

```csharp
translator.RegisterTranslations(ApplicationTranslations.GetTranslations());
```

## Changing language

You can change the application language during runtime via the singleton class `CultureManager` by calling `CultureManager.SetLanguage(...)`.
The `SetLanguage` method takes in an instance of `Language` or a `string` (via implicit conversion). The `string` must be a valid [`RFC 4646`](https://datatracker.ietf.org/doc/html/rfc4646) language key.

You can access the available `Language` keys from the `ITranslator`'s `LoadedLanguages` property.
