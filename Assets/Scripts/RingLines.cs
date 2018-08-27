using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingLines : MonoBehaviour {

    public RunwayManager runwayManager;
    public Transform origin;
    public float startRadiusMultiplier;
    public float contractionPeriod;
    public int resolution;
    public Color32 lineColor;
    

    private Vector3[] linePoints;
  
    private LineRenderer line;
    private float startTime;
    private float startRadius;
   

    private void Start()//
    {
        line = GetComponent<LineRenderer>();
        
        startTime = Time.time;
        startRadius = startRadiusMultiplier * RunwayManager.minPathLength + RunwayManager.originRadius;
    }

	void FixedUpdate () {
        SetLinePoints();
	}

    void SetLinePoints()
    {
        float progress = (Time.time - startTime) / contractionPeriod;
        float smoothenProgress = Mathf.SmoothStep(0, 1, progress);
        float currentRadius = Mathf.Lerp(startRadius,0.5f * origin.localScale.x,progress*progress);
        linePoints = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            float pointSeperation = 210 / resolution;
            if (AudioTest3.AutoPlay)
            {
                pointSeperation = 410 / resolution;
            }
            linePoints[i] =VectorStuff.AlterVector(Vector3.up * currentRadius, pointSeperation * (i - 0.5f * (resolution - 1)),0) + origin.position;
        }
        line.positionCount = resolution;
        line.SetPositions(linePoints);
        line.startColor = Color.Lerp(Color.clear, lineColor, smoothenProgress);
        line.endColor = Color.Lerp(Color.clear, lineColor, smoothenProgress);
        if (currentRadius <= 0.5f * origin.localScale.x)
        {
            Destroy(this.transform.gameObject);
        }
    }
}
