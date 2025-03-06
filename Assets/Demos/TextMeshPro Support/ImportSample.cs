#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

internal static class ImportSample
{
    [MenuItem("Development/Import TextMeshPro Support", false, 2001)]
    private static void ImportTMPSupportV1()
    {
        Run("TextMeshPro Support");
    }

    [MenuItem("Development/Import TextMeshPro Support (Unity 6)", false, 2002)]
    private static void ImportTMPSupportV2()
    {
        Run("TextMeshPro Support (Unity 6)");
    }

    [InitializeOnLoadMethod]
    private static void ImportSampleOnLoad()
    {
#if UNITY_2023_2_OR_NEWER
        ImportTMPSupportV2();
#else
        ImportTMPSupportV1();
#endif
    }

    private static void Run(string sample)
    {
#if UNITY_EDITOR_WIN
        var bash = @"C:\Program Files\Git\bin\bash.exe";
        if (!File.Exists(bash))
        {
            bash = @"C:\Program Files (x86)\Git\bin\bash.exe";
        }
#else
        var bash = "/bin/bash";
#endif

        var p = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                Arguments = $"import-tmp-support.sh '{sample}'",
                CreateNoWindow = true,
                FileName = bash,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = $"{Application.dataPath}/..",
                UseShellExecute = false
            },
            EnableRaisingEvents = true
        };

        p.Exited += (_, __) =>
        {
            var result = new StringBuilder();
            var stdout = p.StandardOutput.ReadToEnd();
            result.Append("stdout: ");
            result.Append(stdout);
            result.AppendLine();
            result.Append("stderr: ");
            result.Append(p.StandardError.ReadToEnd());
            Debug.Log(result);

            if (p.ExitCode == 0 && stdout.StartsWith("Imported: "))
            {
                var path = stdout.Replace("Imported: ", "").Trim();
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ImportRecursive);
            }
        };

        p.Start();
    }
}
#endif
