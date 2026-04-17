using System;
using System.Collections;
using TriInspector;
using UnityEngine;

public class AudioSourceHandler : MonoBehaviour
{
    private AudioSource source;
    private AudioTrigger trigger;
    private float initialVolume;

    [Unit("sec")] [SerializeField] private float fadeInDuration = 1f;
    [Unit("sec")] [SerializeField] private float fadeOutDuration = 1f;
    private Coroutine fadeRoutine;

    [SerializeField] private AudioClip clip;
    [SerializeField] private string playerTag = "Player";

    [SerializeField, LabelText("Play Only Once: ")] private bool playOnlyOnce = false;
    private bool hasPlayedOnce  = false;
    private int enterCounter = 0;

    private void Awake()
    {
        source = GetComponentInChildren<AudioSource>();
        source.clip = clip;
        initialVolume = source.volume;  // save initial volume
        source.volume = 0f;  // for fade in

        trigger = GetComponentInChildren<AudioTrigger>();
    }

    private void OnEnable()
    {
        if(trigger == null)
            return;

        trigger.TriggerEntered += OnTriggerEnter;
        trigger.TriggerExited += OnTriggerExit;
    }

    private void OnDisable()
    {
        if(trigger == null)
            return;

        trigger.TriggerEntered -= OnTriggerEnter;
        trigger.TriggerExited -= OnTriggerExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag) || (hasPlayedOnce && playOnlyOnce))
            return;

        enterCounter++;

        if (enterCounter == 1)
            Fade(true);  // fade in
    }

    private void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag(playerTag))
            return;

        if(enterCounter > 0)
            enterCounter--;

        if (enterCounter == 0)
        {
            Fade(false);  // fade out
        }
    }

    public void Fade(bool inOrOut)
    {
        if (fadeRoutine != null) {
            StopCoroutine(fadeRoutine);
        }

        if(inOrOut)
            fadeRoutine = StartCoroutine(FadeInCoroutine());  // fade in
        else
            fadeRoutine = StartCoroutine(FadeOutCoroutine());  // fade out
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, time / fadeOutDuration);
            yield return null;
        }

        source.volume = 0f;
        source.Stop();

        fadeRoutine = null;
    }

    private IEnumerator FadeInCoroutine()
    {
        float startVolume = source.volume;
        float time = 0f;

        if (!source.isPlaying)
            source.Play();

        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, initialVolume, time / fadeInDuration);
            yield return null;
        }
        hasPlayedOnce  = true;

        source.volume = initialVolume;

        fadeRoutine = null;
    }
}
