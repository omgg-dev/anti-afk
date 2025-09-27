using System.IO;
using UnityEditor;
using UnityEngine;

public static class Exporter {

    private static readonly string _ExportPath = "ExportedPackages/anti-afk.unitypackage";

    [MenuItem("OMGG/Export Package")] // Allow Unity to call this method from the menu bar
    public static void ExportPackage()
    {
        // All path to export from Assets folder
        string[] assetsToExport = {
            "Assets/OMGG/Package/anti-afk/Demo",
            "Assets/OMGG/Package/anti-afk/Scripts",
            "Assets/OMGG/Package/anti-afk/Resources",
            "Assets/OMGG/Package/anti-afk/Editor"
        };

        // Create an export folder if needed
        string dir = Path.GetDirectoryName(_ExportPath);

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        // Export the package
        AssetDatabase.ExportPackage(assetsToExport, _ExportPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);

        Debug.Log($"Export completed: {_ExportPath}");
    }
}
