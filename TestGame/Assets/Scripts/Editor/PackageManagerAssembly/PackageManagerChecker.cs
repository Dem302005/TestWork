using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class PackageChecker
{
    private static AddRequest addRequest;
    private static ListRequest listRequest;
    private static readonly Stack<int> missingPackages = new Stack<int>();

    private static List<PackageEntry> packageToAdd;

    [InitializeOnLoadMethod]
    private static void CheckPackage()
    {
        var filePath = Application.dataPath + "/../Library/PackageChecked";

        if (!File.Exists(filePath))
        {
            Debug.Log("[Auto Package] : Checking if required packages are included");

            var packageListFile =
                Directory.GetFiles(Application.dataPath, "PackageImportList.txt", SearchOption.AllDirectories);
            if (packageListFile.Length == 0)
            {
                Debug.LogError(
                    "[Auto Package] : Couldn't find the packages list. Be sure there is a file called PackageImportList in your project");
            }
            else
            {
                var packageListPath = packageListFile[0];
                packageToAdd = new List<PackageEntry>();
                var content = File.ReadAllLines(packageListPath);
                foreach (var line in content)
                {
                    var split = line.Split('@');
                    var entry = new PackageEntry();

                    entry.name = split[0];
                    entry.version = split.Length > 1 ? split[1] : null;

                    packageToAdd.Add(entry);
                }

                File.WriteAllText(filePath, "Delete this to trigger a new auto package check");
                listRequest = Client.List();
                EditorApplication.update += Update;
            }
        }
    }

    private static void Update()
    {
        if (listRequest != null)
        {
            if (listRequest.IsCompleted)
            {
                var foundPackages = new bool[packageToAdd.Count];

                for (var i = 0; i < foundPackages.Length; ++i)
                    foundPackages[i] = false;

                foreach (var package in listRequest.Result)
                    for (var i = 0; i < foundPackages.Length; ++i)
                        if (package.packageId.Contains(packageToAdd[i].name))
                        {
                            foundPackages[i] = true;
                            Debug.Log("[Auto package] Package " + packageToAdd[i].name +
                                      " already imported in that project");
                        }

                for (var i = 0; i < foundPackages.Length; ++i)
                    if (!foundPackages[i])
                        missingPackages.Push(i);

                listRequest = null;
            }
            else if (listRequest.Error != null)
            {
                Debug.Log(listRequest.Error.message);
                listRequest = null;
            }
        }
        else
        {
            var noMorePackage = false;

            if (missingPackages.Count > 0)
                EditorUtility.DisplayProgressBar("Importing package", "Importing missing package for the project",
                    1.0f - missingPackages.Count / (float)packageToAdd.Count);
            else
                EditorUtility.ClearProgressBar();

            if (addRequest == null)
            {
                if (missingPackages.Count == 0)
                {
                    noMorePackage = true;
                }
                else
                {
                    var package = missingPackages.Pop();
                    var name = packageToAdd[package].name;
                    if (packageToAdd[package].version != null)
                        name += "@" + packageToAdd[package].version;

                    addRequest = Client.Add(name);
                }
            }
            else
            {
                if (addRequest.IsCompleted)
                {
                    if (addRequest.Error != null)
                        Debug.LogError("[Auto Package Error] : " + addRequest.Error.message);
                    else if (addRequest.Result != null)
                        Debug.Log("[Auto Package] : Automatically added package " + addRequest.Result.displayName);
                    else
                        Debug.LogError("[Auto Package] : Unknown error with adding new package to the Package Manager");

                    addRequest = null;
                }
            }

            if (noMorePackage)
            {
                Debug.Log("[Auto Package] : All packages checked");
                EditorApplication.update -= Update;
            }
        }
    }

    public class PackageEntry
    {
        public string name;
        public string version;
    }
}