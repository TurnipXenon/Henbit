using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ANDROID
using Hide.Speech.Android;
#endif // UNITY_ANDROID

namespace Hide.Speech
{
    /// <summary>
    /// Calls functions subscribed onSpeechRecognizedEvent based keywords set using SetKeyword
    /// </summary>
    /// <remarks>
    /// How to use:
    /// 1. In Editor: subscribe a function that will set the keywords on
    /// the UnityEvent onPreparedEvent.
    /// 2. In Editor: subscribe a function that will listen when the keywords are said
    /// on the UnityEvent onSpeechRecognized
    /// 3. In code / In Editor: find a way to get a reference to SpeechRecognizer in the function
    /// that will set the keyword and / or listen for the keyword activation
    /// 4. In code: call SetKeyword(keywords) to your SpeechRecognizer reference
    /// 5. In code: call StartListening after setting the keywords
    /// </remarks>
    public class SpeechRecognizer : MonoBehaviour
    {
        [Tooltip("Assign a function to this to know when you can put keywords and start listening.")]
        public UnityEvent onPreparedEvent;

        [Tooltip("Assign functions that takes a HidePhraseArgument as an argument.")]
        public OnSpeechRecognizedEvent onSpeechRecognizedEvent;

        [Header("Android-exclusive properties")]
        [Tooltip("Decides whether to enforce the following properties. It only works before OnPrepared.")]
        public bool enforceCustomProperties = false;

        [Tooltip("The minimum gap between every StartListening call since continuous Android " +
                 "listening is just repeated StartListening calls.")]
        public long androidPollingRate = 1000;

        [Tooltip("Always-on-Ok-Google phones would often cause a false NoMatchError so a" +
                 " tolerance to differentiate an actual error is needed.")]
        public long androidNoMatchErrorTolerance = 1000;

        [Tooltip("We are only using a Speech-to-Text feature so this variable limits how many" +
                 "distinguishable words are spoken before it stops listening.")]
        public int androidMaxResultCount = 3;

        private ISpeechRecognition _speechRecognition;

        private void Start()
        {
            _speechRecognition = SpeechRecognitionFactory.Create();
            _speechRecognition.OnPrepared += OnPrepared;
        }

        public void SetKeyword(string[] keywords)
        {
            if (_speechRecognition.IsUsable())
            {
                _speechRecognition.SetKeyword(keywords);
            }
        }

        public void StartListening()
        {
            if (_speechRecognition.IsUsable())
            {
                _speechRecognition.StartListening();
            }
        }

        public void StopListening()
        {
            if (_speechRecognition.IsUsable())
            {
                _speechRecognition.StopListening();
            }
        }

        private void OnPrepared()
        {
            if (_speechRecognition.IsUsable())
            {
#if UNITY_ANDROID
                if (enforceCustomProperties 
                    && _speechRecognition is HideAndroidSpeechRecognitionPlugin androidPlugin)
                {
                    androidPlugin.SetPollingRate(androidPollingRate);
                    androidPlugin.SetNoMatchErrorTolerance(androidNoMatchErrorTolerance);
                    androidPlugin.SetMaxResultCount(androidMaxResultCount);
                }
#endif // UNITY_ANDROID

                _speechRecognition.OnPhraseRecognized += OnSpeechRecognized;
                onPreparedEvent.Invoke();
            }
            else
            {
                Debug.LogWarning("Speech recognition not usable.");
            }
        }

        private void OnSpeechRecognized(HidePhraseRecognitionArgs args)
        {
            onSpeechRecognizedEvent.Invoke(args);
        }

        [Serializable]
        public class OnSpeechRecognizedEvent : UnityEvent<HidePhraseRecognitionArgs>
        {
            /* empty */
        }
    }
}