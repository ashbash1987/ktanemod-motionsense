using UnityEngine;

public class IndicatorBackground : MonoBehaviour
{
    private static readonly float WARNING_VALUE = 0.8f;

    public Material UnlitMaterial = null;
    public Material LitMaterial = null;
    public Material WarningMaterial = null;

    public float NormalisedValue = 0.0f;

    private MeshRenderer _meshRenderer = null;
    private bool _enabled = false;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (_enabled)
        {
            NormalisedValue = Mathf.Clamp(NormalisedValue, 0.0f, 1.0f);
            if (NormalisedValue >= WARNING_VALUE)
            {
                _meshRenderer.material = WarningMaterial;
            }
            else
            {
                _meshRenderer.material = LitMaterial;
            }
        }
    }

    public void EnableLight(bool enable)
    {
        _meshRenderer.material = enable ? LitMaterial : UnlitMaterial;
        _enabled = enable;
    }
}
