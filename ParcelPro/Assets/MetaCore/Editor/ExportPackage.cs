using UnityEngine;
using UnityEditor;

public class ExportPackage
{
    [MenuItem("Export/MetaCore SDK")]
    static void export()
    {
        AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), PlayerSettings.productName + ".unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
    }
}




//using UnityEngine;
//using System.Collections;
//using UnityEditor;

//public static class PackageExport
//{


//    [MenuItem("Export/Export with tags and layers, Input settings")]
//    public static void export()
//    {
//        string[] projectContent = new string[] { "Assets", "Packages/Newtonsoft Json", "ProjectSettings/TagManager.asset", "ProjectSettings/InputManager.asset", "ProjectSettings/ProjectSettings.asset" };
//        AssetDatabase.ExportPackage(projectContent, "Done.unitypackage", ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
//        Debug.Log("Project Exported");
//    }

//}