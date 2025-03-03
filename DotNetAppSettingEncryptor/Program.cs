using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace DotNetAppSettingEncryptor;

/// <summary>
/// 
/// </summary>
internal class Program
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    private static void Main(string[] args)
    {
        Console.Title = "Dot Net Encryptor Decryptor | by @ahmadrezadev";
        Console.WriteLine("\r\n            _                         _                       _            \r\n       __ _| |__  _ __ ___   __ _  __| |_ __ ___ ______ _  __| | _____   __\r\n      / _` | '_ \\| '_ ` _ \\ / _` |/ _` | '__/ _ \\_  / _` |/ _` |/ _ \\ \\ / /\r\n     | (_| | | | | | | | | | (_| | (_| | | |  __// / (_| | (_| |  __/\\ V / \r\n      \\__,_|_| |_|_| |_| |_|\\__,_|\\__,_|_|  \\___/___\\__,_|\\__,_|\\___| \\_/  \r\n                           powered by ahmadrezadev.ir\r\n");

        if (args is { Length: > 0 })
        {
            #region Get Keys | دریافت کلید ها

            // args = ["key"];
            if (args[0].ToLower() == "get-keys" || args[0].ToLower() == "key")
            {
                Console.WriteLine("Getting Keys ...");

                try
                {
                    string key;
                    string iv;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        key = Environment.GetEnvironmentVariable("AES_KEY", EnvironmentVariableTarget.Machine)!;
                        iv = Environment.GetEnvironmentVariable("AES_IV", EnvironmentVariableTarget.Machine)!;
                    }
                    else
                    {
                        var startInfo = new ProcessStartInfo("bash", "-c \"echo $AES_KEY\"")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        };
                        using (var process = Process.Start(startInfo))
                        {
                            using (var reader = process!.StandardOutput)
                            {
                                key = reader.ReadToEnd();
                            }
                        }

                        startInfo = new ProcessStartInfo("bash", "-c \"echo $AES_IV\"")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        };
                        using (var process = Process.Start(startInfo))
                        {
                            using (var reader = process!.StandardOutput)
                            {
                                iv = reader.ReadToEnd();
                            }
                        }
                    }

                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine($"AES Key: {key}");
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine($"AES IV: {iv}");
                    Console.WriteLine(Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("an error has occurred! " + ex.Message);
                }
            }

            #endregion

            #region Generate New Keys | ساخت کلید جدید

            // args = ["nk"];
            if (args[0].ToLower() == "new-keys" || args[0].ToLower() == "nk")
            {
                Console.WriteLine("Generating new AES Keys ...");

                try
                {
                    if (!Directory.Exists("_output"))
                        Directory.CreateDirectory("_output");

                    var aesKeyFilePath = $"_output/AES_Key_{DateTime.Now:yyyyMMddHHmm}.txt";
                    var aesIvFilePath = $"_output/AES_IV_{DateTime.Now:yyyyMMddHHmm}.txt";

                    var key = Convert.ToBase64String(Aes.Create().Key);
                    var iv = Convert.ToBase64String(Aes.Create().IV);
                    
                    File.WriteAllText(aesKeyFilePath, key);
                    File.WriteAllText(aesIvFilePath, iv);

                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine($"AES Key: {key}");
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine(Environment.NewLine);
                    Console.WriteLine($"AES IV: {iv}");
                    Console.WriteLine(Environment.NewLine);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("an error has occurred! " + ex.Message);
                }

                Console.WriteLine("New keys has been generated.");
            }

            #endregion

            #region Set Keys In Environment Variables | تنظیم کلیدها در متغیرهای محیطی

            // args = ["sk", "twaZ7qjlSbwPaKnj1w1sjj3Cn/bfboJMiUXsQ2cIhDg=", "CSSpiqG8mPm/IeRnVKUAaA=="];
            if (args[0].ToLower() == "set-keys" || args[0].ToLower() == "sk")
            {
                Console.WriteLine("Setting AES Keys in Environment Variables ...");
                try
                {
                    var key = args[1];
                    var iv = args[2];
                    var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                    if (isWindows)
                    {
                        try
                        {
                            var processInfo = new ProcessStartInfo("cmd.exe", "/c setx AES_KEY \"" + key + "\" /M && setx AES_IV \"" + iv + "\" /M")
                            {
                                Verb = "runas", // Run as Administrator 
                                UseShellExecute = true, 
                                CreateNoWindow = true,  
                                WindowStyle = ProcessWindowStyle.Hidden 
                            };
                            using var ps =  Process.Start(processInfo);
                            ps!.WaitForExit();
                            
                            var testKey = Environment.GetEnvironmentVariable("AES_KEY", EnvironmentVariableTarget.Machine)!;
                            var testIv = Environment.GetEnvironmentVariable("AES_IV", EnvironmentVariableTarget.Machine)!;

                            if (key != testKey || iv != testIv)
                                Console.WriteLine("an error has occurred, please try again or set it manually");
                            else
                                Console.WriteLine("Keys has been set in Environment Variables.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("an error has occurred! " + ex.Message);
                        }
                    }
                    else
                    {
                        var aesKeyCommand = $"echo 'AES_KEY=\"{key}\"' | sudo tee -a /etc/environment";
                        var aesIvCommand = $"echo 'AES_IV=\"{iv}\"' | sudo tee -a /etc/environment";

                        try
                        {
                            var startInfo = new ProcessStartInfo("bash", $"-c \"{aesKeyCommand} && {aesIvCommand}\"")
                            {
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                UseShellExecute = false
                            };

                            using var process = Process.Start(startInfo);
                            process!.WaitForExit();

                            Console.WriteLine("Environment variables have been updated successfully.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }

                        //const string envFile = "/etc/environment";
                        //var aesKeyCommand = $"AES_KEY=\"{key}\"";
                        //var aesIvCommand = $"AES_IV=\"{iv}\"";

                        //try
                        //{
                        //    var fileContent = File.ReadAllLines(envFile).ToList();

                        //    var updated = false;
                        //    for (var i = 0; i < fileContent.Count; i++)
                        //    {
                        //        if (fileContent[i].StartsWith("AES_KEY="))
                        //        {
                        //            fileContent[i] = aesKeyCommand;
                        //            updated = true;
                        //        }
                        //        if (fileContent[i].StartsWith("AES_IV="))
                        //        {
                        //            fileContent[i] = aesIvCommand;
                        //            updated = true;
                        //        }
                        //    }

                        //    if (!updated)
                        //    {
                        //        fileContent.Add(aesKeyCommand);
                        //        fileContent.Add(aesIvCommand);
                        //    }
                        //    File.WriteAllLines(envFile, fileContent);
                        //    Console.WriteLine("Environment variables have been updated successfully.");

                        //    var startInfo = new ProcessStartInfo("bash", "-c \"source /etc/environment\"")
                        //    {
                        //        Verb = "runas", // Run as Administrator 
                        //        UseShellExecute = true,
                        //        CreateNoWindow = true,
                        //        WindowStyle = ProcessWindowStyle.Hidden
                        //    };
                        //    Process.Start(startInfo);

                        //    var testKey = "";
                        //    var testIv = "";
                        //    startInfo = new ProcessStartInfo("bash", $"-c \"echo $AES_KEY\"")
                        //    {
                        //        RedirectStandardOutput = true,
                        //        UseShellExecute = false
                        //    };
                        //    using (var process = Process.Start(startInfo))
                        //    {
                        //        using (var reader = process!.StandardOutput)
                        //        {
                        //            testKey = reader.ReadToEnd();
                        //        }
                        //    }

                        //    startInfo = new ProcessStartInfo("bash", "-c \"echo $AES_IV\"")
                        //    {
                        //        RedirectStandardOutput = true,
                        //        UseShellExecute = false
                        //    };
                        //    using (var process = Process.Start(startInfo))
                        //    {
                        //        using (var reader = process!.StandardOutput)
                        //        {
                        //            testIv = reader.ReadToEnd();
                        //        }
                        //    }

                        //    if (key != testKey || iv != testIv)
                        //        Console.WriteLine("an error has occurred, please try again or set it manually");
                        //    else
                        //        Console.WriteLine("Keys has been set in Environment Variables.");
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine("an error has occurred! " + ex.Message);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("an error has occurred! " + ex.Message);
                }
            }

            #endregion

            #region Encrypt File | رمزگذاری فایل

            // args = ["enc", "_input/appsettings.json", "_output/appsettings.json"];
            if (args[0].ToLower() == "encrypt" || args[0].ToLower() == "enc")
            {
                try
                {
                    Console.WriteLine("Encrypting files ...");

                    string key;
                    string iv;
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        key = Environment.GetEnvironmentVariable("AES_KEY", EnvironmentVariableTarget.Machine)!;
                        iv = Environment.GetEnvironmentVariable("AES_IV", EnvironmentVariableTarget.Machine)!;
                    }
                    else
                    {
                        var startInfo = new ProcessStartInfo("bash", "-c \"echo $AES_KEY\"")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        };
                        using (var process = Process.Start(startInfo))
                        {
                            using (var reader = process!.StandardOutput)
                            {
                                key = reader.ReadToEnd();
                            }
                        }

                        startInfo = new ProcessStartInfo("bash", "-c \"echo $AES_IV\"")
                        {
                            RedirectStandardOutput = true,
                            UseShellExecute = false
                        };
                        using (var process = Process.Start(startInfo))
                        {
                            using (var reader = process!.StandardOutput)
                            {
                                iv = reader.ReadToEnd();
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(key))
                        throw new Exception("AES_KEY is null or white space!");
                    if (string.IsNullOrWhiteSpace(iv))
                        throw new Exception("AES_IV is null or white space!");

                    var jsonContent = File.ReadAllText(args[1]);
                    var aes = new AesEncryption(key, iv);
                    var encryptedData = aes.Encrypt(jsonContent);
                    File.WriteAllBytes(args[2], encryptedData);

                    Console.WriteLine("appsettings.json has been encrypted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("an error has occurred! " + ex.Message);
                }
            }

            #endregion
        }
        else
        {
            try
            {
                Console.WriteLine("Get files from _input path ...");

                if (!Directory.Exists("_input"))
                    Directory.CreateDirectory("_input");
                if (!Directory.Exists("_output"))
                    Directory.CreateDirectory("_output");

                const string jsonFilePath = "_input/appsettings.json";
                const string aesKeyFilePath = "_input/AES_Key.txt";
                const string aesIvFilePath = "_input/AES_IV.txt";

                var key = File.ReadAllText(aesKeyFilePath);
                var iv = File.ReadAllText(aesIvFilePath);
                var jsonContent = File.ReadAllText(jsonFilePath);

                var aes = new AesEncryption(key, iv);
                var encryptedData = aes.Encrypt(jsonContent);
                var decryptedJson = aes.Decrypt(encryptedData);

                var encryptedFilePath = "_output/appsettings.enc";
                File.WriteAllBytes(encryptedFilePath, encryptedData);

                Console.WriteLine("appsettings.json has been encrypted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("an error has occurred! " + ex.Message);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    static string GetShellConfigFile()
    {
        var shell = Environment.GetEnvironmentVariable("SHELL");
        if (shell != null && shell.EndsWith("zsh"))
            return ".zshrc";
        return ".bashrc"; // پیش‌فرض برای لینوکس
    }
}

/*
 *
 * Developed by Ahmadreza Bahramian (http://ahmadrezadev.ir | 09039818200)
 *
 */