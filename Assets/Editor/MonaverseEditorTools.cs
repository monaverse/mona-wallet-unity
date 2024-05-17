using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Monaverse.Editor
{
    public static class MonaverseEditorTools
    {
        [MenuItem("Tools/Monaverse/Export Packages")]
        public static void ExportUnityPackage()
        {
            ExportSDKPackage();
            ExportAPIPackage();
        }

        private static void ExportSDKPackage()
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
            
            ExportPackage(baseFolders: baseFolders,
                excludeFolders: excludeFolders,
                packageName: "Monaverse");
        }

        private static void ExportAPIPackage()
        {
            // Base folders to include
            var baseFolders = new[]
            {
                "Assets/Monaverse/Core/Plugins/Mona/com.monaverse.api",
                "Assets/Monaverse/Core/Plugins/Nethereum",
                "Assets/Monaverse/Core/Plugins/Newtonsoft 3.0.2",
            };

            // Sub-folders to exclude
            var excludeFolders = new []
            {
                "Assets/Monaverse/Core/Plugins/Mona/com.monaverse.api/Tests"
            };
            
            ExportPackage(baseFolders: baseFolders,
                excludeFolders: excludeFolders,
                packageName: "com.monaverse.api");
        }

        private static void ExportPackage(IEnumerable<string> baseFolders, IEnumerable<string> excludeFolders, string packageName)
        {
            var includePaths = baseFolders.SelectMany(folder =>
                    Directory.GetFiles(folder, "*", SearchOption.AllDirectories)
                        .Where(path => !excludeFolders.Any(path.StartsWith))
                        .Select(path => path.Replace("\\", "/")) // Normalize path for Unity
            ).ToArray();

            var packagePath = $"{packageName}.unitypackage";
            AssetDatabase.ExportPackage(includePaths, packagePath,
                ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

            UnityEngine.Debug.Log($"{packagePath} package exported successfully");
        }
    }
}