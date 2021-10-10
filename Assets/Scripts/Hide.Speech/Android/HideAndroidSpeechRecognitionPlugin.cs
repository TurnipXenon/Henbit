#if UNITY_ANDROID
using UnityEngine;
using Hide.Android;
using UnityEngine.Android;

namespace Hide.Speech.Android
{
    /// <summary>Handles the direct interaction between Unity and Android</summary>
    /// <remarks>Never attach on a GameObject on Editor. Rely on SpeechRecognizer Component.</remarks>
    public class HideAndroidSpeechRecognitionPlugin : MonoBehaviour, ISpeechRecognition
    {
        #region Constants

        private const string PluginName = "com.condimentalgames.hide.androidmodule.SpeechRecognizerFragment";

        // Android functions
        private const string FuncCreateFromUnity = "createFromUnity";
        private const string FuncSetKeywords = "setKeywords";
        private const string FuncSetPollingRate = "setPollingRate";
        private const string FuncSetNoMatchErrorTolerance = "setNoMatchErrorTolerance";
        private const string FuncSetMaxResultCount = "setMaxResultCount";
        private const string FuncIsSpeechRecognitionAvailable = "isSpeechRecognitionAvailable";
        private const string FuncInitializeSpeechRecognizer = "initializeSpeechRecognizer";
        private const string FuncStartListening = "startListening";
        private const string FuncDestroySpeechRecognizer = "destroySpeechRecognizer";
        private const string FuncAddUnityListener = "addUnityListener";
        private const string FuncStopListening = "stopListening";
        private const string FuncForceDestroy = "forceDestroy";

        // Unity functions
        private const string FuncOnSpeechRecognized = "OnSpeechRecognizedAndroid";

        #endregion Constants

        #region Fields

        public event HidePhraseRecognitionArgs.RecognizedDelegate OnPhraseRecognized;
        public event HidePhraseRecognitionArgs.OnPreparedDelegate OnPrepared;

        private AndroidJavaObject _joPlugin;
        private GameObject _dialog;
        private bool _isInitialized = false;

        #endregion Fields

        #region Events

        private void Start()
        {
            IsReady = false;
            
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                Permission.RequestUserPermission(Permission.Microphone);
                _dialog = new GameObject();
            }
        }

        // todo: Remove reliance on OnGUI which is expensive
        private void OnGUI()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        private void OnDestroy()
        {
            _joPlugin.Call(FuncForceDestroy);
        }

        private void Reset()
        {
            if (GetType() == typeof(HideAndroidSpeechRecognitionPlugin))
            {
                DestroyImmediate( this );
            }
        }

        /// <summary>
        /// This function is called by an Android event.
        /// DO NOT COPY THE NAME. THE REFLECTION SYSTEM WILL BREAK.
        /// DO NOT MAKE PRIVATE. THE REFLECTION SYSTEM WILL BREAK.
        /// </summary>
        /// <param name="jsonArgs">JSON string</param>
        public void OnSpeechRecognizedAndroid(string jsonArgs)
        {
            var recognized = OnPhraseRecognized;

            if (recognized == null)
            {
                return;
            }

            var args = new HidePhraseRecognitionArgs();
            args.FromJsonOverwrite(jsonArgs);
            recognized(args);
        }

        #endregion Events

        #region Properties

        public bool IsPluginAvailable => _joPlugin != null;
        public bool HasPermissionToListen { private set; get; }
        public bool HasSpeechRecognition { private set; get; }
        public bool IsReady { private set; get; }

        #endregion Properties

        #region Methods

        public bool IsUsable()
        {
            return IsReady && IsPluginAvailable && HasPermissionToListen && HasSpeechRecognition;
        }

        public void SetKeyword(string[] keywordList)
        {
            Debug.Assert(_joPlugin != null, 
                "_joPlugin != null: Use OnPrepared to ensure the plugin is ready");
            _joPlugin.Call(FuncSetKeywords, AndroidCSUtility.JavaArrayFromCS(keywordList));
        }

        public void StartListening()
        {
            DebugIsUsableAssertion();
            _joPlugin.Call(FuncStartListening);
        }

        public void StopListening()
        {
            DebugIsUsableAssertion();
            _joPlugin.Call(FuncStopListening);
        }

        public void SetPollingRate(long milliseconds)
        {
            DebugIsUsableAssertion();
            _joPlugin.Call(FuncSetPollingRate, milliseconds);
        }

        public void SetNoMatchErrorTolerance(long milliseconds)
        {
            DebugIsUsableAssertion();
            _joPlugin.Call(FuncSetNoMatchErrorTolerance, milliseconds);
        }

        public void SetMaxResultCount(int count)
        {
            DebugIsUsableAssertion();
            _joPlugin.Call(FuncSetMaxResultCount, count);
        }

        /// <summary>
        /// OnDestroy() for this object destroys the plugin and the speech recognizer with it anyway.
        /// </summary>
        public void DestroySpeechRecognizer()
        {
            DebugIsUsableAssertion();
            _joPlugin.Call(FuncDestroySpeechRecognizer);
        }
        
        private void Initialize()
        {
            HasPermissionToListen = Permission.HasUserAuthorizedPermission(Permission.Microphone);
            if (!HasPermissionToListen)
            {
                _dialog.AddComponent<HASRPPermissionRationaleDialog>();
                return;
            }

            if (_dialog != null)
            {
                Destroy(_dialog);
            }

            using (var jc = new AndroidJavaClass(PluginName))
            {
                _joPlugin = jc.CallStatic<AndroidJavaObject>(FuncCreateFromUnity);
            }

            if (IsPluginAvailable)
            {
                HasSpeechRecognition = _joPlugin.Call<bool>(FuncIsSpeechRecognitionAvailable);
            }
            
            if (HasSpeechRecognition)
            {
                _joPlugin.Call(FuncAddUnityListener, name, FuncOnSpeechRecognized);
                IsReady = _joPlugin.Call<bool>(FuncInitializeSpeechRecognizer);
            }
            
            _isInitialized = true;
            
            var prepared = OnPrepared;
            prepared?.Invoke(); // == if(prepared != null) prepared.Invoke();
        }

        private void DebugIsUsableAssertion()
        {
            Debug.Assert(IsUsable(),
                $"IsUsable: Either IsReady ({IsReady}), IsPluginAvailable " +
                $"({IsPluginAvailable}), HasPermissionToListen ({HasPermissionToListen}), " +
                $"HasSpeechRecognition ({HasSpeechRecognition}) is false");
        }

        #endregion Methods
    }
}
#endif