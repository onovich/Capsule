#!/usr/bin/env bash
# build_check.sh
# 调用 Unity 无头模式对项目进行编译检查，确认无报错后退出。
# 用法：bash build_check.sh
# 退出码：0 = 编译成功，1 = 编译失败

UNITY="D:/UnityEditors/Unity 2023.2.22f1/Editor/Unity.exe"
PROJECT="$(cd "$(dirname "$0")" && pwd)"
LOG="$PROJECT/Logs/build_check.log"

mkdir -p "$PROJECT/Logs"

echo "[build_check] 开始编译检查..."
echo "[build_check] 项目路径: $PROJECT"
echo "[build_check] 日志路径: $LOG"

"$UNITY" \
  -batchmode \
  -nographics \
  -quit \
  -projectPath "$PROJECT" \
  -logFile "$LOG" \
  -buildTarget StandaloneWindows64 2>&1

EXIT_CODE=$?

echo ""
echo "===== 编译日志（最后 40 行）====="
tail -40 "$LOG"
echo "================================="

if [ $EXIT_CODE -ne 0 ]; then
  echo ""
  echo "[build_check] ❌ 编译失败（exit $EXIT_CODE），请查看上方日志。"
  exit 1
fi

# Unity 退出码为 0 时仍需检查日志中是否有 compile error
if grep -qiE "error CS|compilation failed|scripts have compiler errors" "$LOG"; then
  echo ""
  echo "[build_check] ❌ 检测到编译错误，请查看上方日志。"
  exit 1
fi

echo ""
echo "[build_check] ✅ 编译检查通过，可以提交。"
exit 0
