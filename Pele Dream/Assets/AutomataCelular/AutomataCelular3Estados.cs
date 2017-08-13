/*
 * 3 State 2d cellular automaton totally ripped off from
 * loren schmidt
 * @lorenschmidt
 * http://vacuumflowers.com/projects/
 * http://vacuumflowers.com/cellular_automata/rules.html
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AutomataCelular3Estados : MonoBehaviour
{

    public int ancho = 64, alto = 64;
    public AutomataRegla[] reglasIniciales;

    public ushort[] Estado
    {
        get
        {
            return estadoCruz;
        }
        set
        {
            estadoCara = value;
        }
    }

    public ushort[] DistrubucionReglas
    {
        get
        {
            if (distribucionReglas == null) return null;
            if (distribucionReglas.Length == 0) return null;
            return distribucionReglas;
        }
        set
        {
            distribucionReglas = value;
        }
    }

    public ushort[][] Reglas
    {
        get
        {
            if (i_reglas == null) return null;
            else if (i_reglas.Length == 0) return null;
            else return i_reglas;
        }
    }

    ushort[] estadoCara, estadoCruz, estadoSwaper, distribucionReglas;

    List<ushort[]> l_reglas = new List<ushort[]>();
    ushort[][] i_reglas;

    ushort cuentaUnos = 0, cuentaDos = 0, xker, yker, reglaout;

    int[] kernel = { -1, 0, 1 };
    int kernPnt = 0, xcel, ycel, xsamp, ysamp, isamp;

    private void Awake()
    {
        foreach (AutomataRegla r in reglasIniciales) Agregar(r);
    }

    private void OnEnable()
    {
        IniciarEstados();
    }
    
    public void Agregar(AutomataRegla reglaNueva)
    {
        Agregar(reglaNueva.regla);
    }
    public void Quitar(AutomataRegla reglaNueva)
    {
        Agregar(reglaNueva.regla);
    }
    public void Agregar(ushort[] reglaNueva)
    {
        if (!l_reglas.Contains(reglaNueva)) l_reglas.Add(reglaNueva);
        i_reglas = l_reglas.ToArray();
    }
    public void Quitar(ushort[] reglaNueva)
    {
        l_reglas.Remove(reglaNueva);
        if (l_reglas.Count == 0) i_reglas = null;
    }

    private void Update()
    {
        if (i_reglas == null || estadoCara == null || estadoCruz == null || kernel == null) IniciarEstados();
        if (i_reglas.Length == 0 || estadoCruz.Length == 0 || estadoCara.Length == 0 || kernel.Length == 0) IniciarEstados();

        Paso();
    }

    public void Paso()
    {
        Ciclo(estadoCara, estadoCruz, distribucionReglas, i_reglas);
        estadoSwaper = estadoCara;
        estadoCara = estadoCruz;
        estadoCruz = estadoSwaper;
    }

    public void IniciarEstados()
    {
        estadoCara = new ushort[ancho * alto];
        estadoCruz = new ushort[ancho * alto];
        distribucionReglas = new ushort[ancho * alto];
        i_reglas = l_reglas.ToArray();
        kernel = new int[] { -1, 0, +1 };
    }

    public void SetRegla(int x, int y, ushort regla)
    {
        if (DistrubucionReglas != null)
        {
            isamp = (x%ancho + (y%alto) * ancho);
            distribucionReglas[isamp] = regla;
        }
    }
    public void SetReglaGlobal(ushort indiceRegla)
    {
        for (int i = 0; i < distribucionReglas.Length; i++) distribucionReglas[i] = indiceRegla;
    }

    public void RuidoGlobal()
    {
        for (int i = 0; i < estadoCara.Length; i++) estadoCara[i] = (ushort)Random.Range(0, 3);
    }

    public void RuidoCentral(bool aditivo = false, int radio = 3)
    {
        for (ycel = 0; ycel < alto; ycel++)
        {
            for (xcel = 0; xcel < ancho; xcel++)
            {
                if (xcel > ancho / 2 - radio && xcel < ancho / 2 + radio && ycel > alto / 2 - radio && ycel < alto / 2 + radio) estadoCara[xcel + ycel * ancho] = (ushort)Random.Range(0, 3);
                else if (!aditivo) estadoCara[xcel + ycel * ancho] = 0;
            }
        }
    }

    public void Ciclo(ushort[] entra, ushort[] sale, ushort[] distribucionReglas, ushort[][] reglas, bool ignorarme = true)
    {
        for (ycel = 0; ycel < alto; ycel++)
        {
            for (xcel = 0; xcel < ancho; xcel++)
            {
                cuentaDos = cuentaUnos = 0;
                for (yker = 0; yker < kernel.Length; yker++)
                {
                    for (xker = 0; xker < kernel.Length; xker++)
                    {
                        if (ignorarme && kernel[xker] == 0 && kernel[yker] == 0) continue;
                        xsamp = (xcel + kernel[xker] + ancho) % ancho;// ((x + x2 + width) % width) according to Schmidt
                        ysamp = (ycel + kernel[yker] + alto) % alto;
                        isamp = xsamp + ysamp * ancho;
                        if (entra[isamp] == 1) cuentaUnos++;
                        else if (entra[isamp] == 2) cuentaDos++;
                    }
                }

                isamp = xcel + ycel * ancho;
                reglaout = reglas[distribucionReglas[isamp]][cuentaUnos * 9 + cuentaDos];
                if (reglaout == 3) sale[isamp] = entra[isamp];
                else sale[isamp] = reglaout;
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AutomataCelular3Estados))]
class AutomataCelular3EstadoEditor : Editor
{
    bool modohalf;
    bool ladoA;

    public override void OnInspectorGUI()
    {
        AutomataCelular3Estados automata = target as AutomataCelular3Estados;
        string[] opciones = new string[automata.reglasIniciales.Length];
        for (int i = 0; i < opciones.Length; i++) opciones[i] = automata.reglasIniciales[i].name;
        EditorGUI.BeginChangeCheck();
        ushort opc = (ushort)EditorGUILayout.Popup("Set Regla Global", automata.DistrubucionReglas==null?-1:automata.DistrubucionReglas[0], opciones);
        if (EditorGUI.EndChangeCheck() && automata.DistrubucionReglas!=null)
        {
            if (modohalf)
            {
                for (int x=0; x<automata.ancho; x++)
                {
                    for (int y = 0; y < automata.alto; y++)
                    {
                        if ( (ladoA && x < automata.ancho/2) || (!ladoA && x >= automata.ancho/2) )
                        {
                            automata.SetRegla(x + automata.ancho/4, y, opc);
                        }
                    }
                }
                ladoA = !ladoA;
            }
            else automata.SetReglaGlobal(opc);
        }
        modohalf = EditorGUILayout.Toggle("Modo Mitad Reglas",modohalf);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ruido Global")) automata.RuidoGlobal();
        if (GUILayout.Button("Ruido Central")) automata.RuidoCentral();
        if (GUILayout.Button("Ruido Central Aditivo")) automata.RuidoCentral(true);
        EditorGUILayout.EndHorizontal();
        DrawDefaultInspector();
    }
}
#endif