using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperlanesManager : MonoBehaviour
{
    public static HyperlanesManager inst;

    public GameObject hyperlaneRef;

    // TEST
    public int MaximumLanes = 4;
    public int MinimumLanes = 2;
    public int LanesDivider = 2;
    public float SystemConnectivity = 1.15f;

    public float maxDistance = 3f;

    public bool GenerateLanes;

    public bool displayStarLanes;
    public bool displayLightLanes;
    public bool displayHeavyLanes;


    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }

    void Update()
    {
        if (GenerateLanes)
        {
            GenerateBaseLanes();
            GenerateLanes = false;
        }
    }

    public void Link(int starSys1, int starSys2)
    {
        Vector3 pos1 = GalacticMap.inst.starSystemDatas[starSys1].starDatas[0].galStar.transform.position;
        Vector3 pos2 = GalacticMap.inst.starSystemDatas[starSys2].starDatas[0].galStar.transform.position;

        GameObject hypObj = Instantiate(hyperlaneRef, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform);
        LineRenderer lr = hypObj.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { pos1, pos2 });

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.white, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        lr.colorGradient = gradient;
    }

    public void Link(int starSys1, int starSys2, Color color, bool verso = false)
    {
        Vector3 pos1 = GalacticMap.inst.starSystemDatas[starSys1].starDatas[0].galStar.transform.position;
        Vector3 pos2 = GalacticMap.inst.starSystemDatas[starSys2].starDatas[0].galStar.transform.position;

        GameObject hypObj = Instantiate(hyperlaneRef, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform);
        LineRenderer lr = hypObj.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { pos1, pos2 });

        Gradient gradient = new Gradient();
        if (verso)
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f), new GradientAlphaKey(0.5f, 1.0f) }
        );
        else
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        lr.colorGradient = gradient;
    }

    public void Link(int starSys1, int starSys2, Color color, float intensity, bool verso = false)
    {
        Vector3 pos1 = GalacticMap.inst.starSystemDatas[starSys1].starDatas[0].galStar.transform.position;
        Vector3 pos2 = GalacticMap.inst.starSystemDatas[starSys2].starDatas[0].galStar.transform.position;

        GameObject hypObj = Instantiate(hyperlaneRef, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform);
        LineRenderer lr = hypObj.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { pos1, pos2 });

        Gradient gradient = new Gradient();
        if (verso)
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(intensity, 0.0f), new GradientAlphaKey(intensity, 1.0f) }
        );
        else
            gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(intensity, 0.0f), new GradientAlphaKey(intensity * 0.2f, 1.0f) }
        );
        lr.colorGradient = gradient;
    }

    // GalacticMap.inst

    void GenerateBaseLanes()
    {
        foreach (Transform child in HyperlanesManager.inst.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (displayStarLanes)
            for (int starData = 0; starData < GalacticMap.inst.starSystemDatas.Count; starData++)
            {
                if (GalacticMap.inst.starSystemDatas[starData].nebulaNbr == -1)
                    for (int i = 0; i < GalacticMap.inst.starSystemDatas[starData].closeWarp.Count; i++)
                    {
                        if (GalacticMap.inst.starSystemDatas[GalacticMap.inst.starSystemDatas[starData].closeWarp[i]].nebulaNbr != -1)
                            HyperlanesManager.inst.Link(starData, GalacticMap.inst.starSystemDatas[starData].closeWarp[i], new Color(0.8f, 0.2f, 0.1f, 0.7f), true);
                        else
                            HyperlanesManager.inst.Link(starData, GalacticMap.inst.starSystemDatas[starData].closeWarp[i], new Color(0.2f, 0.9f, 0.7f, 1.0f), true);
                    }
                else
                    for (int i = 0; i < GalacticMap.inst.starSystemDatas[starData].closeWarp.Count; i++)
                    {
                        if (GalacticMap.inst.starSystemDatas[GalacticMap.inst.starSystemDatas[starData].closeWarp[i]].nebulaNbr != -1)
                            HyperlanesManager.inst.Link(starData, GalacticMap.inst.starSystemDatas[starData].closeWarp[i], new Color(0.8f, 0.2f, 0.1f, 0.7f));
                    }
            }

        if (displayLightLanes)
            for (int starData = 0; starData < GalacticMap.inst.starSystemDatas.Count; starData++)
            {
                float initStarPower = 0f;
                foreach (StarData sd in GalacticMap.inst.starSystemDatas[starData].starDatas)
                {
                    float newStarPower = (sd.size + 0.2f) * (sd.spectral + 1.0f) + 0.8f;
                    if (newStarPower > initStarPower)
                        initStarPower = newStarPower;
                }

                int MaximumLightLanes = (int)((float)MaximumLanes * (1 + initStarPower / 2));
                int MinimumLightLanes = (int)((float)MinimumLanes * (1 + initStarPower / 2));

                int min = GalacticMap.inst.starSystemDatas[starData].closeLight.Count / LanesDivider;
                if (min > MaximumLightLanes)
                    min = MaximumLightLanes;

                if (min < MinimumLightLanes)
                    min = MinimumLightLanes;

                if (min > GalacticMap.inst.starSystemDatas[starData].closeLight.Count)
                    min = GalacticMap.inst.starSystemDatas[starData].closeLight.Count;
                //foreach (int closeStarData in GalacticMap.inst.starSystemDatas[starData].closeLight)
                for (int i = 0; i < min; i++)
                    if (GalacticMap.inst.starSystemDatas[starData].distLight[i] < maxDistance)
                        HyperlanesManager.inst.Link(starData, GalacticMap.inst.starSystemDatas[starData].closeLight[i], new Color(0.9f, 0.6f, 0.2f, 1.0f));
            }

        if (displayHeavyLanes)
            for (int starData = 0; starData < GalacticMap.inst.starSystemDatas.Count; starData++)
            {
                int min = GalacticMap.inst.starSystemDatas[starData].closeHeavy.Count / LanesDivider;
                if (min > MaximumLanes)
                    min = MaximumLanes;

                if (min < MinimumLanes)
                    min = MinimumLanes;

                if (min > GalacticMap.inst.starSystemDatas[starData].closeHeavy.Count)
                    min = GalacticMap.inst.starSystemDatas[starData].closeHeavy.Count;
                //foreach (int closeStarData in GalacticMap.inst.starSystemDatas[starData].closeHeavy)
                for (int i = 0; i < min; i++)
                    if (GalacticMap.inst.starSystemDatas[starData].distHeavy[i] < maxDistance)
                        HyperlanesManager.inst.Link(starData, GalacticMap.inst.starSystemDatas[starData].closeHeavy[i], new Color(0.45f, 0.1f, 0.7f, 1.0f));
            }
    }

    public void GenerateLane(int a, bool repeat = false)
    {
        if (!repeat)
        {
            foreach (Transform child in HyperlanesManager.inst.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        if (displayStarLanes)
        {
            for (int i = 0; i < GalacticMap.inst.starSystemDatas[a].closeWarp.Count; i++)
            {
                if (!repeat)
                    HyperlanesManager.inst.Link(a, GalacticMap.inst.starSystemDatas[a].closeWarp[i], new Color(0.4f, 0.95f, 0.85f, 1.0f), 1f / GalacticMap.inst.starSystemDatas[a].distStar[i], true);
                else
                    HyperlanesManager.inst.Link(a, GalacticMap.inst.starSystemDatas[a].closeWarp[i], new Color(0.2f, 0.9f, 0.7f, 1.0f), 0.10f / GalacticMap.inst.starSystemDatas[a].distStar[i], true);
                // Depth-2 pass
                if (repeat == false)
                    GenerateLane(GalacticMap.inst.starSystemDatas[a].closeWarp[i], true);
            }
        }

        if (displayLightLanes)
        {
            float initStarPower = 0f;
            foreach (StarData sd in GalacticMap.inst.starSystemDatas[a].starDatas)
            {
                float newStarPower = (sd.size + 0.2f) * (sd.spectral + 1.0f) + 0.8f;
                if (newStarPower > initStarPower)
                    initStarPower = newStarPower;
            }

            int MaximumLightLanes = (int)((float)MaximumLanes * (1 + initStarPower / 2));
            int MinimumLightLanes = (int)((float)MinimumLanes * (1 + initStarPower / 2));

            int min = GalacticMap.inst.starSystemDatas[a].closeStar.Count / LanesDivider;
            if (min > MaximumLightLanes)
                min = MaximumLightLanes;

            if (min < MinimumLightLanes)
                min = MinimumLightLanes;

            if (min > GalacticMap.inst.starSystemDatas[a].closeLight.Count)
                min = GalacticMap.inst.starSystemDatas[a].closeLight.Count;
            //foreach (int closeStarData in GalacticMap.inst.starSystemDatas[a].closeLight)
            for (int i = 0; i < min; i++)
                if (GalacticMap.inst.starSystemDatas[a].distLight[i] < maxDistance)
                {
                    if (!repeat)
                        HyperlanesManager.inst.Link(a, GalacticMap.inst.starSystemDatas[a].closeLight[i], new Color(0.95f, 0.8f, 0.4f, 1.0f), 1f / GalacticMap.inst.starSystemDatas[a].distLight[i]);
                    else
                        HyperlanesManager.inst.Link(a, GalacticMap.inst.starSystemDatas[a].closeLight[i], new Color(0.9f, 0.6f, 0.2f, 1.0f), 0.05f / GalacticMap.inst.starSystemDatas[a].distLight[i]);
                    // Depth-2 pass
                    if (repeat == false)
                        GenerateLane(GalacticMap.inst.starSystemDatas[a].closeLight[i], true);
                }
        }

        if (displayHeavyLanes)
        {
            int min = GalacticMap.inst.starSystemDatas[a].closeStar.Count / LanesDivider;
            if (min > MaximumLanes)
                min = MaximumLanes;

            if (min < MinimumLanes)
                min = MinimumLanes;

            if (min > GalacticMap.inst.starSystemDatas[a].closeHeavy.Count)
                min = GalacticMap.inst.starSystemDatas[a].closeHeavy.Count;
            //foreach (int closeStarData in GalacticMap.inst.starSystemDatas[a].closeHeavy)
            for (int i = 0; i < min; i++)
                if (GalacticMap.inst.starSystemDatas[a].distHeavy[i] < maxDistance)
                {
                    if (!repeat)
                        HyperlanesManager.inst.Link(a, GalacticMap.inst.starSystemDatas[a].closeHeavy[i], new Color(0.725f, 0.2f, 0.85f, 1.0f), 1f / GalacticMap.inst.starSystemDatas[a].distHeavy[i]);
                    else
                        HyperlanesManager.inst.Link(a, GalacticMap.inst.starSystemDatas[a].closeHeavy[i], new Color(0.45f, 0.1f, 0.7f, 1.0f), 0.15f / GalacticMap.inst.starSystemDatas[a].distHeavy[i]);
                    // Depth-2 pass
                    if (repeat == false)
                        GenerateLane(GalacticMap.inst.starSystemDatas[a].closeHeavy[i], true);
                }
        }
    }
}
