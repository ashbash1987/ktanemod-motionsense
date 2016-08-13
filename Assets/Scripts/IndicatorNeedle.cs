using UnityEngine;

public class IndicatorNeedle : MonoBehaviour
{
    private static readonly float MINIMUM_ANGLE = 0.0f;
    private static readonly float MAXIMUM_ANGLE = 220.0f;

    public float NormalisedValue = 0.0f;

    private void Update()
    {
        NormalisedValue = Mathf.Clamp(NormalisedValue, 0.0f, 1.0f);
        transform.localRotation = Quaternion.Euler(0.0f, Mathf.Lerp(MINIMUM_ANGLE, MAXIMUM_ANGLE, NormalisedValue), 0.0f);
    }
}
