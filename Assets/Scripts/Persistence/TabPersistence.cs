using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using UnityEngine;

public static class TabPersistence
{
    public static string TEMP_EXPORT_FOLDER = UnityEngine.Application.temporaryCachePath + Path.DirectorySeparatorChar + "Export" + Path.DirectorySeparatorChar;
    public static string TEMP_EXPORT_ARCHIVE = UnityEngine.Application.temporaryCachePath + Path.DirectorySeparatorChar + "Pinger-Tabs.pinger";

    public static string TEMP_IMPORT_FOLDER = UnityEngine.Application.temporaryCachePath + Path.DirectorySeparatorChar + "Import" + Path.DirectorySeparatorChar;
    public static string TEMP_IMPORT_ARCHIVE = UnityEngine.Application.temporaryCachePath + Path.DirectorySeparatorChar + "Import-Tabs.zip";

    public static void RenameTab(string pFrom, string pTo)
    {
        string from = Persistence.BASE_PATH + Persistence.TABS_FOLDER + pFrom + ".txt";
        string to = Persistence.BASE_PATH + Persistence.TABS_FOLDER + pTo + ".txt";
        File.Move(from, to);
    }

    public static List<string> GetAllTabNames()
    {
        List<string> tabNames = new List<string>();
        string[] paths = Persistence.GetFilesInDirectory(Persistence.TABS_FOLDER);
        if (paths != null)
        {
            foreach (string path in paths)
            {
                string name = path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                name = name.Substring(0, name.IndexOf("."));
                tabNames.Add(name);
            }
        }
        return tabNames;
    }

    public static void DeleteTab(string pName)
    {
        File.Delete(Persistence.BASE_PATH + Persistence.TABS_FOLDER + pName + ".txt");
    }

    public static void ExportTabs(NativeShare.ShareResultCallback pCallback, params string[] pTabs)
    {
        ClearTemporaryExport();
        Directory.CreateDirectory(TEMP_EXPORT_FOLDER);

        foreach (string tab in pTabs)
        {
            Debug.Log("Tabname: " + tab);
            string from = Persistence.BASE_PATH + Persistence.TABS_FOLDER + tab + ".txt";
            string to = TEMP_EXPORT_FOLDER + tab + ".txt";
            File.Copy(from, to);
        }

        // Comress tabs to zip
        ZipFile.CreateFromDirectory(TEMP_EXPORT_FOLDER, TEMP_EXPORT_ARCHIVE);

        // Delete temporary files after sharing
        NativeShare.ShareResultCallback callback = (result, shareTarget) =>
        {
            Debug.Log("Share result: " + result + ", selected app: " + shareTarget);
            ClearTemporaryExport();
            pCallback?.Invoke(result, shareTarget);
        };

        // Share zip
        new NativeShare()
            .AddFile(TEMP_EXPORT_ARCHIVE)
            .SetSubject("Subject goes here")
            .SetText("Hello world!")
            .SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback(callback)
            .Share();
    }

    private static void ClearTemporaryExport()
    {
        if (File.Exists(TEMP_EXPORT_ARCHIVE))
        {
            File.Delete(TEMP_EXPORT_ARCHIVE);
        }

        if (Directory.Exists(TEMP_EXPORT_FOLDER))
        {
            Directory.Delete(TEMP_EXPORT_FOLDER, true);
        }
    }

