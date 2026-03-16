# ROADMAP.md

当前已知问题与改进规划，按优先级排列。

---

## P0 — 正确性与稳定性

### ~~缓冲区溢出无保护~~ ✅ 已完成
**问题：** `SaveCore` 构造时指定固定 `bufferLength`，若存档数据超过该大小会静默截断或越界。
**实现：** `SaveCore.Save` 中在 `WriteTo` 后校验 `offset > buff.Length`，超出时抛出 `InvalidOperationException`。

### ~~`FileHelper` API 不一致~~ ✅ 已完成
**问题：** 部分方法接受完整 `path` 参数，部分方法内部硬编码使用 `persistentDataPath`，调用者容易混淆。
**实现：** 移除 `WriteFileToPersistent` / `ReadFileFromPersistent` / `CreateDirIfNotExist` / `GetPersistentDir` 四个死方法，统一为完整路径风格。

### ~~`CLog` Warning/Error 回调为 `internal`~~ ✅ 已完成
**问题：** `LogWarningHandler` 和 `LogErrorHandler` 是 `internal`，库外部无法捕获警告和错误日志。
**实现：** 将三个 Handler 统一改为 `public`。

---

## P1 — 功能完善

### ~~异步读写支持~~ ✅ 已完成（2026-03-16）
**背景：** 设计文档（`设计.txt`）和待办（`任务.txt`）均列为目标。
**实现：** `SaveAsync(ISave, key, ct)` / `TryLoadAsync(key, ct)`，真异步 FileStream（`useAsync: true`），每 key 独立 `SemaphoreSlim` 锁，支持 `CancellationToken`。
**测试：** 10 个 EditMode 单元测试全部通过（同步/异步/并发/取消令牌）。

### ~~移动端路径适配~~ ⏭ 跳过
**背景：** `Application.persistentDataPath` 在 Android/iOS 上已是正确路径，现状已够用。

### 存档格式版本控制
**问题：** 字段顺序或类型变更后，旧存档文件无法正确加载，且无任何提示。
**方案：** 在文件头写入版本号（`byte` 或 `ushort`），`TryLoad` 时校验版本；提供迁移回调接口供上层处理兼容逻辑。

---

## P2 — 开发体验

### `GenerateHelper` 类型支持不完整
**问题：** 当前代码生成仅处理一维数组和 `string[]`，不支持嵌套结构体、`List<T>`、`Dictionary` 等复合类型。
**方案：** 扩展 `GetWriteMethod` / `GetReadMethod` 的类型映射，或在生成时对不支持的类型输出 `// TODO` 占位注释并警告。

### IDService 槽位上限过低
**问题：** `byte` 类型上限 255，超出后 `PickSaveId` 会溢出归零，导致 key 冲突。
**方案：** 改为 `ushort`（上限 65535），或在 `PickSaveId` 中检测溢出并抛出明确异常。

---

## P3 — 安全与扩展

### 存档完整性校验
**问题：** 二进制文件损坏时 `FromBytes` 会静默读到错误数据。
**方案：** 可选地在文件末尾附加 CRC32 校验和，`TryLoad` 时验证后再反序列化。

### 存档加密支持
**方案：** 在 `FileHelper.SaveBytes` / `LoadBytes` 外层提供可选的对称加密钩子（外部注入 `Func<byte[], byte[]>`），不在库内硬编码算法。
