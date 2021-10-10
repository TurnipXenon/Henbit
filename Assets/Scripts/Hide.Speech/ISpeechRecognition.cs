namespace Hide.Speech
{
    /// <summary>
    /// Serves as a common interface between different types of speech recognition
    /// </summary>
    public interface ISpeechRecognition
    {
        event HidePhraseRecognitionArgs.RecognizedDelegate OnPhraseRecognized;
        event HidePhraseRecognitionArgs.OnPreparedDelegate OnPrepared;
        bool IsUsable();
        void SetKeyword(string[] keywordList);
        void StartListening();
        void StopListening();
    }
}