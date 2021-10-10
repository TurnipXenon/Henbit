#if UNITY_STANDALONE_WIN
using System;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace Hide.Speech.Windows
{
    /// <summary>Handles the direct interaction between Unity and Windows</summary>
    /// <remarks>Never attach on a GameObject on Editor. Rely on SpeechRecognizer Component.</remarks>
    public class WindowsSpeechRecognition : MonoBehaviour, ISpeechRecognition
    {
        public event HidePhraseRecognitionArgs.RecognizedDelegate OnPhraseRecognized;
        public event HidePhraseRecognitionArgs.OnPreparedDelegate OnPrepared;

        private KeywordRecognizer _keywordRecognizer;

        private void Start()
        {
            var onPrepared = OnPrepared;
            onPrepared?.Invoke();
        }
        
        private void Reset()
        {
            if (GetType() == typeof(WindowsSpeechRecognition))
            {
                DestroyImmediate( this );
            }
        }

        public bool IsUsable()
        {
            return true; // this is always usable since no set up is needed
        }

        public void SetKeyword(string[] keywordList)
        {
            _keywordRecognizer = new KeywordRecognizer(keywordList);
            _keywordRecognizer.OnPhraseRecognized += SpeechRecognized;
        }

        public void StartListening()
        {
            _keywordRecognizer.Start();
        }

        public void StopListening()
        {
            _keywordRecognizer.Stop();
        }

        private void SpeechRecognized(PhraseRecognizedEventArgs args)
        {
            var phraseRecognized = OnPhraseRecognized;
            if (phraseRecognized == null) return;

            var translatedArgs = new HidePhraseRecognitionArgs();
            
            switch (args.confidence)
            {
                case ConfidenceLevel.High:
                    translatedArgs.confidenceLevel = 3 / 4f;
                    break;
                case ConfidenceLevel.Medium:
                    translatedArgs.confidenceLevel = 1 / 2f;
                    break;
                case ConfidenceLevel.Low:
                    translatedArgs.confidenceLevel = 1 / 4f;
                    break;
                case ConfidenceLevel.Rejected:
                    translatedArgs.confidenceLevel = 0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            translatedArgs.text = args.text;
            
            phraseRecognized(translatedArgs);
        }
    }
}
#endif // UNITY_STANDALONE_WIN