using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;

public class voice : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    private void Start()
    {
        actions.Add("go", Go);
        actions.Add("up", Up);
        actions.Add("down", Down);
        actions.Add("back", Back);
        actions.Add("crash", Crash);

        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeach;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeach(PhraseRecognizedEventArgs speach)
    {
        Debug.Log(speach.text);
        actions[speach.text].Invoke();
    }

    private void Up()
    {
        transform.Translate(0,1,0);
    }
    private void Go()
    {
        transform.Translate(1,0,0);
    }
    private void Down()
    {
        transform.Translate(0,-1,0);
    }
    private void Back()
    {
        transform.Translate(-1,0,0);
    }
    private void Crash()
    {
        transform.Translate(100,100,100);
    }
}
