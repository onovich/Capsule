# SamplePanel 预制体修改指南

## 目标
在 SamplePanel 预制体中新增 Settings 区域，演示版本控制、CRC32 校验、加密功能。

## 操作步骤

### 1. 打开预制体
1. 在 Unity 编辑器中，找到 `Assets/com.mortise.capsule/Resources_Sample/SamplePanel.prefab`
2. 双击打开预制体编辑模式

### 2. 新增 Settings 区域

#### 2.1 创建 Settings 父节点
1. 在 Hierarchy 中右键 `SamplePanel` → UI → Panel，命名为 `Settings`
2. 设置 RectTransform：
   - Anchor: Top-Stretch
   - Pos Y: -30
   - Height: 60
   - Left: 10, Right: 10
3. 删除 Image 组件（只保留 RectTransform）
4. 添加 Vertical Layout Group 组件：
   - Padding: Left 10, Right 10, Top 5, Bottom 5
   - Spacing: 5
   - Child Alignment: Upper Left
   - Child Force Expand: Width ✓, Height ✗

#### 2.2 创建 Version Text
1. 右键 `Settings` → UI → Text，命名为 `Text_Version`
2. Text 组件设置：
   - Text: "Version: 1"
   - Font Size: 14
   - Alignment: Left-Center
   - Color: White
3. RectTransform:
   - Height: 20

#### 2.3 创建 CRC Toggle
1. 右键 `Settings` → UI → Toggle，命名为 `Toggle_CRC`
2. 展开 Toggle，找到子节点 `Label`，修改 Text 组件：
   - Text: "Enable CRC32"
3. Toggle 组件：
   - Is On: ✗（默认关闭）
4. RectTransform:
   - Height: 20

#### 2.4 创建 Encrypt Toggle
1. 右键 `Settings` → UI → Toggle，命名为 `Toggle_Encrypt`
2. 展开 Toggle，找到子节点 `Label`，修改 Text 组件：
   - Text: "Enable Encrypt (XOR)"
3. Toggle 组件：
   - Is On: ✗（默认关闭）
4. RectTransform:
   - Height: 20

### 3. 调整 Label 区域位置
1. 选中 `Label` GameObject
2. 修改 RectTransform：
   - Pos Y: -100（向下移动，为 Settings 腾出空间）

### 4. 绑定引用到 SamplePanel 组件
1. 选中根节点 `SamplePanel`
2. 在 Inspector 中找到 `SamplePanel (Script)` 组件
3. 展开 `Settings` 区域，拖拽绑定：
   - `text_version` ← 拖入 `Text_Version`
   - `toggle_enableCrc` ← 拖入 `Toggle_CRC`
   - `toggle_enableEncrypt` ← 拖入 `Toggle_Encrypt`

### 5. 保存预制体
1. Ctrl+S 保存预制体
2. 关闭预制体编辑模式

## 验证
1. 打开场景 `Assets/com.mortise.capsule/Resources_Sample/SampleEntry.unity`
2. 运行场景，检查：
   - Settings 区域显示在顶部
   - Version 文本显示 "Version: 1"
   - 两个 Toggle 默认关闭
   - 勾选 CRC Toggle 后保存，文件会附加 4 字节校验和
   - 勾选 Encrypt Toggle 后保存，文件会被 XOR 加密

## 功能说明
- **Version**: 显示当前存档版本号（固定为 1）
- **Enable CRC32**: 启用后，存档文件末尾附加 4 字节 CRC32 校验和，读取时验证数据完整性
- **Enable Encrypt (XOR)**: 启用后，存档数据使用简单 XOR 加密（key=0xAB），演示加密钩子功能

## 注意事项
- 修改 Toggle 状态后需要重新保存/读取才会生效（代码中 `RebuildSaveCore()` 会根据当前状态重建 SaveCore）
- CRC 和加密可以同时启用，顺序为：写入时先加密再算 CRC，读取时先验 CRC 再解密
