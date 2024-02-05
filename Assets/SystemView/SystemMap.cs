using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SystemMap : MonoBehaviour
{
    public static SystemMap inst;
    public StarSystemData data;

    public GameObject starRef;
    public GameObject centerRef;
    public GameObject telluricRef;
    public GameObject jovianRef;
    public GameObject orbitRef;
    public GameObject nebulaRef;
    public GameObject asteroidRef;

    public Gradient waterGradient;
    public Gradient soilGradient;
    public Gradient aridGradient;
    public Gradient folliageGradient;
    public Gradient jovianGradient;

    public Texture2D[] textures;

    public Texture2D[] nebulaTextures;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }

    public void Initialize(StarSystemData starSystemData)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        data = starSystemData;
        GameObject orbitLine = new GameObject();
        orbitLine.transform.parent = this.transform;

        for (int i = 0; i < data.starDatas.Count; i++)
        {
            GameObject star = Instantiate(starRef, this.transform);

            // Set the orbit
            Orbit starOrbit = star.GetComponent<Orbit>();
            starOrbit.revolutionDistance = 10 * (data.starDatas.Count - 1);
            starOrbit.revolutionSpeed = 1f;
            starOrbit.revolutionOffset = 6.28f / (i + 1);

            // Set the scale
            float scale = data.starDatas[i].size * data.starDatas[i].size * 6f + 2f;
            star.transform.localScale = new Vector3(scale, scale, scale);

            // Set the color
            MeshRenderer starRenderer = star.GetComponent<MeshRenderer>();
            Material mat = new Material(starRenderer.material);
            Color color = GalacticMap.inst.gradient.Evaluate(data.starDatas[i].spectral);
            mat.SetColor("Base", color * 1.5f);
            mat.SetColor("Ripples", color * 1.0f);
            starRenderer.material = mat;
        }

        // TEST : put nebulas
        if (starSystemData.nebulaNbr > -1)
        {/*
            nebulaContainer neb = Instantiate(nebulaRef, new Vector3(25f, -20f, 25f), Quaternion.Euler(0f, 90f, 0f), this.transform).GetComponent<nebulaContainer>();
            neb.SetNebula(nebulaTextures[starSystemData.nebulaNbr]);
            nebulaContainer neb2 = Instantiate(nebulaRef, new Vector3(-25f, -20f, 25f), Quaternion.Euler(0f, 0f, 0f), this.transform).GetComponent<nebulaContainer>();
            neb2.SetNebula(nebulaTextures[starSystemData.nebulaNbr]);
            nebulaContainer neb3 = Instantiate(nebulaRef, new Vector3(25f, -20f, -25f), Quaternion.Euler(0f, 270f, 0f), this.transform).GetComponent<nebulaContainer>();
            neb3.SetNebula(nebulaTextures[starSystemData.nebulaNbr]);
            nebulaContainer neb4 = Instantiate(nebulaRef, new Vector3(-25f, -20f, -25f), Quaternion.Euler(0f, 180f, 0f), this.transform).GetComponent<nebulaContainer>();
            neb4.SetNebula(nebulaTextures[starSystemData.nebulaNbr]);
            */
            nebulaContainer neb = Instantiate(nebulaRef, new Vector3(0f, 0f, 0f), Quaternion.identity, this.transform).GetComponent<nebulaContainer>();
            neb.SetNebula(nebulaTextures[starSystemData.nebulaNbr]);
            
        }

        for (int i = 0; i < data.planetDatas.Count; i++)
        {
            if (data.planetDatas[i].bodyType == BodyType.empty)
            {
                GameObject roids = Instantiate(asteroidRef, this.transform);
                VisualEffect roidsVFX = roids.GetComponent<VisualEffect>();
                roidsVFX.SetFloat("radius", data.planetDatas[i].RevolutionDistance);
            }
            else
            {
                GameObject center = Instantiate(centerRef, this.transform);
                GameObject planet = null;
                GameObject clouds = null;
                GameObject atmo = null;
                if (data.planetDatas[i].bodyType == BodyType.telluric)
                {
                    planet = Instantiate(telluricRef, center.transform);
                    clouds = planet.transform.GetChild(0).gameObject;
                    atmo = planet.transform.GetChild(1).gameObject;
                }
                else if (data.planetDatas[i].bodyType == BodyType.jovian)
                {
                    planet = Instantiate(jovianRef, center.transform);
                }
                GameObject orbit = Instantiate(orbitRef, orbitLine.transform);


                // Set the orbit graphics
                CircleRenderer orbitCircle = orbit.GetComponent<CircleRenderer>();
                orbitCircle.DrawShape(256, data.planetDatas[i].RevolutionDistance);

                // Set the center orbit
                Orbit planetCenterOrbit = center.GetComponent<Orbit>();
                planetCenterOrbit.revolutionDistance = data.planetDatas[i].RevolutionDistance;
                planetCenterOrbit.revolutionSpeed = data.planetDatas[i].RevolutionSpeed;
                planetCenterOrbit.axialTilt = data.planetDatas[i].PrecessionAngle;

                // Set the planet orbit
                Orbit planetOrbit = planet.GetComponent<Orbit>();
                planetOrbit.rotationSpeed = data.planetDatas[i].RotationSpeed;
                planet.transform.localScale = new Vector3(data.planetDatas[i].Size, data.planetDatas[i].Size, data.planetDatas[i].Size);

                // Get the planet material
                Renderer r_Renderer = planet.GetComponent<Renderer>();
                Material m_Material = new Material(r_Renderer.sharedMaterial);
                r_Renderer.sharedMaterial = m_Material;

                if (data.planetDatas[i].bodyType == BodyType.telluric)
                {
                    // Get the clouds material
                    Renderer r_clouds = clouds.GetComponent<Renderer>();
                    Material m_clouds = new Material(r_clouds.sharedMaterial);
                    r_clouds.sharedMaterial = m_clouds;

                    // Get the atmo material
                    Renderer r_atmo = atmo.GetComponent<Renderer>();
                    Material m_atmo = new Material(r_atmo.sharedMaterial);
                    r_atmo.sharedMaterial = m_atmo;


                    // Choose planet colors
                    float energyReceived = data.planetDatas[i].EnergyReceived;
                    float atmosphericModifier = data.planetDatas[i].AtmosphericDensity * data.planetDatas[i].AtmosphericDensity * 0.5f;
                    float sqrtTemp = energyReceived;
                    float avgTemperature = 1f - ((1f - atmosphericModifier) * (1f - sqrtTemp) + atmosphericModifier * (1f - sqrtTemp) * (1f - sqrtTemp));

                    // Set the watertemp modifier so that only temperate planets have liquid water (and very hot planets have lava)
                    float watertemp = 0f;
                    float waterLevel = 0f;
                    if (energyReceived < 0.85f)
                    {
                        watertemp = 2f * avgTemperature;
                        if (watertemp > 1f)
                            watertemp = (-1) * watertemp;
                        watertemp -= 0.25f;
                        if (watertemp < 0f)
                            watertemp = 0f;
                        watertemp = watertemp * 0.5f;
                        waterLevel = data.planetDatas[i].HumidityLevel * watertemp;
                        m_Material.SetFloat("BiomeWeight", (1f - waterLevel) * 2f);
                    }
                    else
                    {
                        watertemp = 0f;
                        waterLevel = avgTemperature - 0.60f;
                        m_Material.SetFloat("BiomeWeight", 5f);
                    }
                    Debug.Log("Energy Received : " + energyReceived + " | Average Temperature : " + avgTemperature + " | WaterLevel : " + waterLevel);
                    Color waterColor = new Color(2f, 0.8f, 0.2f, 1f);
                    if (energyReceived < 0.85f)
                        waterColor = waterGradient.Evaluate(Mathf.Pow(avgTemperature, 0.21f)/2f);
                    else
                        m_Material.SetInt("_Lava", 1);
                    Color soilColor = soilGradient.Evaluate(data.planetDatas[i].TelluricDensity);
                    Color aridColor = aridGradient.Evaluate(energyReceived);
                    Color folliageColor = folliageGradient.Evaluate(Mathf.Pow(energyReceived, 0.27f)/2f);

                    float AxialTilt = data.planetDatas[i].PrecessionAngle / 40f;
                    m_Material.SetFloat("EquatorOpacity", (1f - AxialTilt) * (1f - AxialTilt) * 1.5f);
                    m_Material.SetFloat("EquatorLevel", AxialTilt * 0.6f + 0.2f);

                    // Set Planet Colors
                    m_Material.SetColor("OceanColor", waterColor);
                    m_Material.SetColor("CoastalWaterColor", waterColor * 0.8f + soilColor * 0.2f);
                    m_Material.SetColor("BeachColor", soilColor);
                    m_Material.SetColor("EquatorColor", soilColor);
                    if (watertemp > 0)
                        m_Material.SetColor("FirstBiomeColor", folliageColor);
                    else
                        m_Material.SetColor("FirstBiomeColor", soilColor);
                    m_Material.SetColor("SecondBiomeColor", aridColor);

                    // Set Planet Data
                    m_Material.SetFloat("OceanLimit", waterLevel);


                    // Set Clouds Color and Data
                    m_clouds.SetFloat("Fill", data.planetDatas[i].AtmosphericDensity / 3f + data.planetDatas[i].HumidityLevel / 3f);

                    // Set Atmo Color and Data
                    m_atmo.SetColor("AtmoColor", new Color(data.planetDatas[i].TelluricDensity, data.planetDatas[i].HumidityLevel, data.planetDatas[i].AtmosphericDensity, 1f));
                    m_atmo.SetColor("AtmoColorSpread", folliageColor);
                    m_atmo.SetFloat("AtmoIntensity", data.planetDatas[i].AtmosphericDensity * 2f);
                }
                if (data.planetDatas[i].bodyType == BodyType.jovian)
                {
                    float energyReceived = data.planetDatas[i].EnergyReceived;
                    float temperature = energyReceived;

                    Debug.Log("Energy Received : " + energyReceived + " | Temperature : " + temperature);

                    Color hotColor = jovianGradient.Evaluate(Mathf.Pow(temperature, 0.5f));
                    Color tempColor = jovianGradient.Evaluate(temperature);
                    Color coldColor = jovianGradient.Evaluate(Mathf.Pow(temperature, 2f));

                    // Set Planet Colors
                    m_Material.SetColor("_FirstColor", hotColor);
                    m_Material.SetColor("_SecondColor", (hotColor + tempColor) / 2f);
                    m_Material.SetColor("_ThirdColor", tempColor);
                    m_Material.SetColor("_FourthColor", (coldColor + tempColor) / 2f);
                    m_Material.SetColor("_FifthColor", coldColor);
                }
            }
        }
    }
}
