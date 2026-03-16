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

### ~~存档格式版本控制~~ ✅ 已完成（2026-03-16）
**实现：** 文件头写入 1 字节版本号（buff[0]），`TryLoad`/`TryLoadAsync` 校验版本，不匹配时返回 false 并记录 Warning。

---

## P2 — 开发体验

### ~~`GenerateHelper` 类型支持不完整~~ ✅ 已完成（2026-03-16）
**实现：** 对不支持的类型生成 `// TODO: unsupported type - {name} ({typeName})` 占位注释并输出 `Debug.LogWarning`。

### ~~IDService 槽位上限过低~~ ✅ 已完成（2026-03-16）
**实现：** `byte` → `ushort`，上限从 255 提升到 65535。

---

## P3 — 安全与扩展

### ~~存档完整性校验~~ ✅ 已完成（2026-03-16）
**实现：** `SaveCore` 构造时 `enableCrc=true` 开启，文件末尾附加 4 字节 CRC32 校验和，读取时验证，失败时返回 false 并记录 Error。

### ~~存档加密支持~~ ✅ 已完成（2026-03-16）
**实现：** `SaveCore` 构造时注入 `Func<byte[], byte[]> encryptFunc/decryptFunc`，写入时先加密再算 CRC，读取时先验 CRC 再解密。
