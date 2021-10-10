using UnityEngine;
#if UNITY_ANDROID
using Hide.Speech.Android;
#endif // UNITY_ANDROID
#if UNITY_STANDALONE_WIN
using Hide.Speech.Windows;

#endif // UNITY_STANDALONE_WIN

namespace Hide.Speech
{
    public static class SpeechRecognitionFactory
    {
        public static ISpeechRecognition Create()
        {
            var gameObject = new GameObject {name = "SpeechRecognitionCarrier"};

#if UNITY_ANDROID
            gameObject.AddComponent<HideAndroidSpeechRecognitionPlugin>();
            var speechRecognition = (ISpeechRecognition)gameObject
                .GetComponent<HideAndroidSpeechRecognitionPlugin>();
#elif UNITY_STANDALONE_WIN
            gameObject.AddComponent<WindowsSpeechRecognition>();
            var speechRecognition = (ISpeechRecognition) gameObject
                .GetComponent<WindowsSpeechRecognition>();
#else
            throw new NotImplementedException();
#endif // UNKNOWN

            return speechRecognition;
        }
    }
}