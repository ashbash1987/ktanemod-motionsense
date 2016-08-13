using UnityEngine;

public class IndicatorBarGraph : MonoBehaviour
{
    private static readonly float MINIMUM_ANGLE = -110.0f;
    private static readonly float MAXIMUM_ANGLE = 110.0f;

    public float NormalisedValue = 0.0f;

    private ArcMesh _arcMesh = null;

	private void Start()
    {
        _arcMesh = GetComponent<ArcMesh>();
    }
	
	private void Update()
    {
        NormalisedValue = Mathf.Clamp(NormalisedValue, 0.0f, 1.0f);

        _arcMesh.AngleStart = MINIMUM_ANGLE;
        _arcMesh.AngleEnd = Mathf.Lerp(MINIMUM_ANGLE, MAXIMUM_ANGLE, NormalisedValue) + 0.001f;

        _arcMesh.UStart = 0.0f;
        _arcMesh.UEnd = NormalisedValue;

        _arcMesh.RebuildMesh();
    }
}
