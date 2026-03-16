# CODING_STYLE.md

## 命名规范

### 命名空间
所有运行时代码统一使用 `MortiseFrame.Capsule`，示例代码同命名空间（不单独区分）。

### 类 / 接口 / 枚举
PascalCase，接口以 `I` 开头：
```csharp
public interface ISave { }
public class SaveCore { }
public enum RoleType { }
```

### 方法
PascalCase：
```csharp
public bool TryLoad(byte key, out ISave save) { }
public byte Register(Type saveType, string fileName, int index = 0) { }
```

### 字段

| 场景 | 规范 | 示例 |
|------|------|------|
| 私有/内部字段 | camelCase，无前缀 | `readBuffer`, `protocolDicts` |
| MonoBehaviour SerializeField | `控件类型_功能名` | `btn_save`, `input_name`, `dropdown_roleType` |
| public 字段（DBModel/Entity） | camelCase | `typeID`, `typeName`, `skillTypeIDArr` |
| 静态常量 | 全大写下划线 | `PLATFORM`, `METHOD_WRITE` |

### 本地变量 / 参数
camelCase：`bufferLength`, `saveName`, `offset`

---

## 代码结构规范

### MonoBehaviour 初始化
使用 `Ctor()` 方法代替构造函数，在 `Start()` 或外部显式调用：
```csharp
// 正确
panel.Ctor();
panel.BtnSaveClickHandle = () => Save();

// 禁止在 Awake/Start 内部直接绑定外部逻辑
```

### 事件/回调
使用 `public Action` 字段，在 `OnDestroy` 中置 null 并移除监听：
```csharp
public Action BtnSaveClickHandle;

private void OnDestroy() {
    btn_save.onClick.RemoveAllListeners();
    BtnSaveClickHandle = null;
}
```

### 编辑器专用代码
用 `#if UNITY_EDITOR` 包裹，不放入独立 `Editor/` 目录：
```csharp
#if UNITY_EDITOR
using UnityEditor;
// ...
#endif
```

### internal 访问修饰符
内部实现类成员（`SaveContext` 的方法）使用 `internal`，不使用 `public`。
库外部调用者只能看到 `SaveCore` 和 `ISave`。

---

## ISave 实现规范

- 数据载体使用 `struct`（值类型），加 `[Serializable]` 特性
- 实现类命名以 `DBModel` 结尾，区别于运行时 Entity
- `WriteTo` 与 `FromBytes` 中字段读写顺序必须完全一致
- 推荐使用 **Tools → GenerateSaveMethods** 自动生成方法体，避免手动维护顺序

```csharp
[Serializable]
public struct FooDBModel : ISave {
    public int id;
    public string name;

    public void WriteTo(byte[] dst, ref int offset) {
        ByteWriter.Write<Int32>(dst, id, ref offset);
        ByteWriter.WriteUTF8String(dst, name, ref offset);
    }

    public void FromBytes(byte[] src, ref int offset) {
        id = ByteReader.Read<Int32>(src, ref offset);
        name = ByteReader.ReadUTF8String(src, ref offset);
    }
}
```

---

## 日志规范

库内部使用 `CLog`，不直接调用 `Debug.Log`：
```csharp
// 正确
CLog.Log($"Save Succ: length = {offset}");

// 禁止
Debug.Log("...");
```

外部接入时注册回调：
```csharp
CLog.LogHandler = Debug.Log;
```

---

## Git Commit 规范

格式：`<scope> <type>: <中文描述>`

| scope | 含义 |
|-------|------|
| `<feature>` | 核心功能变更 |
| `<sample>` | 示例代码变更 |
| `<doc>` | 文档变更（README、CLAUDE.md、架构指南等） |
| `<engine>` | Unity 引擎版本升级 |

| type | 含义 |
|------|------|
| `add` | 新增功能 |
| `fix` | 缺陷修复 |
| `update` | 已有功能更新或重构 |

示例：
```
<feature> add: 支持同类型多存档槽
<feature> fix: Save 应传入 key
<sample> fix: Sample 报错
<doc> add: 新增架构指南文档
<doc> update: 完善编译检查流程说明
<engine> update: 2023.2.22f1
```
