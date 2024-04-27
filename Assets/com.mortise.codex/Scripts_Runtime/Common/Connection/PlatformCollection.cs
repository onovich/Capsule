using System.IO;
using UnityEngine;

namespace MortiseFrame.Capsule {

    public static class PlatformCollection {

        public const string PLATFORM
#if UNITY_IOS
            = "iOS";
#elif UNITY_ANDROID
            = "Android";
#elif UNITY_STANDALONE_WIN
            = "StandaloneWindows64";
#else
            = "StandaloneWindows64";
#endif

        public static string GetPersistentPath() {
#if UNITY_ANDROID
            return Application.persistentDataPath;
#elif UNITY_IOS
            return Application.persistentDataPath;
#elif UNITY_STANDALONE_WIN
            return Application.persistentDataPath;
#else
            return Application.persistentDataPath;
#endif
        }

        public static string AB_StreamingAssetsPath() {
#if UNITY_ANDROID
            return Path.Combine(Application.dataPath, "!/assets");
#elif UNITY_IOS
            return Application.streamingAssetsPath;
#elif UNITY_STANDALONE_WIN
            return Application.streamingAssetsPath;
#else
            return Application.streamingAssetsPath;
#endif
        }

        public static string WWW_StreamingAssetsPath() {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
            //  return "file://" + Application.dataPath + "/StreamingAssets";
            return Application.dataPath + "/StreamingAssets";

#elif UNITY_ANDROID
            return "jar:file://" + Application.dataPath + "!/assets";
        //     return Application.streamingAssetsPath;
#elif UNITY_IOS
            return "file://" + Application.dataPath + "/Raw";
#endif
            throw new System.NotImplementedException("Check the ifdefs above.");
        }

        public static string File_StreamingAssetPath() {
#if !UNITY_EDITOR && UNITY_ANDROID
            throw new System.NotImplementedException( "You cannot open files on Android. Must use WWW");
#endif
            return Application.streamingAssetsPath;
        }

    }
}