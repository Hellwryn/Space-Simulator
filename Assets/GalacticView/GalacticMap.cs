using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalacticMap : MonoBehaviour
{
    public static GalacticMap inst;
    public GameObject galaxyPlane;
    public GameObject galStarRef;
    public List<GameObject> galaxyStarRefs;
    public GameObject galNebulaRef;
    public Material[] galNebulaMat;
    public int size;

    public Gradient gradient;
    public Material galaxyMat;
    public RenderTexture rt;
    public Texture2D galTexture;
    public Material nebSpreadText;
    public ParticleSystem nebSpreadParticle;
    public Texture2D nebTexture;


    public List<Vector2> squares;
    public List<Vector2Int> squaresCoord;
    public List<StarSystemData> starSystemDatas;
    public int numberStars;

    // TEST
    public bool GenerateButton;
    public bool GenerateStars;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
        squares = new List<Vector2>();
        squaresCoord = new List<Vector2Int>();
        starSystemDatas = new List<StarSystemData>();
        rt = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
    }

    void Update()
    {
        if (GenerateButton)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            squares = new List<Vector2>();
            squaresCoord = new List<Vector2Int>();
            GenerateButton = false;
            GenerateMap();
            numberStars = squares.Count / 1600;
            numberStars *= 100;
        }
        if (GenerateStars)
        {
            GenerateStars = false;
            GenerateStarsTest();
        }
    }

    void GenerateMap()
    {
        int currHeight = 0;
        float pairOffset = 0;
        for (int i = 0; i < 2 * size - 1; i++)
        {
            // Determine the number of rows in current column
            currHeight = size + i;
            if (i >= size)
                currHeight -= 2 * ((i + 1) - size);

            // Offset by half a unit according to column's parity
            if (i % 2 == size % 2)
                pairOffset = 0.5f;
            else
                pairOffset = 0f;

            for (int j = 0; j < currHeight; j++)
            {
                //squares.Add(Instantiate(squareRef, new Vector3((size + 1 - i) * 0.865f, 1200f, currHeight / 2 - j - pairOffset), Quaternion.identity, this.transform));
                squares.Add(new Vector2((size + 1 - i) * 0.865f, currHeight / 2 - j - pairOffset));
                squaresCoord.Add(new Vector2Int(i, j));
            }
            // -x / 0.865f + size + 1 = i
            // currHeight / 2 - y - pairOffset = j
        }
        galaxyPlane.transform.localScale = new Vector3(0.20f * size, 0.20f * size, 0.20f * size);
    }

    void GenerateStarsTest()
    {
        RenderTextureTest();

        int starPos = 0;
        int i = 0;
        int fails = 0;
        bool isFailed = false;
        while (i < numberStars)
        {
            starPos = Random.Range(0, squares.Count);
            if (fails < numberStars * 10)
            {
                // Chance to fail if texture is too dark
                Vector2 texturePoint = new Vector2((squares[starPos].x + size) / (2f * size) * 255f, (squares[starPos].y + size) / (2f * size) * 255f);
                Color textureColor = galTexture.GetPixel((int)texturePoint.x, (int)texturePoint.y);
                float chance = textureColor.r + textureColor.g + textureColor.b;
                // Skip if less than a threshold
                if (chance < 0.05f)
                {
                    isFailed = true;
                    fails += 1;
                }


                // Chance to fail base on the brightness of the pixel
                /*
                if (chance < Random.Range(0.0f, 1.0f))
                {
                    isFailed = true;
                    fails += 1;
                }
                */
            }

            if (isFailed == false)
            {
                // Get Nebula
                Vector2 nebPoint = new Vector2((squares[starPos].x + size) / (2f * size) * 511f, (squares[starPos].y + size) / (2f * size) * 511f);
                Color nebColor = nebTexture.GetPixel((int)nebPoint.x, (int)nebPoint.y);
                float nebChance = nebColor.a;
                int neb = -1;
                if (nebChance > 0.2)
                    neb = Random.Range(0, galNebulaMat.Length);
                GenerateStar(starPos, neb);
                i++;
            }
            else
                isFailed = false;
        }
        foreach (StarSystemData s in starSystemDatas)
        {
            int numberOfPlanets = Random.Range(5, 12);
            for (int ip = 0; ip < numberOfPlanets; ip++)
            {
                float orbitSize = (160f / (ip+2) + 1f) * 0.75f + (numberOfPlanets - ip) * 10f * 0.25f;
                if (Random.Range(0.0f, 1f) < 0.75f)
                {
                    // Check if telluric or jovian
                    if (ip > numberOfPlanets / 3 != Random.Range(0.0f, 1f) < 0.04f)
                    {
                        float Humidity = Random.Range(0.0f, 1.0f);
                        s.planetDatas.Add(new PlanetData(s, BodyType.telluric, orbitSize + s.starDatas.Count * 10f, Random.Range(0.5f, 4f) / ((numberOfPlanets - ip) * 0.5f) + 3, Random.Range(0.3f, 1.5f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Humidity * Humidity, Random.Range(0.0f, 40.0f), Random.Range(-1.0f, 3.0f)));
                    }
                    else
                    {
                        s.planetDatas.Add(new PlanetData(s, BodyType.jovian, orbitSize + s.starDatas.Count * 10f, Random.Range(0.5f, 4f) / ((numberOfPlanets - ip) * 0.5f) + 3, Random.Range(1.5f, 3.5f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0f, Random.Range(0.0f, 40.0f), Random.Range(-1.0f, 3.0f)));
                    }
                }
                else
                    s.planetDatas.Add(new PlanetData(s, BodyType.empty, orbitSize + s.starDatas.Count * 10f, 0f, 0f, 0f, 0f, 0f, 0f, 0f));
            }
        }
        GetCloseStars();
        GetCloseStarsWarp();
        GetCloseStarsWarp2();
    }

    void GenerateStar(int starPos, int nebula = -1)
    {
        float randSpectral = Random.Range(0f, 1f);
        float randSize = Random.Range(0f, 1f);

        float size = 1f - ((-randSpectral + 1f) * 0.5f * (1f - randSize) + ((randSpectral - 0.5f) * (randSpectral - 0.5f) + 0.25f) * randSize * 2f);

        GameObject currStarRef = galaxyStarRefs[5];

        if (randSpectral < 0.20f)
            currStarRef = galaxyStarRefs[0];
        else if (randSpectral < 0.40f)
            currStarRef = galaxyStarRefs[1];
        else if (randSpectral < 0.55f)
            currStarRef = galaxyStarRefs[2];
        else if (randSpectral < 0.65f)
            currStarRef = galaxyStarRefs[3];
        else if (randSpectral < 0.90f)
            currStarRef = galaxyStarRefs[4];

        GameObject starRes = Instantiate(currStarRef, new Vector3(squares[starPos].x + Random.Range(-0.15f, 0.15f), 1200f + Random.Range(-0.2f, 0.2f), squares[starPos].y + Random.Range(-0.15f, 0.15f)), Quaternion.identity, this.transform);

        StarData newStarData = new StarData(starRes, randSpectral, size);

        float scale = (size * size / 10f + 0.025f) / 3;
        starRes.transform.localScale = new Vector3(scale, scale, scale);

        int actualStarPos = GetStarPos(starPos);
        if (starSystemDatas.Count == actualStarPos)
        {
            StarSystemData res = new StarSystemData(starPos);
            res.AddStar(newStarData);
            starSystemDatas.Add(res);
        }
        else if (starSystemDatas[actualStarPos].starPos == starPos)
        {
            starSystemDatas[actualStarPos].AddStar(newStarData);
        }
        else
        {
            StarSystemData res = new StarSystemData(starPos);
            // TEST : add a nebula
            res.nebulaNbr = nebula;
            if (res.nebulaNbr > -1)
            {
                GameObject nebRes = Instantiate(galNebulaRef, new Vector3(starRes.transform.position.x, starRes.transform.position.y - 0.2f, starRes.transform.position.z), Quaternion.identity);
                nebRes.GetComponent<Renderer>().material = galNebulaMat[res.nebulaNbr];
                nebRes.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                nebRes.transform.SetParent(starRes.transform, true);
            }
            res.AddStar(newStarData);
            starSystemDatas.Insert(actualStarPos, res);
        }

        //SpriteRenderer starRenderer = starRes.GetComponent<SpriteRenderer>();
        //starRenderer.material = new Material(starRenderer.material);
        //starRenderer.color = gradient.Evaluate(randSpectral);
    }

    public void GetCloseStars()
    {
        foreach(StarSystemData sysData in starSystemDatas)
        {
            int initStarPos = sysData.starPos;
            //Debug.Log(initStarPos);
            Vector2Int initCoord = StarPosToCoord(initStarPos);
            //Debug.Log(initCoord);


            // Set range for current star system (closer to core, narrower)
            int max = initCoord.x;
            if (max > 0 && initCoord.y > max || max < 0 && initCoord.y < max)
                max = initCoord.y;
            if (max < 0)
                max = -max;
            int range = 4 + (max * 5 / size);


            List<int> upperxs = new List<int>();
            List<int> lowerxs = new List<int>();

            upperxs.Add(initCoord.y - range);
            if (upperxs[0] < 0)
                upperxs[0] = 0;
            lowerxs.Add(initCoord.y + range + 1);


            int leftOffset = 0;
            int rightOffset = 0;
            for (int i = 1; i <= range; i++)
            {
                int height = 2 * range + 1 - i;
                if (initCoord.x - i >= size)
                    leftOffset += 1;
                if (initCoord.x + i <= size)
                    rightOffset += 1;
                int leftUppery = initCoord.y - range + leftOffset;
                int leftLowery = leftUppery + height;
                if (leftUppery < 0)
                    leftUppery = 0;
                upperxs.Insert(0, leftUppery);
                lowerxs.Insert(0, leftLowery);
                int rightUppery = initCoord.y - range + rightOffset;
                int rightLowery = rightUppery + height;
                if (rightUppery < 0)
                    rightUppery = 0;
                upperxs.Add(rightUppery);
                lowerxs.Add(rightLowery);
            }

            for (int j = 0; j < upperxs.Count; j++)
            {
                for (int k = upperxs[j]; k < lowerxs[j]; k++)
                {
                    int currStarPos = CoordToStarPos(initCoord.x + j - range, k);
                    // Debug.Log(currStarPos);
                    if (currStarPos >= 0)
                    {
                        int actualStarPos = GetStarPos(currStarPos);
                        if (actualStarPos < starSystemDatas.Count && starSystemDatas[actualStarPos].starPos == currStarPos)
                        {
                            // There is a system here
                            sysData.closeStar.Add(actualStarPos);
                        }
                    }
                }
            }
            sysData.closeStar.Sort((a, b) => Vector3.Distance(sysData.starDatas[0].galStar.transform.position, starSystemDatas[a].starDatas[0].galStar.transform.position).CompareTo(Vector3.Distance(sysData.starDatas[0].galStar.transform.position, starSystemDatas[b].starDatas[0].galStar.transform.position)));
            foreach (int cl in sysData.closeStar)
            {
                sysData.distStar.Add(Vector3.Distance(sysData.starDatas[0].galStar.transform.position, starSystemDatas[cl].starDatas[0].galStar.transform.position));
            }
            if (sysData.closeStar.Count > 0)
            {
                sysData.closeStar.RemoveAt(0);
                sysData.distStar.RemoveAt(0);
            }

            sysData.closeLight = new List<int>(sysData.closeStar);
            sysData.closeLight.Sort((a, b) => LightDistance(sysData, a).CompareTo(LightDistance(sysData, b)));
            foreach (int cl in sysData.closeLight)
            {
                sysData.distLight.Add(LightDistance(sysData, cl));
            }


            sysData.closeHeavy = new List<int>(sysData.closeStar);
            sysData.closeHeavy.Sort((a, b) => HeavyDistance(sysData, a).CompareTo(HeavyDistance(sysData, b)));
            foreach (int cl in sysData.closeHeavy)
            {
                sysData.distHeavy.Add(HeavyDistance(sysData, cl));
            }
        }
    }


    public void GetCloseStarsWarp()
    {
        foreach(StarSystemData sysData in starSystemDatas)
        {
            sysData.closeWarp = new List<int>();
            int i = 0;
            int added = 0;
            while (i < sysData.closeStar.Count)
            {
                if (added < 3 && sysData.IsSystemReachable(GalacticMap.inst, sysData.closeStar[i]))
                {
                    sysData.closeWarp.Add(sysData.closeStar[i]);
                    sysData.distWarp.Add(sysData.distStar[i]);
                    added += 1;
                }
                i += 1;
            }
        }
    }

    public void GetCloseStarsWarp2()
    {
        for (int sn = 0; sn < starSystemDatas.Count; sn++)
        {
            StarSystemData sysData = starSystemDatas[sn];
            for (int i = 0; i < sysData.closeWarp.Count; i++)
            {
                int sw = sysData.closeWarp[i];
                bool present = false;
                foreach (int osw in GalacticMap.inst.starSystemDatas[sw].closeWarp)
                {
                    if (osw == sn)
                        present = true;
                }
                if (!present)
                {
                    GalacticMap.inst.starSystemDatas[sw].closeWarp.Add(sn);
                    GalacticMap.inst.starSystemDatas[sw].distWarp.Add(sysData.distWarp[i]);
                }
            }
        }
    }

/*
    void PruneCloseStars()
    {
        // For each star, we'll delete close neighbors if 
        foreach(StarSystemData sysData in starSystemDatas)
        {
            List<int> to_delete = new List<int>();
            for (int i = 0; i < sysData.closeStar.Count; i++)
            {

            }
        }
    }
*/

    float LightDistance(StarSystemData sysData, int a)
    {
        float dist = Vector3.Distance(sysData.starDatas[0].galStar.transform.position, starSystemDatas[a].starDatas[0].galStar.transform.position);

        float initStarPower = 0f;
        foreach (StarData sd in sysData.starDatas)
        {
            float newStarPower = (sd.size + 0.2f) * (sd.spectral + 1.0f) + 0.8f;
            if (newStarPower > initStarPower)
                initStarPower = newStarPower;
        }


        float targetStarPower = 1f;
        foreach (StarData osd in starSystemDatas[a].starDatas)
        {
            float onewStarPower = (osd.size + 0.2f) * (osd.spectral + 1.0f) * 4f + 1;
            if (onewStarPower > initStarPower)
                targetStarPower = onewStarPower;
        }
        // dist / 8 + 1
        return targetStarPower * (dist) / initStarPower;
    }

    float HeavyDistance(StarSystemData sysData, int a)
    {
        float dist = Vector3.Distance(sysData.starDatas[0].galStar.transform.position, starSystemDatas[a].starDatas[0].galStar.transform.position);

        float initStarPower = 1f;
        foreach (StarData sd in sysData.starDatas)
        {
            initStarPower = initStarPower + (sd.size + 0.25f) * 1.6f;
        }
        initStarPower /= (sysData.starDatas.Count + 1);

        float targetStarPower = 1f;
        foreach (StarData osd in starSystemDatas[a].starDatas)
        {
            targetStarPower = initStarPower + osd.size * 8f;
        }
        targetStarPower /= (starSystemDatas[a].starDatas.Count + 1f);
        targetStarPower = 9f - targetStarPower;

        return targetStarPower * (dist / 8f + 1f) * initStarPower * 0.4f;
    }

    public int GetStarPos(int starPos)
    {
        int bot = 0;
        int top = starSystemDatas.Count;
        if (top == 0)
            return 0;

        int i = starSystemDatas.Count / 2;

        while (top - bot > 1)
        {
            i = (top - bot) / 2 + bot;
            if (starPos == starSystemDatas[i].starPos)
                return i;
            if (starPos < starSystemDatas[i].starPos)
                top = i;
            else if (starPos > starSystemDatas[i].starPos)
                bot = i;
        }

        if (starSystemDatas[bot].starPos >= starPos)
            return bot;
        return top;
    }

    void RenderTextureTest()
    {
        Material galaxyUI = new Material(galaxyMat);
        galaxyUI.SetInt("UI", 1);
        RenderTexture buffer = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Graphics.Blit(null, buffer, galaxyUI, 0);
        RenderTexture.active = buffer;

        // Transfer RenderTexture to Texture2D
        galTexture = new Texture2D(256, 256, TextureFormat.RGB24, false);
        Rect rectReadPicture = new Rect(0, 0, 256, 256);
        galTexture.ReadPixels(rectReadPicture, 0, 0);
        galTexture.Apply();
        RenderTexture.active = null;

        // Do the same thing for the Nebula texture
        buffer = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Graphics.Blit(null, buffer, nebSpreadText, 0);
        RenderTexture.active = buffer;
        nebTexture = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        rectReadPicture = new Rect(0, 0, 512, 512);
        nebTexture.ReadPixels(rectReadPicture, 0, 0);
        nebTexture.Apply();
        RenderTexture.active = null;

        var shape = nebSpreadParticle.shape;
        shape.texture = nebTexture;
        shape.radius = size;
        nebSpreadParticle.gameObject.active = true;
        nebSpreadParticle.Clear();
        nebSpreadParticle.Simulate(20f);
        nebSpreadParticle.Play();
    }

    public int CoordToStarPos(int column, int row)
    {
        int res = 0;
        for (int i = 0; i < column; i++)
        {
            // Determine the number of rows in current column
            int currHeight = size + i;
            if (i >= size)
                currHeight -= 2 * ((i + 1) - size);
            res += currHeight;
        }
        res += row;
        return res;
    }

    public Vector2Int StarPosToCoord(int starPos)
    {
        return squaresCoord[starPos];
    }

    public int Vector2ToStarPos(Vector2 vect)
    {
        if (vect.x < 0)
            vect.x -= 1f;
        if (vect.y > 0)
            vect.y += 1f;

        int column = (int)(-vect.x / 0.865f - 0.5f) + size + 1;

        float pairOffset = 0;
        if (column % 2 == size % 2)
            pairOffset = 0.5f;

        int currHeight = size + column;
        if (column >= size)
            currHeight -= 2 * ((column + 1) - size);

        int row = currHeight / 2 - (int)(vect.y + pairOffset - 0.5f);

        return CoordToStarPos(column, row);
    }
}

[System.Serializable]
public class StarSystemData
{
    public int starPos;
    public int nebulaNbr;
    public List<StarData> starDatas;
    public List<PlanetData> planetDatas;
    public List<int> closeStar;
    public List<int> closeWarp;
    public List<int> closeLight;
    public List<int> closeHeavy;
    public List<float> distStar;
    public List<float> distWarp;
    public List<float> distLight;
    public List<float> distHeavy;

    public StarSystemData(int starPos)
    {
        this.starPos = starPos;
        nebulaNbr = -1;
        starDatas = new List<StarData>();
        planetDatas = new List<PlanetData>();
        closeStar = new List<int>();
        closeWarp = new List<int>();
        closeLight = new List<int>();
        closeHeavy = new List<int>();
        distStar = new List<float>();
        distWarp = new List<float>();
        distLight = new List<float>();
        distHeavy = new List<float>();
    }

    public void AddStar(StarData star)
    {
        starDatas.Add(star);
    }

    public bool IsSystemReachable(GalacticMap map, int starNumb)
    {
        for (int i = 0; i < closeStar.Count; i++)
        {
            // Check if the star system is a neighbor
            if (closeStar[i] == starNumb)
            {
                // Check if another neighbor is closer to this system
                for (int j = 0; j < closeStar.Count; j++)
                {
                    if (closeStar[j] == starNumb)
                        continue;
                    for (int k = 0; k < map.starSystemDatas[closeStar[j]].closeStar.Count; k++)
                    {
                        float multiplier = HyperlanesManager.inst.SystemConnectivity;
                        if (map.starSystemDatas[closeStar[j]].closeStar[k] == starNumb && map.starSystemDatas[closeStar[j]].distStar[k] * multiplier < distStar[i] && distStar[i] > distStar[j])
                            return false;
                    }
                }
                return true;
            }
        }
        return false;
    }
}

[System.Serializable]
public class StarData
{
    public GameObject galStar;
    public float spectral; // Special cases for special stars (neutron, black hole, etc...)
    public float size;

    public StarData(GameObject galStar, float spectral, float size)
    {
        this.galStar = galStar;
        this.spectral = spectral;
        this.size = size;
    }
}

public class PlanetData
{
    public StarSystemData starSystem;
    public BodyType bodyType;
    public float RevolutionDistance;
    public float RevolutionSpeed;
    public float Size;
    public float TelluricDensity;
    public float AtmosphericDensity;
    public float HumidityLevel;
    public float PrecessionAngle;
    public float RotationSpeed;
    public float EnergyReceived;
    public List<PlanetData> moonDatas;
    // public List<RegionData> regionDatas;

    public PlanetData(StarSystemData starSystem, BodyType bodyType, float RevolutionDistance, float RevolutionSpeed, float Size, float TelluricDensity, float AtmosphericDensity, float HumidityLevel, float PrecessionAngle, float RotationSpeed)
    {
        this.starSystem = starSystem;
        this.bodyType = bodyType;
        this.RevolutionDistance = RevolutionDistance;
        this.RevolutionSpeed = RevolutionSpeed;
        this.Size = Size;
        this.TelluricDensity = TelluricDensity;
        this.AtmosphericDensity = AtmosphericDensity;
        this.HumidityLevel = HumidityLevel;
        this.PrecessionAngle = PrecessionAngle;
        this.RotationSpeed = RotationSpeed;
        this.EnergyReceived = GetEnergyReceived();
        this.moonDatas = new List<PlanetData>();
    }

    //GetGravity
    //GetFloraColor

    public float GetEnergyReceived()
    {
        float res = 0f;

        // Compute the power generated by the multiple center stars
        float starPowers = 1f;
        foreach (StarData s in starSystem.starDatas)
        {
            float starPower = Mathf.Pow(s.size * s.spectral, 0.5f);
            starPowers = starPowers * (1 - starPower);
        }
        starPowers = 1 - starPowers;

        // Compute the lowest and highest possible temperatures, and lerp between those two
        //float averageTemp = (-1f) * Mathf.Pow((RevolutionDistance - 25f) / 65f * 2f - 1f, 3f) / 2f + 0.5f;
        float averageTemp = (-1f) * Mathf.Pow((RevolutionDistance - 25f) / 65f, 0.5f) + 1f;
        if (averageTemp < 0)
            averageTemp = 0;
        if (averageTemp > 1)
            averageTemp = 1;
        if (starPowers < 0.5f)
        {
            float lowestTemp = (-1f) * Mathf.Pow((RevolutionDistance - 25f) / 65f, 0.2f) + 1f;
            if (lowestTemp < 0)
                lowestTemp = 0;
            if (lowestTemp > 1)
                lowestTemp = 1;
            float lerpWeight = starPowers * 2f;
            res = lowestTemp * lerpWeight + averageTemp * (1f - lerpWeight);
        }
        else
        {
            //float highestTemp = (-1f) * Mathf.Pow((RevolutionDistance - 25f) / 65f, 2f) + 1f;
            float highestTemp = (-1f) * Mathf.Pow((RevolutionDistance - 25f) / 65f * 2f - 1f, 3f) / 2f + 0.5f;
            if (highestTemp < 0)
                highestTemp = 0;
            if (highestTemp > 1)
                highestTemp = 1;
            float lerpWeight = (starPowers - 0.5f) * 2f;
            res = averageTemp * lerpWeight + highestTemp * (1f - lerpWeight);
        }

        if (res < 0f)
            res = 0f;
        if (res > 1f)
            res = 1f;
        if (float.IsNaN(res))
            res = 1f;
        return res;
    }
}

public enum BodyType
{
    empty, // asteroid
    telluric,
    jovian
}