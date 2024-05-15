using System.IO;
using System.Linq;
using UnityEditor;

namespace Monaverse.Editor
{
    public class MonaverseEditorTools
    {
        [MenuItem("Tools/Monaverse/Export Package")]
        public static void ExportUnityPackage()
        {
            // Base folders to include
            var baseFolders = new[]
            {
                "Assets/Monaverse",
                "Assets/TextMesh Pro",
                "Assets/WalletConnectUnity",
            };

            // Sub-folders to exclude
            var excludeFolders = new []
            {
                "Assets/Monaverse/Core/Plugins/Mona/com.monaverse.api/Tests"
            };

            // Get all assets, excluding the specified sub-folders
            var includePaths = baseFolders.SelectMany(folder =>
                    Directory.GetFiles(folder, "*", SearchOption.AllDirectories)
                        .Where(path => !excludeFolders.Any(path.StartsWith))
                        .Select(path => path.Replace("\\", "/")) // Normalize path for Unity
            ).ToArray();

            const string packagePath = "Monaverse.unitypackage";
            AssetDatabase.ExportPackage(includePaths, packagePath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

            UnityEngine.Debug.Log("Package exported successfully to: " + packagePath);
        }
    }
}