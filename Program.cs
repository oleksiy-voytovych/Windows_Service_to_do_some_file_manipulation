using System;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace AppDeployer 
{
    class Program  
    {
        static void Main(string[] args){
            string serverUrl = "https://example.com/deployment-instructions.txt";
            WebRequest request = WebRequest.Create(serverUrl);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            byte[] responseBytes = new byte[responseStream.Length];
            responseStream.Read(responseBytes, 0, responseBytes.Length);

            // Decode the deployment instructions.
            string deploymentInstructions = Encoding.UTF8.GetString(responseBytes);

            // Iterate over the deployment instructions.
            foreach (string instruction in deploymentInstructions.Split('\n'))
            {
                // Parse the deployment instruction.
                string[] instructionParts = instruction.Split(' ');
                string action = instructionParts[0];
                string appName = instructionParts[1];
                string appVersion = instructionParts[2];

                // Perform the deployment action.
                switch (action)
                {
                    case "install":
                        InstallApp(appName, appVersion);
                        break;
                    case "update":
                        UpgradeApp(appName, appVersion);
                        break;
                    case "uninstall":
                        UninstallApp(appName);
                        break;
                }
            }
        }


        }
        private static void InstallApp(string appName, string appVersion){


            string appUrl = $"https://example.com/apps/{appName}/{appVersion}.zip";
            WebRequest request = WebRequest.Create(appUrl);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            byte[] responseBytes = new byte[responseStream.Length];
            responseStream.Read(responseBytes, 0 ,responseBytes.Length);

            string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), appName);
            ZipArchive zipArchive = ZipArchive.Open(new MemoryStream(responseBytes));
            zipArchive.ExtractToDirectory(appPath);

            string appExecutablePath = Path.Combine(appPath, $"{appName}.exe");
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup); 
            shortcut shortcut = new shortcut();
            shortcut.TargetPath = appExecutablePath;
            shortcut.WorkingDirectory = appPath;
            shortcut.Save(Path.Combine(startupPath, $"{appName}.lnk"));
        }
           private static void UpgradeApp(string appName, string appVersion){
            UninstallApp(appName);

            InstallApp(appName, appVersion);


           }
        private static void UninstallApp(string appName){
            string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), appName);

            Directory.Delete(appPath, true);
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            File.Delete(Path.Combine(startupPath, $"{appName}.lnk"));
        }

    }








