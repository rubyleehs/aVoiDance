using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AudioTest2 : MonoBehaviour {
    public AudioSource audioSource;
    static int numSamples = 1024;
    static int thresholdWindowSize = 15;
    static float thresholdMultiplier = 1.2f;
    float[] curSpectrum = new float[numSamples];
    float[] prevSpectrum = new float[numSamples];
    int indexToProcess = thresholdWindowSize;
    List<SpectralFluxInfo> spectralFluxSamples = new List<SpectralFluxInfo>();

    void Update()
    {
        audioSource.GetSpectrumData(curSpectrum, 0, FFTWindow.BlackmanHarris);
        AnalyzeSpectrum(curSpectrum, audioSource.time);
    }

    public void setCurSpectrum(float[] spectrum)
    {
        curSpectrum.CopyTo(prevSpectrum, 0);
        spectrum.CopyTo(curSpectrum, 0);
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

    float getPrunedSpectralFlux(int spectralFluxIndex)
    {
        return Mathf.Max(0f, spectralFluxSamples[spectralFluxIndex].spectralFlux - spectralFluxSamples[spectralFluxIndex].threshold);
    }

    bool isPeak(int spectralFluxIndex)
    {
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

    public void AnalyzeSpectrum(float[] spectrum, float time)
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
                spectralFluxSamples[indexToProcess].prunedSpectralFlux = getPrunedSpectralFlux(indexToProcess);

                // Now that we are processed at n, n-1 has neighbors (n-2, n) to determine peak
                int indexToDetectPeak = indexToProcess - 1;

                bool curPeak = isPeak(indexToDetectPeak);

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
