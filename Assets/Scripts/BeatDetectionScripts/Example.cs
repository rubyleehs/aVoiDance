using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    public GameObject testObj;
    void Start()
    {
        //Select the instance of AudioProcessor and pass a reference
        //to this object
        AudioProcessor processor = FindObjectOfType<AudioProcessor>();
        processor.onBeat.AddListener(onOnbeatDetected);
        processor.onSpectrum.AddListener(onSpectrum);
    }

    //this event will be called every time a beat is detected.
    //Change the threshold parameter in the inspector
    //to adjust the sensitivity
    void onOnbeatDetected()
    {
        Instantiate(testObj, new Vector3(Time.time * 0.2f, 0,0), Quaternion.identity);
    }

    //This event will be called every frame while music is playing
    void onSpectrum(float[] spectrum)
    {
        //The spectrum is logarithmically averaged
        //to 12 bands

        for (int i = 0; i < spectrum.Length; ++i)
        {
            Vector3 start = new Vector3(i *0.5f, 0, 0);
            Vector3 end = new Vector3(i*0.5f, spectrum[i] * 10, 0);
            Debug.DrawLine(start, end);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Instantiate(testObj, new Vector3(Time.time * 0.2f, -1, 0), Quaternion.identity);
        }
    }
}
