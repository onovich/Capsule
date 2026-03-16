# ARCHITECTURE_GUIDE.md

## 核心设计原则

### 1. 轻量零运行时开销
- 不使用 JSON / XML 等文本格式，全程操作原始 `byte[]`
- 预分配固定大小缓冲区（默认 4069 字节），避免运行时 GC 分配
- 依赖库仅 MortiseFrame.LitIO，不引入第三方重量级框架

### 2. 接口驱动 + 数据与行为分离
- `ISave` 是唯一的公开序列化协议，实现者全权负责字段的读写顺序
- DBModel（数据）与 Entity（运行时表现）严格分层，不互相持有引用
- EditorModel（ScriptableObject）仅在编辑器阶段使用，负责预览与烘焙

### 3. 内部状态完全封装
- `SaveContext` 持有所有可变状态（缓冲区、注册表、路径），对外不可见
- 公开 API 只有 `SaveCore`，调用者无需关心内部存储细节

### 4. 可注入日志
- `CLog` 不依赖 `Debug.Log`，通过 `Action<string>` 回调接入外部日志系统
- 库内部不直接调用 Unity API 打印日志，保持可测试性

---

## 分层架构

```
调用者（游戏代码）
    │
    ▼
SaveCore          ← 唯一公开入口，封装注册/存/读/删操作
    │
    ▼
SaveContext       ← 内部状态容器（缓冲区 + 注册表 + 路径）
    │         │
    ▼         ▼
FileHelper   IDService    BiDictionary
（I/O封装）  （key自增）   （双向协议表）
```

### 数据流（Save）
```
ISave.WriteTo(buffer, ref offset)
    → FileHelper.SaveBytes(path, buffer, offset)
```

### 数据流（Load）
```
FileHelper.LoadBytes(path, buffer)
    → ISave.FromBytes(buffer, ref offset)
    → 返回填充完毕的 ISave 实例
```

---

## 关键设计决策

### 多槽位支持
`Register(Type, fileName, index)` 中的 `index` 允许同一 `ISave` 类型注册多个存档槽。
注册表的 Value 为 `(Type, int)` 元组，key 为 `byte`（上限 255 个槽位）。

### 编辑器工具代码生成
`GenerateHelper`（`#if UNITY_EDITOR`）通过反射遍历所有 `ISave` 实现类，
自动生成 `WriteTo` / `FromBytes` 方法体，减少手写模板代码的维护负担。
触发方式：Unity 菜单 → **Tools → GenerateSaveMethods**。

### ScriptableObject 预览
`SampleRoleEditorModel` 作为 SO 资产，提供 `Bake` / `Load` 右键菜单，
允许在不进入 Play 模式的情况下测试存档读写。

---

## 目录结构规范

```
Assets/com.mortise.capsule/
├── Scripts_Runtime/          ← 运行时代码（打入 MortiseFrame.Capsule 程序集）
│   ├── Common/
│   │   ├── Connection/       ← 平台路径工具
│   │   ├── Datastructure/    ← 通用数据结构（BiDictionary）
│   │   ├── Helper/           ← 工具类（FileHelper、GenerateHelper）
│   │   └── Service/          ← 服务类（IDService）
│   ├── ISave.cs
│   ├── SaveCore.cs
│   ├── SaveContext.cs
│   └── CLog.cs
├── Scripts_Sample/           ← 示例代码（MortiseFrame.Capsule.Sample 程序集）
└── Resources_Sample/         ← 示例场景与 SO 资产
```

文件夹命名使用 `Scripts_` / `Resources_` 前缀加后缀区分用途，不使用 `Editor/` 约定目录（编辑器代码用 `#if UNITY_EDITOR` 保护）。
