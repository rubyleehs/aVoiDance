using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectralFluxInfo
{
    public float time;
    public float spectralFlux;
    public float threshold;
    public float prunedSpectralFlux;
    public bool isPeak;
}

public class AudioTest : MonoBehaviour
{

    public AudioSource audioSource;
    [Range(5, 20)]
    public int thresholdWindowSize = 15;
    public float thresholdMultiplier;
    List<SpectralFluxInfo> spectralFluxSamples;

    private int numBinsPow2 = 10;
    private int numSamples;
    float[] curSpectrum;
    float[] prevSpectrum;

    // Use this for initialization
    void Start()
    {
        spectralFluxSamples = new List<SpectralFluxInfo>();
        numSamples = (int)Mathf.Pow(2, numBinsPow2);
        curSpectrum = new float[1024];
        prevSpectrum = new float[1024];
    }

    public void setCurSpectrum(float[] spectrum)
    {
        curSpectrum.CopyTo(prevSpectrum, 0);
        spectrum.CopyTo(curSpectrum, 0);
    }

    // Update is called once per frame
    /*
    void Update()
    {
        if (audioSource.time >= 128f && audioSource.time < 129f)
        {
            float[] curSpectrum = new float[1024];
            audioSource.GetSpectrumData(curSpectrum, 0, FFTWindow.BlackmanHarris);

            float targetFrequency = 234f;//use me to target
            float hertzPerBin = (float)AudioSettings.outputSampleRate / 2f / 1024;
            int targetIndex = (int)(targetFrequency / hertzPerBin);

            string outString = "";
            for (int i = targetIndex - 3; i <= targetIndex + 3; i++)
            {
                outString += string.Format("| Bin {0} : {1}Hz : {2} |   ", i, i * hertzPerBin, curSpectrum[i]);
            }

            Debug.Log(outString);
        }
    }
    */

    void Update()
    {
        curSpectrum = new float[1024];

        audioSource.GetSpectrumData(curSpectrum, 0, FFTWindow.BlackmanHarris);

        AnalyzeSpectrum(ref curSpectrum, audioSource.time);
    }


    float calculateRectifiedSpectralFlux()
    {
        float sum = 0f;

        // Aggregate positive changes in spectrum data
        for (int i = 0; i < numSamples; i++)
        {
            sum += Mathf.Max(0f, curSpectrum[i] - prevSpectrum[i]);
        }
        return sum;
    }

    float getFluxThreshold(int spectralFluxIndex)
    {
        // How many samples in the past and future we include in our average
        int windowStartIndex = Mathf.Max(0, spectralFluxIndex - thresholdWindowSize / 2);
        int windowEndIndex = Mathf.Min(spectralFluxSamples.Count - 1, spectralFluxIndex + thresholdWindowSize / 2);

        // Add up our spectral flux over the window
        float sum = 0f;
        for (int i = windowStartIndex; i < windowEndIndex; i++)
        {
            sum += spectralFluxSamples[i].spectralFlux;
        }

        // Return the average multiplied by our sensitivity multiplier
        float avg = sum / (windowEndIndex - windowStartIndex);
        return avg * thresholdMultiplier;
    }

    float GetPrunedSpectralFlux(int spectralFluxIndex)
    {
        return Mathf.Max(0f, spectralFluxSamples[spectralFluxIndex].spectralFlux - spectralFluxSamples[spectralFluxIndex].threshold);
    }

    bool IsPeak(int spectralFluxIndex)
    {
        Debug.Log(spectralFluxSamples.Count + " index: " + spectralFluxIndex);
        Debug.Log((spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux));
        if (spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex + 1].prunedSpectralFlux &&
            spectralFluxSamples[spectralFluxIndex].prunedSpectralFlux > spectralFluxSamples[spectralFluxIndex - 1].prunedSpectralFlux)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AnalyzeSpectrum(ref float[] spectrum, float time)
    {
        for (int indexToProcess = 0; indexToProcess < 1024; indexToProcess++)
        {
            // Set spectrum
            setCurSpectrum(spectrum);

            // Get current spectral flux from spectrum
            SpectralFluxInfo curInfo = new SpectralFluxInfo();
            curInfo.time = time;
            curInfo.spectralFlux = calculateRectifiedSpectralFlux();
            spectralFluxSamples.Add(curInfo);

            // We have enough samples to detect a peak
            if (spectralFluxSamples.Count >= thresholdWindowSize)
            {
                // Get Flux threshold of time window surrounding index to process
                spectralFluxSamples[indexToProcess].threshold = getFluxThreshold(indexToProcess);

                // Only keep amp amount above threshold to allow peak filtering
                spectralFluxSamples[indexToProcess].prunedSpectralFlux = GetPrunedSpectralFlux(indexToProcess);

                // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
                int indexToDetectPeak = indexToProcess - 1;

                bool curPeak = IsPeak(indexToDetectPeak);

                if (curPeak)
                {
                    spectralFluxSamples[indexToDetectPeak].isPeak = true;
                }
                
            }
            else
            {
                Debug.Log(string.Format("Not ready yet.  At spectral flux sample size of {0} growing to {1}", spectralFluxSamples.Count, thresholdWindowSize));
            }
        }
    }
}
