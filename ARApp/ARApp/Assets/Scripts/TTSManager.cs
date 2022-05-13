using UnityEngine;

public static class TTSManager {

    private static AndroidJavaClass _mainactivity = null;
    private static AndroidJavaClass _libraryclass = null;

    const string _MainActivity = "com.unity3d.player.UnityPlayer";
    const string _LibraryClass = "com.java.ttslibrary.TTSLib";
    const string _MethodName = "Speak"; 
    const string _CheckAvailability = "GetLanguageAvailability"; 
    const string _getlocale = "GetLocale";
    const string _getavailablelocales = "GetAvailableLocales";
    const string _setlocale = "SetLocale";
    const string _setEngineByPackageName = "setEngineByPackageName"; 
    const string _downloadttsdata = "DownloadTTSData"; 
    const string _shutdowntts = "StopTTS"; 
    const string _uttering = "IsUttering"; 
    const string _setpitch = "SetPitch"; 
    const string _setspeed = "SetSpeedRate"; 
    const string _isbootedup = "IsRunning"; 
    const string _stopspeaking = "StopUtterance";
    const string _starttts = "StartTTS";

    public static void BootUpTTS() {
        #if UNITY_ANDROID && !UNITY_EDITOR
            _mainactivity = new AndroidJavaClass(_MainActivity);
            _libraryclass = new AndroidJavaClass(_LibraryClass);
            _libraryclass.CallStatic(_starttts, _mainactivity.GetStatic<AndroidJavaObject>("currentActivity"));
            SetSpeechRate(0.8f);
        #endif
    }

    public static void SetSpeechRate(float value) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (_libraryclass!=null) {
            _libraryclass.CallStatic(_setspeed, value);
        }
        #endif
    }

    public static void SetPitch(float value) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (_libraryclass!=null) {
            _libraryclass.CallStatic(_setpitch, value);
        }
        #endif
    }

    public static bool GetLanguageAvailability(string locale) { 

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            return false;
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
            return (_libraryclass!=null)?_libraryclass.CallStatic<bool>(_CheckAvailability,_mainactivity.GetStatic<AndroidJavaObject>("currentActivity"), locale):false;
        #else 
            return false;
        #endif
    }

    public static void Speak(string sentence) {

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            return;
        }

        #if UNITY_ANDROID && !UNITY_EDITOR
        if(_libraryclass==null){
            BootUpTTS();
        }
        if(_libraryclass!= null && !_libraryclass.CallStatic<bool>(_uttering)) {
            _libraryclass.CallStatic(_MethodName,sentence);
        }
        #endif
    }

    public static void DownloadTTSData() { 

        if (Application.internetReachability == NetworkReachability.NotReachable) {
            return;
        }
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (_libraryclass!=null) {
            _libraryclass.CallStatic(_downloadttsdata, _mainactivity.GetStatic<AndroidJavaObject>("currentActivity"));
        }
        #endif
    }

    public static void StopTTS() {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (_libraryclass!=null) {
            _libraryclass.CallStatic(_shutdowntts);
        }
        #endif
        _mainactivity = null;
        _libraryclass = null;
    }

    public static string[] GetAvailableLocales() {
        #if UNITY_ANDROID && !UNITY_EDITOR
        return (_libraryclass!=null)?_libraryclass.CallStatic<string[]>(_getavailablelocales):null;
        #else
        return null;
        #endif
    }

    public static bool SetLocale(string locale, string script = "", string region = "") { 
        #if UNITY_ANDROID && !UNITY_EDITOR
        return (_libraryclass!=null) ? _libraryclass.CallStatic<bool>(_setlocale, locale,script,region) : false;
        #else
        return false;
        #endif
    }

    public static string GetLocale() {
        #if UNITY_ANDROID && !UNITY_EDITOR
        return (_libraryclass!=null) ? _libraryclass.CallStatic<string>(_getlocale) : "TTS Has not been initialized!";
        #else 
        return "";
        #endif
    }

    public static bool IsBootedUp() {
        #if UNITY_ANDROID && !UNITY_EDITOR
        return (_libraryclass!=null) ? _libraryclass.CallStatic<bool>(_isbootedup) : false;
        #else
        return false;
        #endif
    }

    public static bool IsUttering() {

        #if UNITY_ANDROID && !UNITY_EDITOR
        return (_libraryclass!=null)?_libraryclass.CallStatic<bool>(_uttering) : false;
        #else
        return false;
        #endif
    }

    public static bool SetEngineByPackageName(string packagename) {
        #if UNITY_ANDROID && !UNITY_EDITOR
        return (_libraryclass != null) ? _libraryclass.CallStatic<bool>(_setEngineByPackageName, packagename) : false;
        #else
        return false;
        #endif
    }

    public static void StopSpeaking() {
        #if UNITY_ANDROID && !UNITY_EDITOR
        if(_libraryclass != null) {
            _libraryclass.CallStatic(_stopspeaking);
        }
        #endif
    }
}
