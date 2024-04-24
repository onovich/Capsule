#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public static class GenerateHelper {

        static Type TYPE_INTERFACE = typeof(ISave);
        static string METHOD_WRITE = "WriteTo";
        static string METHOD_READ = "FromBytes";
        static string STRUCT_HEAD = "public struct";
        static string CLASS_HEAD = "public class";
        static string METHOD_HEAD = "public void";
        static string CLASS_WRITER = "ByteWriter";
        static string CLASS_READER = "ByteReader";

        static void TryWriteMethod(bool inClass,
                               bool methodFound,
                               string line,
                               Type type,
                               StringBuilder newFileContent,
                               Func<Type, string> GenerateMethod) {
            if (inClass && !methodFound) {
                newFileContent.AppendLine(GenerateMethod(type));
                newFileContent.AppendLine();
                Debug.Log($"Write method: {type.Name}");
            }
        }

        [MenuItem("Tools/GenerateSaveMethods")]
        public static void GenerateSaveMethods() {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Contains(TYPE_INTERFACE))
                .ToList();

            Debug.Log($"Found {types.Count} types that implement {TYPE_INTERFACE.Name}");

            foreach (var type in types) {
                string filter = $"t:script {type.Name}";
                string[] guids = AssetDatabase.FindAssets(filter);
                if (guids.Length == 0) {
                    Debug.LogWarning($"No script found for type {type.Name}");
                    continue;
                }

                string filePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                Debug.Log($"Updating script: {type.Name} at path {filePath}");

                string[] lines = File.ReadAllLines(filePath);
                StringBuilder newFileContent = new StringBuilder();
                bool insideClass = false;
                bool writeMethodFound = false;
                bool readMethodFound = false;

                for (int i = 0; i < lines.Length; i++) {
                    string line = lines[i];

                    if (line.Contains($"{STRUCT_HEAD} {type.Name} : {TYPE_INTERFACE.Name}") || line.Contains($"{CLASS_HEAD} {type.Name} : {TYPE_INTERFACE.Name}")) {
                        insideClass = true;
                    }

                    if (insideClass && line.Trim().StartsWith($"{METHOD_HEAD} {METHOD_WRITE}(")) {
                        newFileContent.AppendLine(GenerateWriteToMethod(type));
                        writeMethodFound = true;
                        i = SkipMethodBody(lines, i);
                        continue;
                    }

                    if (insideClass && line.Trim().StartsWith($"{METHOD_HEAD} {METHOD_READ}(")) {
                        newFileContent.AppendLine(GenerateFromBytesMethod(type));
                        readMethodFound = true;
                        i = SkipMethodBody(lines, i);
                        continue;
                    }

                    if (line.Contains("}")) {
                        TryWriteMethod(insideClass, writeMethodFound, line, type, newFileContent, GenerateWriteToMethod);
                        TryWriteMethod(insideClass, readMethodFound, line, type, newFileContent, GenerateFromBytesMethod);
                        insideClass = false;
                    }

                    newFileContent.AppendLine(line);
                }

                File.WriteAllText(filePath, newFileContent.ToString());
                Debug.Log($"Updated script: {type.Name}");
            }

            // AssetDatabase.Refresh();
        }

        static int SkipMethodBody(string[] lines, int start) {
            int depth = 0;
            for (int i = start; i < lines.Length; i++) {
                if (lines[i].Contains("{")) {
                    depth++;
                }
                if (lines[i].Contains("}")) {
                    depth--;
                    if (depth == 0) {
                        return i;
                    }
                }
            }
            return start;
        }

        static string GenerateWriteToMethod(Type type) {
            StringBuilder methodBuilder = new StringBuilder();
            methodBuilder.AppendLine($"\t\t{METHOD_HEAD} {METHOD_WRITE}(byte[] dst, ref int offset) {{");
            methodBuilder.Append(GenerateMethodBody(type, "Write"));
            methodBuilder.AppendLine("\t\t}");
            return methodBuilder.ToString();
        }

        static string GenerateFromBytesMethod(Type type) {
            StringBuilder methodBuilder = new StringBuilder();
            methodBuilder.AppendLine($"\t\t{METHOD_HEAD} {METHOD_READ}(byte[] src, ref int offset) {{");
            methodBuilder.Append(GenerateMethodBody(type, "Read"));
            methodBuilder.AppendLine("\t\t}");
            return methodBuilder.ToString();
        }

        static string GenerateMethodBody(Type type, string operation) {
            StringBuilder methodBuilder = new StringBuilder();
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                string methodName = operation == "Write" ? GetWriteMethod(field.FieldType) : GetReadMethod(field.FieldType);
                if (!string.IsNullOrEmpty(methodName)) {
                    bool isEnum = field.FieldType.IsEnum;
                    string enumUnderlyingType = isEnum ? $"({GetEnumUnderlyingType(field.FieldType).Name})" : "";
                    string enumType = isEnum ? $"({field.FieldType.Name})" : "";
                    string line = operation == "Write"
                        ? $"\t\t\t{CLASS_WRITER}.{methodName}(dst, {enumUnderlyingType}{field.Name}, ref offset);"
                        : $"\t\t\t{field.Name} = {enumType}{CLASS_READER}.{methodName}(src, ref offset);";
                    methodBuilder.AppendLine(line);
                }
            }
            return methodBuilder.ToString();
        }

        static Type GetEnumUnderlyingType(Type fieldType) {
            return Enum.GetUnderlyingType(fieldType);
        }

        static string GetWriteMethod(Type fieldType) {

            // Array
            if (fieldType.IsArray && fieldType != typeof(string[])) {
                return $"WriteArray<{fieldType.GetElementType().Name}>";
            }

            // String Array
            if (fieldType == typeof(string[])) {
                return "WriteUTF8StringArray";
            }

            // Enum
            if (fieldType.IsEnum) {
                return $"Write<{GetEnumUnderlyingType(fieldType).Name}>";
            }

            // Other
            switch (fieldType.Name) {
                case "Int32":
                case "Single":  // float
                case "Double":
                case "Byte":
                case "Boolean":
                case "Vector3":
                case "Quaternion":
                    return $"Write<{fieldType.Name}>";
                case "String":
                    return "WriteUTF8String";
                default:
                    return null;
            }
        }

        static string GetReadMethod(Type fieldType) {

            // Array
            if (fieldType.IsArray && fieldType != typeof(string[])) {
                return $"ReadArray<{fieldType.GetElementType().Name}>";
            }

            // String Array
            if (fieldType == typeof(string[])) {
                return "ReadUTF8StringArray";
            }

            // Enum
            if (fieldType.IsEnum) {
                return $"Read<{Enum.GetUnderlyingType(fieldType).Name}>";
            }

            // Other
            switch (fieldType.Name) {
                case "Int32":
                case "Single":  // float
                case "Double":
                case "Byte":
                case "Boolean":
                case "Vector3":
                case "Quaternion":
                    return $"Read<{fieldType.Name}>";
                case "String":
                    return "ReadUTF8String";
                default:
                    return null;
            }
        }
    }
}
#endif