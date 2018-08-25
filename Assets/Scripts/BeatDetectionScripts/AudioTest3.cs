using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MuseInfo
{
    // https://www.teachmeaudio.com/mixing/techniques/audio-spectrum
    public static Vector2Int SubBass = new Vector2Int(20, 60);
    public static Vector2Int Bass = new Vector2Int(60, 250);
    public static Vector2Int LowMidRange = new Vector2Int(250, 500);
    public static Vector2Int MidRange = new Vector2Int(500, 2000);
    public static Vector2Int UpperMidRange = new Vector2Int(2000, 4000);
    public static Vector2Int Presence = new Vector2Int(4000, 6000);
    public static Vector2Int Brillance = new Vector2Int(6000, 20000);
}

public class AudioTest3 : MonoBehaviour
{
    public int samplesToTest;
    public GameObject beatGO;
    public GameObject pathGO;
    public AudioSource audioSourceToSample;
    public AudioSource audioSourceToPlay;
    public float detectionSensivity;
    public float minBinAverageRangeValue;
    public float minBinAverageRangeMultiplier;
    public float playDelay;
    public float lagDelay;
    public int minSampleBetweenBeats;
    public float minTimeBetweenBeats;
    public float beatCaptureTimeAllownce;
    public RunwayManager runwayManager;
    public UIManager uiManager;

    public static List<float[]> songSpectrum = new List<float[]>();
    public static float[] curSpectrum;
    private float hertzPerBin;


    private int noOfPaths = 2;
    public static int[] pathRunwayIndex;
    public List<Transform> paths = new List<Transform>();
    public List<PathManager> pathManagers = new List<PathManager>();
    public static int combo =0;

    public static List<float> samNum = new List<float>();
    // Use this for initialization

    private void Start()
    {
        pathRunwayIndex = new int[2] { 2, 5 };


        for (int i = 0; i < noOfPaths; i++)
        {
            float pathAngle = RunwayManager.runwayAngleSeperation * (i - 0.5f * (RunwayManager.runwayCount - 1));
            paths.Add(Instantiate(pathGO,this.transform.position,Quaternion.Euler(new Vector3(0,0,pathAngle))).transform);
            pathManagers.Add(paths[i].GetComponent<PathManager>());
            pathManagers[i].currentIndex = pathRunwayIndex[i];
            pathManagers[i].targetIndex = pathRunwayIndex[i];
            pathManagers[i].audioManager = this;
            runwayManager.paths.Add( paths[i].GetComponent<PathManager>()); 
        }
        StartCoroutine(InitiateSong());
    }


    void Update()
    {
        //Camera.main.transform.position = new Vector3(Time.time * 0.4f - 0.8f, 0.25f, -10);
        curSpectrum = new float[1024];
        audioSourceToSample.GetSpectrumData(curSpectrum, 0, FFTWindow.BlackmanHarris);
        songSpectrum.Add(curSpectrum);
        hertzPerBin = (float)AudioSettings.outputSampleRate / 2f / 1024;


        //BeatCheck(MuseInfo.SubBass, -0.5f,0);
        BeatCheck(MuseInfo.Bass, 0f, 0);
        BeatCheck(MuseInfo.LowMidRange, 0.5f, 1);
        BeatCheck(MuseInfo.MidRange, 1f, 1);//
        //BeatCheck(MuseInfo.UpperMidRange, 1.5f,1);

        CheckSamplesNum();

        if(Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                TryCaptureAnyBeat(0);
                return;
            }
            else if (Input.GetAxis("Horizontal") > 0) TryCaptureAnyBeat(1);
            else TryCaptureAnyBeat(-1);
        }
        /*
        for (int i = 0; i < noOfPaths - 1; i++)
        {
            PathManager pathManager = paths[i].GetComponent<PathManager>();
            float beatDeltaTime = Time.time - pathManager.beats[0] - playDelay;
            if (beatDeltaTime <0)
            {
                //miss beat
                //screenshake
            }
        }
        */
    }

    private void CheckSamplesNum()
    {
        samNum.Add(Time.time);
        while (Time.time - samNum[0] >= playDelay) samNum.RemoveAt(0);
    }


    void BeatCheck(Vector2Int frequencyRange, float h, int spawnPathIndex)
    {
        if(songSpectrum.Count > samplesToTest )
        {
            if (pathManagers[spawnPathIndex].beats != null && pathManagers[spawnPathIndex].beats.Count > 0)//
            {
                if (Time.time - pathManagers[spawnPathIndex].beats[pathManagers[spawnPathIndex].beats.Count - 1] < minTimeBetweenBeats) return;
            }
            Vector2Int targetIndexRange = new Vector2Int((int)(frequencyRange.x/hertzPerBin), (int)(frequencyRange.y/hertzPerBin));
            float netSumRangeAverage = 0;
            float curNetAverage = GetAverage(curSpectrum,0,curSpectrum.Length);
            float curRangeAverage = GetAverage(curSpectrum, targetIndexRange.x, targetIndexRange.y);

            for (int t = songSpectrum.Count -1; t >= songSpectrum.Count- samplesToTest; t--)
            {
                float tRangeAverage = GetAverage(songSpectrum[t], targetIndexRange.x, targetIndexRange.y);
                if (tRangeAverage > curRangeAverage) return;

                netSumRangeAverage += tRangeAverage;
            }
            netSumRangeAverage /= samplesToTest;

            if(curRangeAverage >= netSumRangeAverage * detectionSensivity && curRangeAverage >= curNetAverage * minBinAverageRangeMultiplier && curRangeAverage >= minBinAverageRangeValue)
            {
                pathManagers[spawnPathIndex].AddBeat();
                StartCoroutine(runwayManager.TrySpawnStar(spawnPathIndex));
                StartCoroutine(runwayManager.TrySpawnStar(spawnPathIndex));//
                //pathBeatsVisuals[spawnPathIndex].Add(Instantiate(beatGO,Vector3.zero,,);
            }
        }
    }
    public IEnumerator InitiateSong()
    {
        audioSourceToSample.Play();
        yield return new WaitForSeconds(playDelay);
        audioSourceToPlay.Play();
        yield return true;
    }

    public static float GetAverage(float[] _array, int startIndex, int endIndex)
    {
        if (_array == null) return 0;
        if (_array.Length <= endIndex + 1) return 0;

        float average = 0;
        for (int i = startIndex; i < endIndex; i++)
        {
            average += _array[i];
        }
        average /= endIndex - startIndex;//
        return average;
    }


    private void TryCaptureAnyBeat(int dir)
    {
        for (int i = 0; i < noOfPaths ; i++)
        {
            PathManager pathManager = paths[i].GetComponent<PathManager>();
            pathManager.TryCaptureBeat(dir);
        }
    }

    public PathManager IsActivatedPath(int index)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            PathManager _pathManager = paths[i].GetComponent<PathManager>();
            if (/*_pathManager.currentIndex == _pathManager.targetIndex &&*/ _pathManager.targetIndex == index) return paths[i].GetComponent<PathManager>();
        }
        return null;
    }

    public void AddToCombo()
    {
        combo++;
        uiManager.CheckCombo();
    }
}
