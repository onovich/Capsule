# REVIEW_CHECKLIST.md

提交代码前逐项确认。

---

## 编译检查（必须在提交前通过）

新功能完成后，执行无头编译脚本确认无报错：

```bash
bash build_check.sh
```

- [ ] 脚本退出码为 0
- [ ] 日志中无 `error CS` / `compilation failed` 关键字
- [ ] 若编译失败，修复后重新运行，通过后再继续后续检查项

> 日志输出至 `Logs/build_check.log`（已被 .gitignore 忽略）。

## 单元测试（有 Scripts_Tests 时执行）

```bash
"D:/UnityEditors/Unity 2023.2.22f1/Editor/Unity.exe" -batchmode -nographics -projectPath "D:/UnityProjects/Capsule" -runTests -testPlatform EditMode -testResults "D:/UnityProjects/Capsule/Logs/TestResults.xml" -logFile "D:/UnityProjects/Capsule/Logs/test_check.log"
```

- [ ] 不加 `-quit`（`-runTests` 自行控制退出）
- [ ] 退出码为 0
- [ ] `TestResults.xml` 中 `failed="0"`

> 注意：`asmdef` 的 `defineConstraints` 不要包含 `UNITY_INCLUDE_TESTS`，否则批处理模式下测试程序集不会被编译。

## Git 提交
- [ ] 新建目录时确认目录的 `.meta` 文件已一并 `git add`（Unity 会为每个新目录生成 `.meta`）

---

## ISave 实现

- [ ] 数据载体为 `struct` + `[Serializable]`，命名以 `DBModel` 结尾
- [ ] `WriteTo` 与 `FromBytes` 中字段顺序完全一致
- [ ] 新增字段后已重新运行 **Tools → GenerateSaveMethods**（或手动同步）
- [ ] 字段全部为 LitIO 支持的类型（基本类型、枚举、数组、Vector3、Quaternion 等）

## SaveCore 使用

- [ ] `Register` 在使用前调用，key 妥善保存
- [ ] 同类型多存档槽时 `index` 参数各不相同
- [ ] 注册槽位总数未超过 255（`IDService` 上限）
- [ ] 缓冲区大小（`bufferLength`）足以容纳最大存档数据

## 日志

- [ ] 库内部使用 `CLog`，未直接调用 `Debug.Log`
- [ ] `CLog.LogHandler` 在使用前已从外部注册

## 编辑器代码

- [ ] 编辑器专用代码包裹在 `#if UNITY_EDITOR` 内
- [ ] ScriptableObject 的 `Bake` / `Load` 方法在编辑器中验证通过

## MonoBehaviour

- [ ] 初始化逻辑在 `Ctor()` 中，`Start()` 仅做调用
- [ ] `Action` 回调在 `OnDestroy` 中置 null 并移除 UI 监听

## 命名与风格

- [ ] 命名空间为 `MortiseFrame.Capsule`
- [ ] 内部实现成员使用 `internal`，未误设为 `public`
- [ ] SerializeField 字段命名格式为 `控件类型_功能名`（如 `btn_save`）

## 提交信息

- [ ] 格式为 `<scope> <type>: <中文描述>`
- [ ] scope 为 `<feature>` / `<sample>` / `<engine>` 之一
- [ ] type 为 `add` / `fix` / `update` 之一
