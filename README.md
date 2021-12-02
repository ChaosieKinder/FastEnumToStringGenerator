just a little playing around testing out Code Generators

# FastEnumToStringGenerator
Adds an attribute for code generation of system and external enums, automatically grabs all enums in the current project. Extension methods are generated in same namespace as the enum they are for, so no additional using statements are required for enums in the same project. 

Usage:
```csharp
enum MyEnum {
  value1,
  value2
}

// ... elsewhere...
var enumValue = MyEnum.value1;
string name = enumValue.FastToString();
```

Usage for external enums:
```csharp
[ExtraFastEnum(typeof(System.Base64FormattingOptions))]
class Placeholder
{
  //...
}
```
You can choose to organize your attributes wherever you want. Duplicates are removed automatically.
