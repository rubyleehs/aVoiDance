using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{

    public Transform spawnPos;
    public Transform endPos;
    public GameObject beatGO;
    public List<float> beats = new List<float>();
    public List<Transform> beatsVisuals = new List<Transform>();
    public int currentIndex;
    public int targetIndex;
    public LineRenderer pathBG;
    public Color missPathBGColor;
    public Color hitPathBGColor;

    public float pathBGEventFadeDuration;

    public GameObject captureParticleEffect;
    public float captureParticleEffectObjMaxLifetime;

    public float camShakeDuration;
    public float camShakeMagnitude;

    public AudioTest3 audioManager;
    private Color normPathBGColor;
    private float targetRotAngle;
    private float turnStartTime = 0;
    private float lastEventTime = 0;


    private void Awake()
    {
        beats = new List<float>();
        beatsVisuals = new List<Transform>();
        spawnPos.localPosition = new Vector3(0, RunwayManager.minPathLength + RunwayManager.originRadius, 0);
        endPos.localPosition = new Vector3(0, RunwayManager.originRadius, 0);
        targetIndex = currentIndex;
        normPathBGColor = pathBG.startColor;
        Color32 _endColor = pathBG.startColor;
        _endColor.a = 0;
        pathBG.endColor = _endColor;
        pathBG.positionCount = 2;
        pathBG.SetPosition(0, Vector3.zero);
        pathBG.SetPosition(1, Vector3.up * (RunwayManager.minPathLength + RunwayManager.originRadius));
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float turnProgress = (Time.time- turnStartTime)/audioManager.minTimeBetweenBeats;
        float currentIndexAngle = RunwayManager.runwayAngleSeperation * (currentIndex - 0.5f * (RunwayManager.runwayCount - 1));
        targetRotAngle = RunwayManager.runwayAngleSeperation * (targetIndex - 0.5f * (RunwayManager.runwayCount - 1));
        pathBG.startColor = Color.Lerp(pathBG.startColor, normPathBGColor, (Time.time - lastEventTime) / pathBGEventFadeDuration);

        this.transform.rotation = Quaternion.Lerp(Quaternion.Euler(new Vector3(0, 0, currentIndexAngle)), Quaternion.Euler(new Vector3(0, 0, targetRotAngle)), turnProgress);
        //this.transform.rotation = Quaternion.Euler(Vector3.MoveTowards(new Vector3(0,0,this.transform.rotation.z),new Vector3(0,0,targetRotAngle), RunwayManager.runwayAngleSeperation/audioManager.minTimeBetweenBeats));
        if (turnProgress >=0.94f)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(0, 0, targetRotAngle ));
            currentIndex = targetIndex;
        }


        for (int i = 0; i < beats.Count; i++)
        {
            float progress = (Time.time - beats[i] - audioManager.lagDelay) / audioManager.playDelay;//
            beatsVisuals[i].position = Vector3.Lerp(spawnPos.position, endPos.position, progress * progress);
            if (progress - (audioManager.beatCaptureTimeAllownce / audioManager.playDelay) >= 1)
            {
                FailBeat();
            }
        }

    }

    public void AddBeat()
    {
        beats.Add(Time.time);
        beatsVisuals.Add(Instantiate(beatGO, spawnPos.position, this.transform.rotation, this.transform).transform);
    }

    public void CaptureBeat()
    {
        turnStartTime = Time.time;
        beats.RemoveAt(0);
        Destroy(beatsVisuals[0].gameObject);
        beatsVisuals.RemoveAt(0);
        StartCoroutine(CaptureEffect());
        audioManager.AddToCombo();
        UIManager.beatCaptured++;
        Debug.Log("capture sucess!");
    }

    public IEnumerator CaptureEffect()
    {
        lastEventTime = Time.time;
        pathBG.startColor = hitPathBGColor;
        CameraShake.Shake(camShakeDuration, camShakeMagnitude * 0.3f);
        GameObject _effect = Instantiate(captureParticleEffect, endPos, false);
        _effect.transform.SetParent(null);
        yield return new WaitForSeconds(captureParticleEffectObjMaxLifetime);
        Destroy(_effect);
    }

    public void FailBeat()//add sound effect!
    {
        beats.RemoveAt(0);
        Destroy(beatsVisuals[0].gameObject);
        beatsVisuals.RemoveAt(0);
        UIManager.beatMissed++;
        FailEffect();
        AudioTest3.combo = 0;
    }

    public void FailEffect()
    {
        lastEventTime = Time.time;
        CameraShake.Shake(camShakeDuration, camShakeMagnitude);
        pathBG.startColor = missPathBGColor;
    }

    public void TryCaptureBeat(int dir)
    {
        if (beats.Count == 0) return;
        float beatDeltaTime = Time.time - beats[0] - audioManager.playDelay;
        //if (beatDeltaTime >= 0 && beatDeltaTime <= audioManager.beatCaptureTimeAllownce)
        if ((beatDeltaTime >= -audioManager.beatCaptureTimeAllownce && !GlobalData.Assist) || (GlobalData.Assist && beatDeltaTime >= 0 && beatDeltaTime <= audioManager.beatCaptureTimeAllownce))
        {
            currentIndex = targetIndex;
            targetIndex += dir;
            targetIndex = Mathf.Clamp(targetIndex, 0, RunwayManager.runwayCount - 1);
            //float pathRotAng = RunwayManager.runways[currentIndex].localRotation.z;
            CaptureBeat();
        }
        else if(beatDeltaTime >= -audioManager.beatMissTimeSpan)
        {
            FailBeat();
        }
    }

}
