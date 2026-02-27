using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using System.IO;
using System.IO.Compression;


public class PostProcessBuild 
{
    [PostProcessBuild]
    public static void CreateZip(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.WebGL)
        {
            string version = PlayerSettings.bundleVersion; 
            string parentDirectory = Directory.GetParent(pathToBuiltProject).FullName;
            string archivePath = Path.Combine(parentDirectory, $"WebBuild_{version}.zip");

            if (File.Exists(archivePath))
                File.Delete(archivePath);

            ZipFile.CreateFromDirectory(pathToBuiltProject, archivePath);
        }
    }

}
