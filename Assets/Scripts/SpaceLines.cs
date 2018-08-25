using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceLines : MonoBehaviour
{

    public int runwayCount;
    public float runwayLength;
    public GameObject runwayLinesGO;


    private Camera cam;
    private List<Transform> runways;
    private float runwayAngleSeperation;

    private void Awake()
    {
        runways = new List<Transform>();
        runwayAngleSeperation = 180f / runwayCount;
        cam = Camera.main;
        runwayLength = cam.orthographicSize * cam.aspect ;

        for (int i = 0; i < runwayCount; i++)
        {
            CreateRunway(i);
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateRunway(int runwayIndex)
    {
        LineRenderer line = Instantiate(runwayLinesGO, this.transform).transform.GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = 2;
        line.SetPosition(0, new Vector3(0, 0.5f, 0));
        line.SetPosition(1, new Vector3(0, 0.5f + cam.orthographicSize / this.transform.localScale.y, 0));

        List<Transform> cal_runways = new List<Transform>();
        runwayIndex = Mathf.Min(runwayIndex, runwayCount);
        for (int i = 0; i < runwayIndex; i++)
        {
            cal_runways.Add(runways[i]);
        }
        cal_runways.Add(line.transform);
        for (int i = runwayIndex; i < runways.Count; i++)
        {
            cal_runways.Add(runways[i]);
        }

        runways.Clear();
        runways.AddRange(cal_runways);

        for (int i = 0; i < runways.Count; i++)//
        {
            float runwayRotAng = runwayAngleSeperation * (i - 0.5f * (runways.Count - 1));
            runways[i].rotation = Quaternion.Euler(new Vector3(0, 0, runwayRotAng));
        }
    }
}
