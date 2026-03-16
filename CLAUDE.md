# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 文档索引

| 文档 | 内容 |
|------|------|
| [ARCHITECTURE_GUIDE.md](ARCHITECTURE_GUIDE.md) | 核心设计原则、分层架构、目录结构规范 |
| [CODING_STYLE.md](CODING_STYLE.md) | 命名规范、代码结构规范、Git Commit 规范 |
| [REVIEW_CHECKLIST.md](REVIEW_CHECKLIST.md) | 提交前逐项审查清单 |
| [ROADMAP.md](ROADMAP.md) | 已知问题分析与改进规划 |

---

## 项目简介

**MortiseFrame.Capsule**（`com.mortise.capsule`）是一个用于 Unity 的轻量级二进制存档库（v0.0.9，要求 Unity 2019.4+），通过字节缓冲区模式将游戏数据序列化为本地二进制文件。

依赖 **MortiseFrame.LitIO**（`com.mortise.litio`），通过 Git URL 安装，提供所有 `ISave` 实现中使用的 `ByteWriter` 和 `ByteReader`。

## 开发方式

本项目是 Unity UPM 包，无独立 CLI 构建/测试命令，所有开发均在 Unity Editor 中进行：

- 使用 Unity 2023.2.22f1 打开项目
- 示例场景路径：`Assets/com.mortise.capsule/Resources_Sample/SampleEntry.unity`
- 已引入 Unity Test Framework（`com.unity.test-framework`）依赖

## 架构说明

### 核心流程

1. **`ISave`** — 所有可存档数据结构实现的接口，需实现 `WriteTo(byte[], ref int)` 和 `FromBytes(byte[], ref int)`。

2. **`SaveCore`** — 对外公开的 API 入口：
   - 实例化 `SaveCore(bufferLength, path)`
   - 调用 `Register(type, fileName, index)` 注册存档槽，返回 `byte` 类型的 key
   - 调用 `Save(ISave, key)` 写入，`TryLoad(key, out ISave)` 读取
   - 调用 `DeleteFile(key)` 或 `DeleteAllFiles()` 删除存档
   - 调用 `Clear()` 重置所有状态

3. **`SaveContext`** — 内部状态容器，持有读写字节缓冲区、`BiDictionary<byte, (Type, int)>` 协议注册表、文件名字典和根路径，不对外暴露。

4. **`IDService`** — 自增 `byte` 计数器，为每次 `Register` 分配唯一 key（每个 `SaveCore` 实例最多 255 个存档槽）。

### 同类型多文件支持

`Register` 的 `index` 参数允许同一 `ISave` 类型注册多个存档槽（如多存档位）。`BiDictionary` 的 value 为 `(Type, int)` 元组以区分不同槽位。

### 日志系统

`CLog` 提供可注入的 `Action<string>` 回调（`LogHandler`、`LogWarningHandler`、`LogErrorHandler`），在库外部接入即可路由日志。

### 关键文件

| 文件 | 职责 |
|------|------|
| `Scripts_Runtime/ISave.cs` | 对外序列化接口 |
| `Scripts_Runtime/SaveCore.cs` | 对外 API |
| `Scripts_Runtime/SaveContext.cs` | 内部状态与缓冲区管理 |
| `Scripts_Runtime/CLog.cs` | 可注入日志 |
| `Scripts_Runtime/Common/Helper/FileHelper.cs` | 文件 I/O 封装 |
| `Scripts_Sample/SampleRoleDBModel.cs` | `ISave` 参考实现 |
| `Scripts_Sample/SampleEntry.cs` | `SaveCore` 使用参考 |

### 程序集定义

- `MortiseFrame.Capsule` — 运行时程序集，引用 LitIO
- `MortiseFrame.Capsule.Sample` — 示例程序集，同时引用两者

## 提交信息规范

格式：`<scope> <type>: <描述>`，描述使用中文。

```
<feature> add: 描述
<feature> fix: 描述
<engine> update: 描述
<sample> fix: 描述
```
