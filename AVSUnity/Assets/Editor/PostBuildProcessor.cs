using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;

public class PostBuildProcessor : MonoBehaviour
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        UnityEngine.Debug.Log(pathToBuiltProject);
        var processInfo = new ProcessStartInfo("cmd.exe");
        processInfo.Arguments = "/k cd .. && cd build && docker-compose -f ./Docker/docker-compose.yml build && docker login --username avsss17 --password avsss17 && docker tag docker_avsbuild avsss17/avsss17 && docker push avsss17/avsss17 && pause";
        //processInfo.WorkingDirectory
        processInfo.CreateNoWindow = false;
        processInfo.UseShellExecute = false;

        var process = Process.Start(processInfo);
        process.WaitForExit();
        //process.Close();
    }
}
