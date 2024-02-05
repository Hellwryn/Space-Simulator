using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nebulaContainer : MonoBehaviour
{
    public ParticleSystem[] particleSystem;
    public Renderer[] renderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNebula(Texture2D texture2D)
    {
        foreach (ParticleSystem p in particleSystem)
        {
            var shape = p.shape;
            shape.texture = texture2D;
            p.Clear();
            p.Simulate(20f);
            p.Play();
        }
        foreach (Renderer r in renderer)
        {
            Material mat = new Material(r.material);
            mat.SetTexture("_Texture2D", texture2D);
            r.material = mat;
        }
    }
}
