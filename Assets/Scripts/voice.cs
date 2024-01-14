using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;

public class voice : MonoBehaviour
{
    private KeywordRecognizer keywordRecognizer;
    private

Dictionary<string, Action> actions = new Dictionary<string, Action>();

    // The sensitivity of the voice treshold
    public float voiceTreshold = 0.5f;

    // The reference to the player transform
    private Transform playerTransform;

    // The direction of the player gaze
    private Vector3 lookDirection;

    private float speed = 1f;

    private void Start()
    {
        // Get the reference to the player's transform
        playerTransform = GameObject.Find("Player").transform;

        // Set up the actions for the keywords
        actions.Add("go", Go);
        actions.Add("up", Up);
        actions.Add("down", Down);
        actions.Add("back", Back);
        actions.Add("crash", Crash);

        // Create the keyword recognizer and initialize it
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeach;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeach(PhraseRecognizedEventArgs speach)
    {
        Debug.Log(speach.text);
        actions[speach.text].Invoke();

        // Update the look direction based on the player's rotation
        lookDirection = playerTransform.forward;
    }

    private void Up()
    {
        transform.Translate(0, 1, 0);
    }

    private void Go()
    {
        // Calculate the movement vector based on the look direction
        Vector3 movementVector = lookDirection * speed;

        // Move the GameObject
        transform.Translate(movementVector);
    }

    private void Down()
    {
        transform.Translate(0, -1, 0);
    }

    private void Back()
    {
        // Reverse the look direction
        lookDirection *= -1;

        // Apply the movement based on the reversed direction
        Go();
    }

    private void Crash()
    {
        transform.Translate(100, 100, 100);
    }
}