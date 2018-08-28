using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarInfo
{
    public float spawnTime;
    public int indexNum;
}

public class RunwayManager : MonoBehaviour
{
    public Transform origin;
    public LineRenderer aura;
    public static float originRadius = 5.5f;
    public static float minPathLength =18;
    //public float maxBottomAllowanceSpace;
    public int IrunwayCount = 8;
    public static int runwayCount;
    public bool CompactRunway;
    public GameObject runwayLinesGO;
    public GameObject starGO;
    public GameObject ring;
    public float ringSpawnPeriod;
    public static float contractionPeriod = 2;

    public float runwayLengthBeatMax;
    public float auraComboMultiplier;
    public float auraStartRatio;

    private Camera cam;
    private Vector2 camSize;
    public AudioTest3 audioManager;
    public static List<Transform> runways;

    private List<Transform> starsVisuals;
    private List<StarInfo> stars;

    public List<PathManager> paths = new List<PathManager>();

    public static float maxRunwayAngle;
    public static float runwayAngleSeperation;
    public float lastRingSpawn;

    private float largestrunwayVisualBeatSpectrumAverage = 0;


    private void Awake()
    {
        cam = Camera.main;
        runwayCount = IrunwayCount;
        runways = new List<Transform>();
        maxRunwayAngle = Mathf.Rad2Deg * Mathf.Asin(Mathf.Min(1, camSize.x / (2 * (originRadius + minPathLength))));
        runwayAngleSeperation = -2 * maxRunwayAngle / (runwayCount - 1 + System.Convert.ToInt32(CompactRunway));
        stars = new List<StarInfo>();
        starsVisuals = new List<Transform>();
        aura.positionCount = 3;
        aura.SetPositions(new Vector3[3] { Vector3.up * originRadius, Vector3.zero, -Vector3.up * originRadius });
        aura.startWidth = 2 * originRadius;


        if (AudioTest3.AutoPlay)
        {
            maxRunwayAngle = 90f;
            runwayAngleSeperation = 360 / 8;
            origin.localScale = Vector3.one * originRadius * 2f;
        }
        else UpdateCamera();
        
        for (int i = 0; i < runwayCount; i++)
        {
            CreateRunway(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!AudioTest3.AutoPlay)UpdateCamera();
        UpdateRunway();
        if (Time.time - lastRingSpawn >= ringSpawnPeriod) SpawnRing();
        //TrySpawnStar();
        StarsLogic();
        AuraLogic();
    }

    void AuraLogic()
    {
        Vector3 _linePos = Vector3.up * (auraStartRatio + AudioTest3.combo * auraComboMultiplier);
        aura.SetPositions(new Vector3[3] { _linePos, Vector3.zero, -_linePos });
        aura.startWidth = 4 * originRadius * (auraStartRatio + AudioTest3.combo * auraComboMultiplier);//
    }

    void StarsLogic()
    {
        for (int i = 0; i < stars.Count; i++)
        {
            float progress = (Time.time - stars[i].spawnTime) / audioManager.playDelay;
            starsVisuals[i].localPosition = Vector3.Lerp(new Vector3(0, 0.5f + 0.5f * minPathLength / originRadius), new Vector3(0, 0.5f, 0), progress * progress);
            if (progress >= 1)
            {
                PathManager _pathManager = audioManager.IsActivatedPath(stars[i].indexNum);
                if (_pathManager != null)
                {
                    AudioTest3.combo = 0;
                    _pathManager.FailEffect();
                    Debug.Log("You got hit!");
                }
                Destroy(starsVisuals[i].gameObject);
                starsVisuals.RemoveAt(i);
                stars.RemoveAt(i);
                i--;
            }
        }
    }

    void UpdateCamera()
    {
        float widthRadiusRatio = camSize.x / originRadius;
        camSize = 2f * new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);
        origin.localScale = Vector3.one * originRadius * 2f;
        if (2 * originRadius < camSize.x)
        {
            //Debug.Log("originRadius Value Is Too Small! Setting to 0.5f * camSize!");
            //originRadius = 0.5f * camSize.x;
            cam.transform.parent.position = new Vector3(0, 0.5f* camSize.y, -10);
            return;
        }
        //float bottomSpaceAllowance = Mathf.Min(maxBottomAllowanceSpace, camSize.y - sagittaLength - minPathLength); 
        float camY = originRadius * (Mathf.Sqrt(1 - Mathf.Pow(0.5f * widthRadiusRatio, 2))) + 0.5f * camSize.y; //- bottomSpaceAllowance;
        cam.transform.parent.position = new Vector3(0, camY, -10);
    }

