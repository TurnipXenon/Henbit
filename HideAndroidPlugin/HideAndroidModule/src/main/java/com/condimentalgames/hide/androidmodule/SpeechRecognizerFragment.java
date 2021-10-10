package com.condimentalgames.hide.androidmodule;

import android.app.Activity;
import android.app.Fragment;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.speech.RecognitionListener;
import android.speech.RecognizerIntent;
import android.speech.SpeechRecognizer;
import android.util.Log;
import android.widget.Toast;

import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;
import java.util.Timer;
import java.util.TimerTask;

/**
 * - Sub classes are based on UnityEngine.Windows.Speech to match its functionalities.
 * - Handle permissions in Unity.
 *
 * Steps on how to use:
 * 1. Create from Unity
 * 2. Set keywords
 * 3. Check / Ask permission
 * 4. Check for speech recognition
 * 5. Add Unity listeners
 * 6. Add initializeSpeechRecognition
 * 7. Start listening
 */
// todo: improve documentation
public class SpeechRecognizerFragment extends Fragment implements RecognitionListener {
    private static final String TAG = "Unity:Hide";
    private static final String UNITY_FRAGMENT_TAG = "UNITY_SPEECH_RECOGNITION_FRAGMENT";

    private List<Listener> listeners = new ArrayList<>();
    private String[] keyWordList = new String[0];
    private int maxResultCount = 3;
    private long pollingRate = 1000; // measured in millisecond
    private long noMatchErrorTolerance = 500;

    private SpeechRecognizer speechRecognizer;
    private Intent recognizerIntent;
    private Activity activity;
    private float[] scores;
    private ArrayList<String> matches;
    private long recognitionStartTime = 0;

    // region Events
    @Override
    public void onDestroy() {
        destroySpeechRecognizer();
        super.onDestroy();
    }

    @Override
    public void onReadyForSpeech(Bundle params) {
    }

    @Override
    public void onBeginningOfSpeech() {
    }

    @Override
    public void onRmsChanged(float rmsdB) {
    }

    @Override
    public void onBufferReceived(byte[] buffer) {
    }

    @Override
    public void onEndOfSpeech() {
    }

    @Override
    public void onError(int error) {
        switch (error) {
            case SpeechRecognizer.ERROR_NO_MATCH:
                long elapsedTime = System.currentTimeMillis() - recognitionStartTime;
                if (elapsedTime < noMatchErrorTolerance || elapsedTime > pollingRate) {
                    startListening();
                    return;
                }
                break;
        }
        Log.e(TAG, "onError: " + error);
    }

    @Override
    public void onResults(Bundle results) {
        matches = results.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
        scores = results.getFloatArray(SpeechRecognizer.CONFIDENCE_SCORES);
        if (matches != null) {
            String bestMatch = "";
            float bestScore = -1f;

            for (int i = 0; i < matches.size(); i++) {
                for (String keyWord :
                        keyWordList) {
                    if (matches.get(i).toLowerCase().contains(keyWord) && scores[i]>bestScore) {
                        bestMatch = keyWord;
                        break;
                    }
                }
            }

            if (!bestMatch.isEmpty()) {
                PhraseRecognizedEventArgs args = new PhraseRecognizedEventArgs();
                args.text = bestMatch;
                args.confidenceLevel = bestScore;
                onSpeechRecognized(args);
            }
        }

        final long difference =  recognitionStartTime - (System.currentTimeMillis() + pollingRate);
        if (difference <= 0) {
            startListening();
        } else {
            new Timer().schedule(new TimerTask() {
                @Override
                public void run() {
                    startListening();
                }
            }, difference);
        }
    }

    @Override
    public void onPartialResults(Bundle partialResults) {
    }

    @Override
    public void onEvent(int eventType, Bundle params) {
    }
    // endregion Events
    public static SpeechRecognizerFragment createFromUnity() {
        return createFromAndroid(UnityPlayer.currentActivity, UNITY_FRAGMENT_TAG);
    }

    public static SpeechRecognizerFragment createFromAndroid(Activity activity, String fragmentTag) {
        SpeechRecognizerFragment fragment = new SpeechRecognizerFragment();
        fragment.activity = activity;
        activity.getFragmentManager().beginTransaction().add((Fragment)fragment, fragmentTag).commit();
        Log.i(TAG, "createFromUnity: Success");
        return fragment;
    }

    public void setKeywords(String[] keyWordList) {
        this.keyWordList = keyWordList;
        for (int i = 0; i < this.keyWordList.length; i++){
            this.keyWordList[i] = this.keyWordList[i].toLowerCase();
            Log.i(TAG, "setKeywords: new: " + this.keyWordList[i]);
        }
    }

    public void setPollingRate(long milliseconds) {
        this.pollingRate = milliseconds;
    }

