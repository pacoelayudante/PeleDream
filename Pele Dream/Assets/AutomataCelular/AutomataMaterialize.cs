using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomataMaterialize : MonoBehaviour
{

    public Renderer rend;
    public Material baseMaterial;
    public AutomataCelular3Estados automata;
    public bool drawTexture = true;
    public Texture2D outputTexture;
    public Color32[] paleta32 = new Color32[] { new Color32(0, 0, 0, 255), new Color32(110, 0, 101, 255), new Color32(255, 1, 67, 255) };
    Color32[] pixels;

    private void Reset()
    {
        automata = FindObjectOfType<AutomataCelular3Estados>();
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!baseMaterial) baseMaterial = rend.material;
        if (rend && automata && baseMaterial)
        {
            ancho = automata.ancho;
            alto = automata.alto;
            if (!outputTexture)
            {
                outputTexture = new Texture2D(ancho, alto, TextureFormat.ARGB32, false, false);
                outputTexture.filterMode = FilterMode.Point;
                rend.material = baseMaterial;
                rend.material.mainTexture = outputTexture;
            }
            DrawOnTexture(outputTexture, automata.Estado);
        }
    }

    int xcel, ycel, xsamp, ysamp, isamp, alto, ancho;
    public void DrawOnTexture(Texture2D textura, ushort[] estado, Color[] paleta = null)
    {
        if (textura.width != ancho || textura.height != alto)
        {
            Debug.LogError("Size of texture must be size of automata grid.");
            return;
        }
        if (paleta != null)
        {
            for (int i = 0; i < paleta32.Length && i < paleta.Length; i++) paleta32[i] = paleta[i];
        }
        pixels = textura.GetPixels32();

        for (ycel = 0; ycel < alto; ycel++)
        {
            for (xcel = 0; xcel < ancho; xcel++)
            {
                isamp = xcel + ycel * ancho;
                pixels[isamp] = paleta32[estado[isamp]];
            }
        }

        textura.SetPixels32(pixels);
        textura.Apply();
    }
}