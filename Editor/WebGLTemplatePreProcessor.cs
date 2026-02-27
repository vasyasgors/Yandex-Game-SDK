using UnityEngine;
using UnityEditor;
using System.IO;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace YandexGameSdk
{
    public class WebGLTemplatePreProcessor
    {
        
        private const string PackageName = "com.beardedschoolboy.yandexgamesdk"; 
        private const string TemplateName = "YandexGameSimple";

        private FileSystemWatcher _packageWatcher;


        //[InitializeOnLoadMethod]
        private static void InitPackageWatcher()
        {
            var helper = new WebGLTemplatePreProcessor();
            helper.SetupWatcher(); 
        }

        private void SetupWatcher()
        {
            string packageRoot = GetPackageResolvedPath();
            if (string.IsNullOrEmpty(packageRoot) || !Directory.Exists(packageRoot))
            {
                Debug.LogWarning($"[WebGLTemplatePreProcessor] Не удалось инициализировать вотчер. Путь к пакету: '{packageRoot}'");
                return;
            }

            _packageWatcher?.Dispose();

            _packageWatcher = new FileSystemWatcher(packageRoot, "*.*")
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite |
                               NotifyFilters.FileName |
                               NotifyFilters.DirectoryName
            };

            _packageWatcher.Changed += OnPackagesChanged;
            _packageWatcher.Created += OnPackagesChanged;
            _packageWatcher.Deleted += OnPackagesChanged;
            _packageWatcher.Renamed += OnPackagesChanged;
            _packageWatcher.EnableRaisingEvents = true;
        }

        private void OnPackagesChanged(object sender, FileSystemEventArgs e)
        {
            PreprocessTemplate();
        }

        private void PreprocessTemplate()
        {
            string packageRoot = GetPackageResolvedPath();
            if (string.IsNullOrEmpty(packageRoot))
            {
                Debug.LogError($"[WebGLTemplatePreProcessor] Не удалось определить путь к пакету {PackageName}. Проверь установку пакета.");
                return;
            }

            string sourceFolder = Path.Combine(packageRoot, "WebGLTemplate");

            if (!Directory.Exists(sourceFolder))
            {
                Debug.LogError($"[WebGLTemplatePreProcessor] Папка шаблона не найдена: {sourceFolder}");
                return;
            }

            string destinationFolder = Path.GetFullPath("Assets/WebGLTemplates/" + TemplateName);


            FileUtil.ReplaceDirectory(sourceFolder, destinationFolder);
            AssetDatabase.Refresh();
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
