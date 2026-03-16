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
- **任务完成后必须清理 unstaged 变更**：要么 commit、要么 discard、要么 ignore，保持工作区干净（除非有正在开发的未完成任务）
- Unity 场景文件（.unity）的自动元数据更新（serializedVersion、RenderSettings 等）应该提交，保持与当前 Unity 版本一致

---

## Git Commit 规范

- **功能修改使用 `modify`，不使用 `update`**：`<feature> modify: 描述`
- **版本号提交格式**：`<version> x.y.z`（不写 "版本号升至"，直接写版本号）
- **版本号提交必须同步打 tag**：`git tag vx.y.z` 与 commit 同时进行
- `<engine> update` 仅用于 Unity 引擎版本升级

---

## 异步 API 设计

- `TaskCanceledException` 是 `OperationCanceledException` 的子类，NUnit `Assert.ThrowsAsync<OperationCanceledException>` **不匹配子类**，需用 `Assert.ThrowsAsync<TaskCanceledException>`
- `SemaphoreSlim.WaitAsync(ct)` 在 token 已取消时直接抛出，不进入 try 块，finally 不会 Release——正确行为，无需处理

---

## 版本发布流程

1. 修改 `Assets/com.mortise.capsule/package.json` 的 `version` 字段
2. 提交：`<version> x.y.z`（直接写版本号，不写 "版本号升至"）
3. 打 tag：`git tag vx.y.z`
4. 推送：`git push origin main && git push origin vx.y.z`

**注意：** 版本号提交必须使用 `<version>` scope，不使用 `<feature> update`。

---

## 当前进度（2026-03-16）

### 已完成（v0.2.0）
- ✅ **P0 三项**：缓冲区溢出保护、CLog Handler 可见性、FileHelper 死方法移除
- ✅ **P1 异步读写**：`SaveAsync` / `TryLoadAsync`，真异步 FileStream（`useAsync: true`），每 key 独立 `SemaphoreSlim` 锁，支持 `CancellationToken`
- ✅ **P1 版本控制**：文件头 1 字节版本号，TryLoad 校验版本不匹配时返回 false + LogWarning
- ✅ **P2 IDService ushort**：key 类型从 byte 升级到 ushort，上限从 255 提升到 65535
- ✅ **P2 GenerateHelper TODO**：不支持类型生成 TODO 注释并输出 Warning
- ✅ **P3 CRC32 校验**：enableCrc=true 开启，文件末尾附加 4 字节 CRC32，读取时验证
- ✅ **P3 加密钩子**：构造时注入 Func<byte[],byte[]> encryptFunc/decryptFunc，写入时先加密再算 CRC，读取时先验 CRC 再解密
- ✅ **单元测试**：16 个 EditMode 测试全部通过（同步/异步/并发/取消令牌/版本/CRC/加密）

### ROADMAP 全部完成
所有 P0/P1/P2/P3 项目已实现，版本号已升至 0.2.0，tag v0.2.0 已打。
