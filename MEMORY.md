# MEMORY.md

本文件记录跨会话的经验教训与工作流注意事项，供后续 AI 会话直接参考。
**不要在每次会话开始时主动读取此文件**，仅在遇到相关问题时按需查阅，或在完成任务后更新。

---

## Unity 批处理测试

- 运行 `-runTests` 时**不能加 `-quit`**，否则 Unity 在测试执行前退出，`TestResults.xml` 不会生成
- `asmdef` 的 `defineConstraints` **不能包含 `UNITY_INCLUDE_TESTS`**，批处理模式下该符号未定义，测试程序集不会被编译
- 同类型多存档槽注册时 `index` 参数必须各不相同，否则 `Register` 返回同一个 key

### 单元测试命令

```bash
"D:/UnityEditors/Unity 2023.2.22f1/Editor/Unity.exe" -batchmode -nographics -projectPath "D:/UnityProjects/Capsule" -runTests -testPlatform EditMode -testResults "D:/UnityProjects/Capsule/Logs/TestResults.xml" -logFile "D:/UnityProjects/Capsule/Logs/test_check.log"
```

退出码 0 且 `TestResults.xml` 中 `failed="0"` 方可提交。

---

## Git 提交

- 新建目录后必须检查**目录本身的 `.meta` 文件**是否已加入暂存区（`git status` 确认）
- `git add Assets/path/to/Dir/` 会包含目录内文件的 `.meta`，但**目录本身的 `.meta`**（`Dir.meta`）需单独 `git add`

---

## 异步 API 设计

- `TaskCanceledException` 是 `OperationCanceledException` 的子类，NUnit `Assert.ThrowsAsync<OperationCanceledException>` **不匹配子类**，需用 `Assert.ThrowsAsync<TaskCanceledException>`
- `SemaphoreSlim.WaitAsync(ct)` 在 token 已取消时直接抛出，不进入 try 块，finally 不会 Release——正确行为，无需处理

---

## 当前进度（2026-03-16）

### P1 已完成
- ✅ **异步读写支持**：`SaveAsync` / `TryLoadAsync`，真异步 FileStream（`useAsync: true`），每 key 独立 `SemaphoreSlim` 锁，支持 `CancellationToken`
- ✅ **单元测试**：10 个 EditMode 测试全部通过（同步/异步/并发/取消令牌）
- ✅ **Sample 改写**：`SampleEntry` 已使用异步 API

### 待处理（参见 ROADMAP.md）
- P0：`CLog` Warning/Error Handler 改为 `public`
- P1：移动端路径适配、存档格式版本控制
- P2：`GenerateHelper` 类型支持、`IDService` 槽位上限
- P3：存档完整性校验、存档加密支持
