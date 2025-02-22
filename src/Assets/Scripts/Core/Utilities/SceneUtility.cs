#region

using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.SceneManagement;

#endregion

namespace Core.Utilities
{
    public static class SceneUtility
    {
        private const string NamespaceName = "Main.Scene.Generated";
        private const string ClassName = "SceneLoaderFeatures";

        private const string RequiredNamespace =
            "using Feature.Interface;\nusing UnityEngine.SceneManagement;\n\n";

        private const string SceneEnumClassName = "Scene";
        private const string SceneLoaderClassName = "SceneLoader";
        private const string SceneLoaderSavePath = "Assets/Scripts/Main/Scene/Generated/SceneLoader.cs";
        private static readonly string SavePath = "Assets/Scripts/Main/Scene/Generated/Scenes.cs";

#if UNITY_EDITOR
        [MenuItem("Services/Create/SceneLoader")]
#endif
        private static void Create()
        {
            CreateFeatures();
            CreateSceneLoaderClass();
        }

        private static void CreateSceneLoaderClass()
        {
            var sb = new StringBuilder();

            // Insert required namespaces and auto-generated comment
            InsertRequiredNamespace(sb);
            InsertComment(sb);

            // Start namespace definition
            sb.AppendLine($"namespace {NamespaceName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public class {SceneLoaderClassName}");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly ISceneDataModel sceneDataModel;");
            sb.AppendLine("        private readonly string path;");
            sb.AppendLine("        private RootInstance rootInstance;");
            sb.AppendLine($"        private {SceneEnumClassName} scene;");

            sb.AppendLine(
                $"        public {SceneLoaderClassName}({SceneEnumClassName} scene, string path, ISceneDataModel sceneDataModel)");
            sb.AppendLine("        {");
            sb.AppendLine("            this.scene = scene;");
            sb.AppendLine("            this.path = path;");
            sb.AppendLine("            this.sceneDataModel = sceneDataModel;");
            sb.AppendLine("        }");

            sb.AppendLine("        public SceneLoader Bind(RootInstance rootInstance)");
            sb.AppendLine("        {");
            sb.AppendLine("            this.rootInstance = rootInstance;");
            sb.AppendLine("            return this;");
            sb.AppendLine("        }");

            sb.AppendLine("        public void Load()");
            sb.AppendLine("        {");
            sb.AppendLine("            if (rootInstance != null)");
            sb.AppendLine("            {");
            sb.AppendLine("                rootInstance.CurrentDataModel = sceneDataModel;");
            sb.AppendLine("                rootInstance.AddHistory(scene);");
            sb.AppendLine("            }");

            sb.AppendLine("            SceneManager.LoadScene(path);");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            CreateFile(sb, SceneLoaderSavePath);
        }

        private static void CreateFeatures()
        {
            var sb = new StringBuilder();

            // Insert required namespaces and auto-generated comment
            InsertRequiredNamespace(sb);
            InsertComment(sb);

            const string enumName = "Scene";

            // Start namespace definition
            sb.AppendLine($"namespace {NamespaceName}");
            sb.AppendLine("{");
            sb.AppendLine($"    public enum {enumName}");
            sb.AppendLine("    {");

            var count = SceneManager.sceneCountInBuildSettings;
            var sceneNames = new List<string>();

            for (var i = 0; i < count; i++)
            {
                var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                var sceneName = Path.GetFileNameWithoutExtension(scenePath);
                sceneNames.Add(ConvertToEnumName(sceneName));
            }

            // Create enum entries
            foreach (var sceneName in sceneNames)
            {
                sb.AppendLine($"        {sceneName},");
            }

            // Close enum definition
            sb.AppendLine("    }");
            sb.AppendLine("");

            // Start class definition for SceneLoader
            sb.AppendLine($"    public static class {ClassName}");
            sb.AppendLine("    {");

            // Create factory methods for SceneLoader
            for (var i = 0; i < sceneNames.Count; i++)
            {
                var sceneName = sceneNames[i];
                var scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);

                sb.AppendLine($"        public static SceneLoader {sceneName}(ISceneDataModel sceneDataModel)");
                sb.AppendLine("        {");
                sb.AppendLine(
                    $"            return new SceneLoader({enumName}.{sceneName}, \"{scenePath}\", sceneDataModel);");
                sb.AppendLine("        }");
            }

            sb.AppendLine(
                "        public static SceneLoader GetSceneLoader(Scene scene, ISceneDataModel sceneDataModel)");
            sb.AppendLine("        {");
            sb.AppendLine("            return scene switch");
            sb.AppendLine("            {");
            foreach (var sceneName in sceneNames)
            {
                sb.AppendLine($"                {enumName}.{sceneName} => {sceneName}(sceneDataModel),");
            }

            sb.AppendLine("                var _ => null,");
            sb.AppendLine("            };");
            sb.AppendLine("        }");

            // End class and namespace definition
            sb.AppendLine("    }");
            sb.AppendLine("}");

            CreateFile(sb, SavePath);
        }

        private static string ConvertToEnumName(string sceneName) => sceneName.Replace(" ", "").Replace("-", "_");

        private static void InsertComment(StringBuilder builder)
        {
            builder.Append("// <auto-generated>\n");
            builder.Append($"// This code was generated by {nameof(SceneUtility)}.\n");
            builder.Append("// Do not modify this file manually.\n");
            builder.Append("// </auto-generated>\n\n");
        }

        private static void InsertRequiredNamespace(StringBuilder builder)
        {
            builder.Append(RequiredNamespace);
        }

        private static void CreateFile(StringBuilder builder, string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(directoryPath))
            {
                if (directoryPath != null)
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }

            using (var stream = File.Create(path))
            {
                var generatedCode = builder.ToString();
                var bytes = Encoding.UTF8.GetBytes(generatedCode);
                stream.Write(bytes, 0, bytes.Length);
            }
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        public static void Finished()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}