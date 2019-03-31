using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] float period = 5f;
    //todo remove from inspector later
    [Range(0, 1)] [SerializeField] float movementFactor;

    [SerializeField] Vector3 startingPos;
    [SerializeField] Vector3 endPos;
    Vector3 movementVector;

    void Start()
    {
        movementVector = endPos - startingPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) return;
        
        float cycles = Time.time / period;
        const float tau = Mathf.PI * 2f;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