    public static bool ImportTab()
    {
        ClearTemporaryImport();
        if (!ImportFromIntent(TEMP_IMPORT_ARCHIVE))
        {
            return false;
        }
        
        // Decompress the zip file
        Directory.CreateDirectory(TEMP_IMPORT_FOLDER);
        ZipFile.ExtractToDirectory(TEMP_IMPORT_ARCHIVE, TEMP_IMPORT_FOLDER);

        // Copy tabs to storage if the names are unique
        string[] importTabs = Directory.GetFiles(TEMP_IMPORT_FOLDER);
        List<string> existingTabs = GetAllTabNames();
        List<string> invalidTabs = new List<string>();
        foreach (string imported in importTabs)
        {
            string name = imported.Substring(imported.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            name = name.Substring(0, name.IndexOf("."));
            if (!existingTabs.Contains(name))
            {
                string to = Persistence.BASE_PATH + Persistence.TABS_FOLDER + name + ".txt";
                File.Copy(imported, to);
            }
            else
            {
                invalidTabs.Add(name);
            }
        }

        // Show dialog to inform the user
        if (invalidTabs.Count == 0)
        {
            NativeToolkit.ShowAlert("Success", "Successfully imported all tabs.");
        }
        else
        {
            string invalidName = "";
            int iterations = Math.Min(3, invalidTabs.Count);
            for (int i = 0; i < iterations; i++)
            {
                invalidName += invalidTabs[i];
                invalidName += (i != iterations - 1) ? ", " : ".";
            }
            NativeToolkit.ShowAlert("Names already in use", "Some tabs couldn't be copied because their names are already in use. Names in quesion are: " + invalidName);
        }

        ClearTemporaryImport();
        return true;
    }

    private static void ClearTemporaryImport()
    {
        if (File.Exists(TEMP_IMPORT_ARCHIVE))
        {
            File.Delete(TEMP_IMPORT_ARCHIVE);
        }

        if (Directory.Exists(TEMP_IMPORT_FOLDER))
        {
           Directory.Delete(TEMP_IMPORT_FOLDER, true);
        }
    }

    // Combination of
    // https://answers.unity.com/questions/1350799/convert-android-uri-to-a-file-path-unity-can-read.html
    // https://answers.unity.com/questions/1327186/how-to-get-intent-data-for-unity-to-use.html
    private static string prevIntentURI;
    private static bool ImportFromIntent(string importPath)
    {
        try
        {
            // Get the current activity
            AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activityObject = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            // Get the current intent
            AndroidJavaObject intent = activityObject.Call<AndroidJavaObject>("getIntent");

            // TODO: Check if causes issues with some file providers (only tested with whatsapp)
            // Check if the same uri has been imported twice in a row. Only happens if the user shows all opened apps and then reopens the unity app from the list
            // When importing whatsapp generates a new uri everytime a file is pressed
            string currentURI = intent.Call<string>("getDataString");
            if (prevIntentURI == currentURI)
            {
                return false;
            }
            prevIntentURI = currentURI;

            // Get the intent data using AndroidJNI.CallObjectMethod so we can check for null
            IntPtr method_getData = AndroidJNIHelper.GetMethodID(intent.GetRawClass(), "getData", "()Ljava/lang/Object;");
            IntPtr getDataResult = AndroidJNI.CallObjectMethod(intent.GetRawObject(), method_getData, AndroidJNIHelper.CreateJNIArgArray(new object[0]));
            if (getDataResult.ToInt32() != 0)
            {
                // Now actually get the data. We should be able to get it from the result of AndroidJNI.CallObjectMethod, but I don't now how so just call again
                AndroidJavaObject intentURI = intent.Call<AndroidJavaObject>("getData");

                // Open the URI as an input channel
                AndroidJavaObject contentResolver = activityObject.Call<AndroidJavaObject>("getContentResolver");
                AndroidJavaObject inputStream = contentResolver.Call<AndroidJavaObject>("openInputStream", intentURI);
                AndroidJavaObject inputChannel = inputStream.Call<AndroidJavaObject>("getChannel");

                // Open an output channel
                AndroidJavaObject outputStream = new AndroidJavaObject("java.io.FileOutputStream", importPath);
                AndroidJavaObject outputChannel = outputStream.Call<AndroidJavaObject>("getChannel");

                // Copy the file
                long bytesTransfered = 0;
                long bytesTotal = inputChannel.Call<long>("size");
                while (bytesTransfered < bytesTotal)
                {
                    bytesTransfered += inputChannel.Call<long>("transferTo", bytesTransfered, bytesTotal, outputChannel);
                }

                // Close the streams
                inputStream.Call("close");
                outputStream.Call("close");
                return true;
            }
        }
        catch (System.Exception ex) { }

        return false;
    }
}