    public void setNoMatchErrorTolerance(long milliseconds) {
        this.noMatchErrorTolerance = milliseconds;
    }

    public void setMaxResultCount(int maxResultCount) {
        this.maxResultCount = maxResultCount;
    }

    public void addJavaListener(Listener listener) {
        this.listeners.add(listener);
    }

    public void addUnityListener(final String objectName, final String functionName) {
        UnityListener unityListener = new UnityListener();
        unityListener.objectName = objectName;
        unityListener.functionName = functionName;
        PhraseRecognizedEventArgs args = new PhraseRecognizedEventArgs();
        this.listeners.add(unityListener);
    }

    public boolean isSpeechRecognitionAvailable() {
        return SpeechRecognizer.isRecognitionAvailable(activity);
    }

    public boolean initializeSpeechRecognizer() {
        if (!SpeechRecognizer.isRecognitionAvailable(activity)) {
            Log.e(TAG, "Speech recognition not available");
            Toast.makeText(activity, "Speech recognition not available", Toast.LENGTH_LONG).show();
            return false;
        }

        // WARNING: trust Unity to do its job
        boolean granted = true; // activity.checkSelfPermission(activity, Manifest.permission.RECORD_AUDIO) == PackageManager.PERMISSION_GRANTED;
        if (granted) {
            // reference: https://stackoverflow.com/a/11125271
            Handler mainHandler = new Handler(activity.getMainLooper());

            Runnable mainRunnable = new Runnable() {
                @Override
                public void run() {
                    speechRecognizer = SpeechRecognizer.createSpeechRecognizer(activity);
                    speechRecognizer.setRecognitionListener(SpeechRecognizerFragment.this);

                    recognizerIntent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
                    recognizerIntent.putExtra(RecognizerIntent.EXTRA_LANGUAGE_MODEL, RecognizerIntent.LANGUAGE_MODEL_FREE_FORM);
                    recognizerIntent.putExtra(RecognizerIntent.EXTRA_MAX_RESULTS, 3);
                }
            };

            mainHandler.post(mainRunnable);
        } else {
            Log.e(TAG, "Permission to record audio not yet granted. Ensure that permission was granted in Unity");
            Toast.makeText(activity, "Record audio not permitted", Toast.LENGTH_LONG).show();
        }
        return granted;
    }

    public void startListening() {
        recognitionStartTime = System.currentTimeMillis();

        Handler mainHandler = new Handler(activity.getMainLooper());
        Runnable mainRunnable = new Runnable(){
            @Override
            public void run() {
                speechRecognizer.startListening(recognizerIntent);
            }
        };
        mainHandler.post(mainRunnable);
    }

    /**
     * Call this onDisable for the MonoBehavior owning the class
     */
    public void stopListening() {
        Handler mainHandler = new Handler(activity.getMainLooper());
        Runnable mainRunnable = new Runnable(){
            @Override
            public void run() {
                if (speechRecognizer != null) {
                    speechRecognizer.stopListening();
                }
            }
        };
        mainHandler.post(mainRunnable);
    }

    /**
     * Call this onDisable for the MonoBehavior owning the class
     */
    public void destroySpeechRecognizer() {
        Handler mainHandler = new Handler(activity.getMainLooper());
        Runnable mainRunnable = new Runnable(){
            @Override
            public void run() {
                if (speechRecognizer != null) {
                    speechRecognizer.stopListening();
                    speechRecognizer.destroy();
                }
            }
        };
        mainHandler.post(mainRunnable);
    }

    public void forceDestroy() {
        destroySpeechRecognizer();
        activity.getFragmentManager().beginTransaction().remove(this).commit();
    }

    private void onSpeechRecognized(PhraseRecognizedEventArgs args) {
        for (Listener listener :
                listeners) {
            listener.OnSpeechRecognized(args);
        }
    }

    public interface Listener {
        public void OnSpeechRecognized(PhraseRecognizedEventArgs args);
    }

    public class SemanticMeaning {
        public String key = "";
        public String[] values = new String[0];
    }

    // todo: support other arguments
    public class PhraseRecognizedEventArgs {
        public float confidenceLevel = 0;
        public SemanticMeaning[] semanticMeanings = new SemanticMeaning[0];
        public String text = "";
        public long phraseStartTime = 0;
        public long phraseDuration = 0;

        public String toJSON() {
            JSONObject jsonObject = new JSONObject();
            try {
                jsonObject.put("confidenceLevel", confidenceLevel);
                jsonObject.put("text", text);
                return jsonObject.toString();
            } catch (JSONException e) {
                e.printStackTrace();
                return "";
            }
        }
    }

    public class UnityListener implements Listener {
        public String objectName;
        public String functionName;

        @Override
        public void OnSpeechRecognized(PhraseRecognizedEventArgs args) {
            UnityPlayer.UnitySendMessage(objectName, functionName, args.toJSON());
        }
    }
}