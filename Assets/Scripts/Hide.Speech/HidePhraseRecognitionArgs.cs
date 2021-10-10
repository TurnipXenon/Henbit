using UnityEngine;

namespace Hide.Speech
{
    /// <summary>
    /// Based on Windows.Speech.PhraseRecognizedEventArgs
    /// </summary>
    public class HidePhraseRecognitionArgs
    {
        // todo: Decide whether to include missing attributes in UnityEngine.Windows.Speech.PhraseRecognizedEventArgs
        public float confidenceLevel = 0;
        public string text = "";

        public void FromJsonOverwrite(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }
        
        /// <summary>
        ///   <para>Delegate for OnPhraseRecognized event. Based on Windows.Speech.PhraseRecognizer</para>
        /// </summary>
        /// <param name="args">Information about a phrase recognized event.</param>
        public delegate void RecognizedDelegate(HidePhraseRecognitionArgs args);

        public delegate void OnPreparedDelegate();
    }
}