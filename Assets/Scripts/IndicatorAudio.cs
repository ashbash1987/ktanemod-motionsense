using System.Collections;
using UnityEngine;

public class IndicatorAudio : MonoBehaviour
{
    private static readonly float WARNING_VALUE = 0.8f;
    private static readonly float AUDIO_GRACE_PERIOD = 1.0f;

    public float NormalisedValue = 0.0f;

    private KMAudio _audio = null;
    private float _oldNormalisedValue = 0.0f;
    private bool _grace = false;

    private void Start()
    {
        _audio = GetComponent<KMAudio>();
    }

    private void Update()
    {
        NormalisedValue = Mathf.Clamp(NormalisedValue, 0.0f, 1.0f);

        if (NormalisedValue > _oldNormalisedValue && NormalisedValue >= WARNING_VALUE && !_grace)
        {
            StartCoroutine(AudioPlayback());
        }

        _oldNormalisedValue = NormalisedValue;
    }

    private IEnumerator AudioPlayback()
    {
        _grace = true;
        _audio.PlaySoundAtTransform("warning", transform);

        yield return new WaitForSeconds(AUDIO_GRACE_PERIOD);

        _grace = false;
    }
}
