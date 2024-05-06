# Capsule
Capsule, a lightweight local save library suitable for small-scale single-player games. Saves are stored as binary files, allowing for quick read and write operations.<br/>
**Capsule，一个轻量级本地存档库，适用于小体量单机游戏。存档被存储为二进制文件，可以快速读写。**

![](https://github.com/onovich/Capsule/blob/main/Assets/com.mortise.capsule/Resources_Sample/cover.jpg)

# Readiness
Supports arbitrary value type data structures, value type arrays, enums that can be expressed as int, and UTF-8 strings. Does not support multi-level nesting (not tested).<br/>
**支持任意值类型数据结构、值类型数组、可表达为 int 的枚举、UTF-8 字符串。不支持多层嵌套（未测试）。**

# Code Generation
After inheriting the ISave interface, define fields, implement the interface, and use Tool > GenerateSaveMethods to automatically generate the WriteTo / FromBytes methods required for serialization/deserialization.<br/>
**继承 ISave 接口后，定义字段，实现接口，使用 Tool > GenerateSaveMethods 可自动生成序列化 / 反序列化所需的 WriteTo / FromBytes 方法。**

# Sample
```
// Data
public struct SampleRoleDBModel : ISave {

  public string name;
  public RoleType roleType;
  public int[] skillTypeIDArr;

  // Generated Code
  public void WriteTo(byte[] dst, ref int offset) {
    ByteWriter.WriteUTF8String(dst, name, ref offset);
    ByteWriter.Write<Int32>(dst, (Int32)roleType, ref offset);
    ByteWriter.WriteArray<Int32>(dst, skillTypeIDArr, ref offset);
  }

  public void FromBytes(byte[] src, ref int offset) {
    name = ByteReader.ReadUTF8String(src, ref offset);
    roleType = (RoleType)ByteReader.Read<Int32>(src, ref offset);
    skillTypeIDArr = ByteReader.ReadArray<Int32>(src, ref offset);
  }

}
```

```
// Save
saveCore = new SaveCore();
byte key = saveCore.Register(typeof(SampleRoleDBModel), "SampleRoleSave"); // The string here will be used for the file name.
saveCore.Save(roleDBModel);
```

```
// Load
var succ = saveCore.TryLoad(key, out ISave save);
if (succ) {
    roleDBModel = (SampleRoleDBModel)save;
}
```

# Project Sample
[Oshi](https://github.com/onovich/Oshi)

# Dependency
Serialization / Deserialization Tool 
[LitIO](https://github.com/onovich/LitIO)

# UPM URL
ssh://git@github.com/onovich/Capsule.git?path=/Assets/com.mortise.capsule#main