    void UpdateRunway()
    {
        largestrunwayVisualBeatSpectrumAverage = 0;
        maxRunwayAngle = Mathf.Rad2Deg * Mathf.Asin(Mathf.Min(1,camSize.x / (2 * (originRadius + minPathLength))));
        if (AudioTest3.AutoPlay) maxRunwayAngle = 180;
        runwayAngleSeperation = -2 * maxRunwayAngle / (runwayCount -1 + System.Convert.ToInt32(CompactRunway));
        int weightedRunwayVisualBeatSpectrumRange = 2 * 500 / ((runwayCount) *(runwayCount + 1));
        for (int n = 0; n < runways.Count; n++)
        {
            int _delayedSamplesIndex = AudioTest3.songSpectrum.Count - AudioTest3.samNum.Count;
            float runwayVisualBeatSpectrumAverage = 0;
            if (_delayedSamplesIndex > 0)
            {
                runwayVisualBeatSpectrumAverage = AudioTest3.GetAverage(AudioTest3.songSpectrum[AudioTest3.songSpectrum.Count - AudioTest3.samNum.Count], 20 + n * (n + 1) / 2 * weightedRunwayVisualBeatSpectrumRange, 20 + (n + 1) * (n + 2) / 2 * weightedRunwayVisualBeatSpectrumRange);
                if (runwayVisualBeatSpectrumAverage < 1) runwayVisualBeatSpectrumAverage = Mathf.Pow(runwayVisualBeatSpectrumAverage + 1, 2) - 1;
                largestrunwayVisualBeatSpectrumAverage = Mathf.Max(largestrunwayVisualBeatSpectrumAverage, runwayVisualBeatSpectrumAverage);
                if(largestrunwayVisualBeatSpectrumAverage != runwayVisualBeatSpectrumAverage)
                {
                    largestrunwayVisualBeatSpectrumAverage *= 0.55f;//
                    runwayVisualBeatSpectrumAverage = largestrunwayVisualBeatSpectrumAverage;
                }
            }
                //float runwayVisualBeatSpectrumAverage = AudioTest3.GetAverage(AudioTest3.curSpectrum, 20 + n * (n + 1) / 2 * weightedRunwayVisualBeatSpectrumRange, 20 + (n + 1) * (n + 2) / 2 * weightedRunwayVisualBeatSpectrumRange);
            
            float runwayRotAng =  runwayAngleSeperation * (n - 0.5f * (runways.Count -1));
            runways[n].GetComponent<LineRenderer>().SetPosition(1, new Vector3(0, 0.5f + 0.5f* minPathLength / originRadius - 0.5f * runwayLengthBeatMax + Mathf.SmoothStep(0,runwayLengthBeatMax, 0.5f + runwayVisualBeatSpectrumAverage)));//
            runways[n].rotation = Quaternion.Euler(new Vector3(0, 0, runwayRotAng));
        }
    }

    void CreateRunway(int runwayIndex)
    {
        LineRenderer line = Instantiate(runwayLinesGO, origin).transform.GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(0, 0.5f, 0));
        line.SetPosition(1, new Vector3(0, 0.5f + 0.7f* minPathLength / originRadius, 0));

        List<Transform> cal_runways = new List<Transform>();
        runwayIndex = Mathf.Min(runwayIndex, runwayCount);
        for (int i = 0; i < runwayIndex; i++)
        {
            cal_runways.Add(runways[i]);
        }
        cal_runways.Add(line.transform);
        for (int i = runwayIndex ; i < runways.Count; i++)
        {
            cal_runways.Add(runways[i]);
        }

        runways.Clear();
        runways.AddRange(cal_runways);
    }

    void SpawnRing()
    {
        lastRingSpawn = Time.time;
        RingLines ringLines = Instantiate(ring).transform.GetComponent<RingLines>();
        ringLines.origin = origin;
        ringLines.runwayManager = this;
    }

    public IEnumerator TrySpawnStar(int assignedBeatPathIndex)
    {
        float chance = Random.Range(0, 100);
        if (chance > GlobalData.modeStarSpawn[GlobalData.mode%GlobalData.modeText.Length]) yield break;
        float randDeltaTime = 0.5f * audioManager.minTimeBetweenBeats + Random.Range(-0.35f * audioManager.minTimeBetweenBeats, 0.35f * audioManager.minTimeBetweenBeats);

        yield return new WaitForSeconds(randDeltaTime);
        int randRunwayIndex = Random.Range(0, runwayCount);
        if(paths[0].currentIndex != paths[1].currentIndex && randRunwayIndex == paths[(assignedBeatPathIndex + 1) % 2].currentIndex)
        {
            randRunwayIndex = assignedBeatPathIndex;
        }
        Transform star = Instantiate(starGO, Vector3.zero, Quaternion.identity, runways[randRunwayIndex]).transform;
        star.localScale = star.localScale/origin.localScale.x;
        star.localRotation = Quaternion.identity;
        star.localPosition = new Vector3(0, 0.5f + 0.5f * minPathLength / originRadius);
        starsVisuals.Add(star);
        stars.Add(new StarInfo { spawnTime = Time.time, indexNum = randRunwayIndex});//spawning is done. left movement/delettion/detection
        /*
        bool runwayIsActive = false;

        for (int i = 0; i < audioManager.paths.Count; i++)
        {
            if (randRunwayIndex == AudioTest3.pathRunwayIndex[i]) runwayIsActive = true;
        }

        if (!runwayIsActive)
        {
            SpawnStar();
            return;
        }
        */
    }

    void SpawnStar()
    {

    }
}
