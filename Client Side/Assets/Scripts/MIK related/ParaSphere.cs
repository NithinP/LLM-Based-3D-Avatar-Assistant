using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParaSphere : MonoBehaviour
{
    public int _band;
    public float _startScale, _scaleMultiplier;
    public bool _useBuffer;

    private Transform targetTransform;
    private Vector3 startTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_useBuffer)
        {
            float bandBufferValue = AutoDetectVoice._bandBuffer[_band];

            transform.localScale = new Vector3(transform.localScale.x, (bandBufferValue * _scaleMultiplier) + _startScale, transform.localScale.z);
        }
        if (!_useBuffer)
        {
            transform.localScale = new Vector3(transform.localScale.x, (AutoDetectVoice._freqBand[_band] * _scaleMultiplier) + _startScale, transform.localScale.z);
        }
    }

}
