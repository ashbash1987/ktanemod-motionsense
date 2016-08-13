using System.Collections;
using UnityEngine;

public class MotionSenseModule : MonoBehaviour
{
    private static readonly float MAXIMUM_ROTATION_VALUE = 240.0f;
    private static readonly float FALLOFF_RATE = 20.0f;
    private static readonly float FAIL_GRACE_PERIOD = 2.0f;
    private static readonly float MAXIMUM_GRACE_PERIOD = 2.0f;

    public IndicatorBackground IndicatorBackground = null;
    public IndicatorBarGraph IndicatorBarGraph = null;
    public IndicatorNeedle IndicatorNeedle = null;
    public IndicatorAudio IndicatorAudio = null;

    public KMNeedyModule NeedyModule = null;

    private bool _active = false;
    private bool _failGrace = false;

    private Transform _rootXForm = null;
    private Quaternion _rootObjectLastOrientation = Quaternion.identity;
    private float _totalRotation = 0.0f;
    private float _maximumTotalRotation = 0.0f;
    private float _timeSinceMaximum = 0.0f;

    private Quaternion RootObjectOrientation
    {
        get
        {
            if (_rootXForm != null)
            {
                return _rootXForm.rotation;
            }

            return Quaternion.identity;
        }
    }

	private void Start()
    {
        _rootXForm = transform.root;

        NeedyModule.OnNeedyActivation += Activate;
        NeedyModule.OnNeedyDeactivation += Deactivate;
        NeedyModule.OnTimerExpired += TimeExpire;
    }	

    private void Update()
    {
        float totalRotation = Mathf.Max(0.0f, _totalRotation - (FALLOFF_RATE * Time.deltaTime));

        Quaternion currentOrientation = RootObjectOrientation;
        if (_active && !_failGrace)
        {
            totalRotation += Mathf.Abs(Quaternion.Angle(currentOrientation, _rootObjectLastOrientation));
        }

        _rootObjectLastOrientation = currentOrientation;

        _timeSinceMaximum += Time.deltaTime;
        SetTotalRotation(totalRotation, _timeSinceMaximum >= MAXIMUM_GRACE_PERIOD);
    }

    private void Activate()
    {
        _rootObjectLastOrientation = RootObjectOrientation;
        _active = true;
        _timeSinceMaximum = 0.0f;
        SetTotalRotation(0.0f, true);
        IndicatorBackground.EnableLight(true);
    }

    private void Deactivate()
    {
        _active = false;
        IndicatorBackground.EnableLight(false);
    }

    private void TimeExpire()
    {
        NeedyModule.HandlePass();
        Deactivate();
    }

    private void SetTotalRotation(float totalRotation, bool forceMaximum = false)
    {
        _totalRotation = totalRotation;
        if (!_failGrace && _totalRotation >= MAXIMUM_ROTATION_VALUE)
        {
            StartCoroutine(FailGracePeriod());
        }

        IndicatorBackground.NormalisedValue = IndicatorAudio.NormalisedValue = IndicatorNeedle.NormalisedValue = _totalRotation / MAXIMUM_ROTATION_VALUE;

        if (_totalRotation > _maximumTotalRotation)
        {
            _timeSinceMaximum = 0.0f;
            _maximumTotalRotation = _totalRotation;
            IndicatorBarGraph.NormalisedValue = IndicatorNeedle.NormalisedValue;
        }
        else if (forceMaximum)
        {
            _maximumTotalRotation = _totalRotation;
            IndicatorBarGraph.NormalisedValue = IndicatorNeedle.NormalisedValue;
        }
    }

    private IEnumerator FailGracePeriod()
    {
        NeedyModule.HandleStrike();
        _failGrace = true;
        _totalRotation = MAXIMUM_ROTATION_VALUE;
        yield return new WaitForSeconds(FAIL_GRACE_PERIOD);
        _failGrace = false;
        SetTotalRotation(_totalRotation, true);
    }
}
