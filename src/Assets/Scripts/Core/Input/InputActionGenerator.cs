#region

using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

#endregion

namespace Core.Input
{
    internal static class InputActionGenerator
    {
        private const string NamespaceName = "Core.Input.Generated";
        private const string ClassName = "ActionGuid";
        private const string RequiredNamespace = "using System;\n";
        private static readonly string SavePath = $"Assets/Scripts/Core/Input/Generated/{ClassName}.cs";
        private static readonly string ProfilePath = Application.dataPath + "/Resources";
        private static InputActionAssetProfile profileAsset;
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        private static void LoadProfile()
        {
            if (!Directory.Exists(ProfilePath))
            {
                Debug.LogError("Resources directory was not found!");
                return;
            }

            profileAsset = Resources.Load<InputActionAssetProfile>("InputActionAssetProfile");
            if (profileAsset == null)
            {
                Debug.LogWarning(
                    "InputActionAssetProfile was not found! \nPlease create a profile in the Resources folder.");
            }
        }

#if UNITY_EDITOR
        [MenuItem("InputSystem/LoadManual")]
#endif
        private static void LoadManual()
        {
            LoadProfile();
        }
#if UNITY_EDITOR
        [MenuItem("InputSystem/GenerateActionName")]
#endif
        private static void Generate()
        {
            if (profileAsset == null)
            {
                Debug.LogError("InputActionAssetProfile is not loaded. Please load the profile first.");
                return;
            }

            var usingAsset = profileAsset.GetUsingAsset();
            var builder = new StringBuilder();

            // Generate class for action maps
            builder.Append(CreateActionMapCode(usingAsset));
            builder.Append("\n\n");

            // Generate classes for each action map's GUIDs
            foreach (var actionMap in usingAsset.actionMaps)
            {
                builder.Append(CreateInputActionCode(actionMap));
                builder.Append("\n\n");
            }

            // Remove the last new line characters
            if (builder.Length > 0)
            {
                builder.Remove(builder.Length - 2, 2);
            }

            // Wrap the generated code in a namespace
            SurroundAsNamespace(builder, NamespaceName);

            // Insert required namespaces and auto-generated comment
            InsertRequiredNamespace(builder);
            InsertComment(builder);

            // Create and write to file
            CreateFile(builder);

            Debug.Log($"{SavePath} has been successfully generated with InputAction GUIDs.");
        }

        private static void CreateFile(StringBuilder builder)
        {
            var directoryPath = Path.GetDirectoryName(SavePath);
            if (!Directory.Exists(directoryPath))
            {
                if (directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            using (var stream = File.Create(SavePath))
            {
                var generatedCode = builder.ToString();
                var bytes = Encoding.UTF8.GetBytes(generatedCode);
                stream.Write(bytes, 0, bytes.Length);
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        private static void InsertComment(StringBuilder builder)
        {
            builder.Insert(0, "// <auto-generated>\n" +
                              $"// This code was generated by {nameof(InputActionGenerator)}.\n" +
                              "// </auto-generated>\n\n");
        }

        private static void InsertRequiredNamespace(StringBuilder builder)
        {
            builder.Insert(0, $"{RequiredNamespace}\n");
        }

        private static StringBuilder CreateActionMapCode(InputActionAsset actionAsset)
        {
            var builder = new StringBuilder();

            if (actionAsset.actionMaps.Count == 0)
            {
                return builder;
            }

            foreach (var actionMap in actionAsset.actionMaps)
            {
                builder.AppendLine(
                    $"       public static readonly Guid {actionMap.name} = new Guid(\"{actionMap.id}\");");
            }

            builder.Remove(builder.Length - 1, 1); // Remove the last new line
            SurroundAsClass(builder, ClassName, true);
            return builder;
        }

        private static StringBuilder CreateInputActionCode(InputActionMap actionMap)
        {
            var builder = new StringBuilder();

            builder.AppendLine($"       public static readonly Guid MapId = new Guid(\"{actionMap.id}\");");
            foreach (var inputAction in actionMap)
            {
                builder.AppendLine(
                    $"       public static readonly Guid {inputAction.name} = new Guid(\"{inputAction.id}\");");
            }

            builder.Remove(builder.Length - 1, 1); // Remove the last new line
            SurroundAsClass(builder, actionMap.name, true);
            return builder;
        }

        private static void SurroundAsClass(StringBuilder builder, string className, bool isStatic)
        {
            builder.Insert(0, $"    public{(isStatic ? " static" : string.Empty)} class {className}\n    {{\n");
            builder.Append("\n    }\n");
        }

        private static void SurroundAsNamespace(StringBuilder builder, string namespaceName)
        {
            builder.Insert(0, $"namespace {namespaceName}\n{{\n");
            builder.Append("}\n");
        }
    }
}