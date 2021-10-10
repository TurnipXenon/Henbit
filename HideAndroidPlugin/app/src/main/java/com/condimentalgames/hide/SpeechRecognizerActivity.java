package com.condimentalgames.hide;

import android.Manifest;
import android.app.Activity;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import com.condimentalgames.hide.androidmodule.SpeechRecognizerFragment;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;

public class SpeechRecognizerActivity extends Activity implements SpeechRecognizerFragment.Listener{
    private static final String SPEECH_FRAGMENT_TAG = "SPEECH_FRAGMENT_TAG";
    private static final int RECORD_AUDIO_REQUEST_CODE = 100;
    private static final String LOG_TAG = "Android:Hide";

    private SpeechRecognizerFragment fragment;
    private final String[] keyWordList = new String[]{"moo", "oink", "cluck", "testing"};
    private final String[] equivalentList = new String[]{"Cow", "Pig", "Chicken", "Testing"};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_speech_recognizer);

        if (ContextCompat.checkSelfPermission(this, Manifest.permission.RECORD_AUDIO) == PackageManager.PERMISSION_GRANTED) {
            initializeSpeechRecognition();
        } else if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M && shouldShowRequestPermissionRationale("This app tests microphone")) {
            // show in context ui
        } else {
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                requestPermissions(new String[]{Manifest.permission.RECORD_AUDIO}, RECORD_AUDIO_REQUEST_CODE);
            }
        }
    }

    @Override
    protected void onDestroy() {
        fragment.destroySpeechRecognizer();
        super.onDestroy();
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        switch (requestCode) {
            case RECORD_AUDIO_REQUEST_CODE:
                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    initializeSpeechRecognition();
                } else {
                    Log.e(LOG_TAG, "Request for permission required to test audio.");
                    Toast.makeText(this, "Record audio sad", Toast.LENGTH_LONG).show();
                }
                break;
        }
    }

    @Override
    public void OnSpeechRecognized(SpeechRecognizerFragment.PhraseRecognizedEventArgs args) {
        // do something
        Toast.makeText(this, "Detected: " + args.text, Toast.LENGTH_LONG).show();
    }

    public void initializeSpeechRecognition() {
        fragment = SpeechRecognizerFragment.createFromAndroid(this, SPEECH_FRAGMENT_TAG);
        fragment.setKeywords(keyWordList);
        fragment.addJavaListener((SpeechRecognizerFragment.Listener) this);
        if (fragment.initializeSpeechRecognizer()) {
            fragment.startListening();
        } else {
            Toast.makeText(this, "Cannot listen", Toast.LENGTH_SHORT).show();
        }
    }
}