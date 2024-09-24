/*========================================================================
Copyright (c) 2020 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
=========================================================================*/

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Vuforia;

namespace SpaceAR.Client.Vuforia
{
    public class DirectoryScanner
    {
        [Tooltip("The Directory where the Area Target DataSets are stored.")]
        public static string AreaTargetsDataDirectory = "AreaTargetData";

        const string PATTERN_XML = "*.xml";
        const string PATTERN_DAT = "*.dat";
        const string PATTERN_3DT = "*.3dt";
        const string PATTERN_GLB = "*.glb";

        public static bool GetDataSetsInFolder(out Dictionary<string, string> directories)
        {
            // Clear previous file list
            directories = new Dictionary<string, string>();

            // Get the base dir (SD card on Android, or App /Documents folder on iOS)
            var storageRoot = GetStorageRoot();
            var scanningDirectory = new DirectoryInfo(storageRoot);

            if (!scanningDirectory.Exists)
            {
                scanningDirectory.Create();
                return false;
            }

            var directoryInfos = scanningDirectory.GetDirectories();

            foreach (var directoryInfo in directoryInfos)
            {
                var datasetFileXML = FindFileWithExtension(directoryInfo.FullName, PATTERN_XML);
                var datasetFileDat = FindFileWithExtension(directoryInfo.FullName, PATTERN_DAT);
                var datasetFileOcclusionModel = FindFileWithExtension(directoryInfo.FullName, PATTERN_3DT);
                var datasetFileGLB = FindFileWithExtension(directoryInfo.FullName, PATTERN_GLB);

                // Only display datasets that have both an .xml and .dat file
                if (datasetFileDat != null && datasetFileXML != null && (datasetFileOcclusionModel != null || datasetFileGLB != null))
                    // Keep track of full path by directory name
                    directories[directoryInfo.Name] = directoryInfo.FullName;
                else
                    Debug.LogError("Could not find dataset files in directory " + directoryInfo.Name);
            }

            return true;
        }

        public static string GetStorageRoot()
        {
            if (VuforiaRuntimeUtilities.IsPlayMode())
                //return Path.Combine(Application.streamingAssetsPath, "Vuforia", AreaTargetsDataDirectory);
                return Path.Combine(Application.persistentDataPath, AreaTargetsDataDirectory);

            // On Android, Application.persistentDataPath should look like:
            // '/sdcard/Android/data/com.myorg.myapp/files'
            // 
            // On iOS, Application.persistentDataPath points to the 
            // 'Documents' folder of the App
            return Path.Combine(Application.persistentDataPath, AreaTargetsDataDirectory);
        }

        public static string FindFileWithExtension(string dirPath, string extension)
        {
            if (!Directory.Exists(dirPath))
                return null;

            var files = new DirectoryInfo(dirPath).GetFiles(extension, SearchOption.AllDirectories);
            if (files.Length == 0)
                return null;

            return files[0].Name;
        }
    }
}
