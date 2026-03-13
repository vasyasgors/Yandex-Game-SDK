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

            // Получаем имя папки билда
            string folderName = Path.GetFileName(pathToBuiltProject.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            string archivePath = Path.Combine(parentDirectory, $"{folderName}_{version}.zip");

            if (File.Exists(archivePath))
                File.Delete(archivePath);

            ZipFile.CreateFromDirectory(pathToBuiltProject, archivePath);
        }
    }

}
