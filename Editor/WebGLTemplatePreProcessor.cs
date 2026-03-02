using UnityEngine;
using UnityEditor;
using System.IO;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace YandexGameSdk
{
    public class WebGLTemplatePreProcessor
    {
        private const string PackageName = "com.beardedschoolboy.yandexgamesdk";

        [MenuItem("Tools/YandexGameSDK/Update WebGL Template")]
        private static void UpdateTemplateFromMenu()
        {
            CopyTemplateToAssets();
        }

        private static void CopyTemplateToAssets()
        {
            string packageRoot = GetPackageResolvedPath();
            if (string.IsNullOrEmpty(packageRoot))
            {
                Debug.LogError($"[WebGLTemplatePreProcessor] Не удалось определить путь к пакету {PackageName}. Проверьте установку пакета.");
                return;
            }

            string sourceFolder = Path.Combine(packageRoot, "WebGLTemplate");

            if (!Directory.Exists(sourceFolder))
            {
                Debug.LogError($"[WebGLTemplatePreProcessor] Папка шаблона не найдена: {sourceFolder}");
                return;
            }

            string destinationFolder = Path.GetFullPath("Assets/WebGLTemplate");

            try
            {
                // Создаем папку если её нет
                Directory.CreateDirectory(destinationFolder);

                // Копируем ВСЕ содержимое из sourceFolder в destinationFolder
                DirectoryCopy(sourceFolder, destinationFolder, true);

                AssetDatabase.Refresh();

                Debug.Log($"[WebGLTemplatePreProcessor] Содержимое WebGLTemplate успешно скопировано в Assets/WebGLTemplate!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WebGLTemplatePreProcessor] Ошибка при копировании шаблона: {ex.Message}");
            }
        }

        // Рекурсивное копирование содержимого директории
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            // Копируем файлы из корня
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // Копируем подпапки
            if (copySubDirs)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    Directory.CreateDirectory(temppath);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static string GetPackageResolvedPath()
        {
            string assetPath = $"Packages/{PackageName}";
            PackageInfo info = PackageInfo.FindForAssetPath(assetPath);

            if (info == null)
            {
                Debug.LogError($"[WebGLTemplatePreProcessor] PackageInfo not found for asset path: {assetPath}");
                return null;
            }

            string resolvedPath = info.resolvedPath;
            if (string.IsNullOrEmpty(resolvedPath))
            {
                Debug.LogError($"[WebGLTemplatePreProcessor] resolvedPath is null or empty for package {PackageName}");
                return null;
            }

            return Path.GetFullPath(resolvedPath);
        }
    }
}
