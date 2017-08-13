using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Delaunay;

public class VoronoiPasaReglas : MonoBehaviour {
    public int repeticiones = 4;
    public int tiposRegiones = 5;
    public AutomataCelular3Estados automata;
    public Voronoi voronoi;

    [HideInInspector]
    public List<Vector2> puntos = new List<Vector2>();
    
    private void Reset()
    {
        automata = FindObjectOfType<AutomataCelular3Estados>();
    }

    private void OnEnable()
    {
        if (!automata)
        {
            enabled = false;
            return;
        }
        if (automata.Reglas == null)
        {
            enabled = false;
            return;
        }
        puntos.Clear();
        List<uint> colors = new List<uint>();
        for (int i=0; i<automata.Reglas.Length* repeticiones; i++)
        {
            colors.Add(0);
            puntos.Add(new Vector2( Random.value*automata.ancho, Random.value*automata.alto ));
        }

        float minDgen = (automata.ancho * automata.alto);
        float minD = minDgen;
        float d = minDgen;
        ushort id = 0;
        for (ushort x = 0; x<automata.ancho; x++)
        {
            for (ushort y =0; y<automata.alto; y++)
            {
                id = 0;
                minD = (x - puntos[0].x) * (x - puntos[0].x) + (y - puntos[0].y) * (y - puntos[0].y);
                for (ushort p =1; p<puntos.Count; p++)
                {
                    d = (x - puntos[p].x) * (x - puntos[p].x) + (y - puntos[p].y) * (y - puntos[p].y);
                    if (d < minD)
                    {
                        minD = d;
                        id = (ushort)(p % automata.reglasIniciales.Length);
                    }
                }
                automata.SetRegla(x, y, id);
            }
        }
        //voronoi = new Voronoi(puntos,colors,new Rect(0,0,automata.ancho,automata.alto));
    }
}
