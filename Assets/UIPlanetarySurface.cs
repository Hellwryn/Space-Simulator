using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlanetarySurface : MonoBehaviour
{
    private Material m_Material;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectPlanet(PlanetGeneration pg)
    {
        SetShaderValues(pg);
    }

    void Initialize()
    {
        m_Material = this.GetComponent<Image>().material;
    }

    private void SetShaderValues(PlanetGeneration pg)
    {
        if (m_Material == null)
            Initialize();
        m_Material.SetVector("SeedOffset", pg.seedOffset);
        m_Material.SetFloat("Width", pg.width);
        m_Material.SetFloat("BorderChaos", pg.borderChaos);
        m_Material.SetFloat("NorthPole", pg.northPole);
        m_Material.SetFloat("SouthPole", pg.southPole);
        m_Material.SetFloat("OceanLimit", pg.OceanLimit);
        m_Material.SetColor("OceanColor", pg.OceanColor);
        m_Material.SetFloat("CoastSize", pg.CoastSize);
        m_Material.SetColor("CoastalWaterColor", pg.CoastalWaterColor);
        m_Material.SetFloat("BeachSize", pg.BeachSize);
        m_Material.SetColor("BeachColor", pg.BeachColor);
        m_Material.SetColor("FirstBiomeColor", pg.FirstBiomeColor);
        m_Material.SetColor("SecondBiomeColor", pg.SecondBiomeColor);
        m_Material.SetFloat("BiomeSize", pg.BiomeSize);
        m_Material.SetFloat("BiomeWeight", pg.BiomeWeight);
        m_Material.SetFloat("BiomesLimit", pg.BiomesLimit);
        m_Material.SetColor("MountainColor", pg.MountainColor);
        m_Material.SetFloat("NightLevel", pg.NightLevel);
        m_Material.SetInt("UI", 1);
    }
}
