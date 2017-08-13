using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Text.RegularExpressions;
#endif

[CreateAssetMenu()]
public class AutomataRegla : ScriptableObject {
    public ushort[] regla = new ushort[9 * 9];
}

#if UNITY_EDITOR
[CustomEditor(typeof(AutomataRegla))]
public class AutomataReglaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AutomataRegla reglas = target as AutomataRegla;

        ushort[] regla = reglas.regla;

        EditorGUI.BeginChangeCheck();
        string reglaTxt = EditorGUILayout.TextField("Regla [[,,],[,,],[,,]]","");
        if(EditorGUI.EndChangeCheck())
        {
            if (regla == null) regla = new ushort[9 * 9];
            int i = 0;
            foreach (Match m in Regex.Matches(reglaTxt, "\\d"))
            {
                regla[i] = (ushort)(m.Value[0] - '0');
                if (regla[i] < 0) regla[i] = 0;
                else if (regla[i] > 3) regla[i] = 3;
                i++;
                if (i >= 9 * 9) break;
            }
        }

        if (regla.Length != 9 * 9) regla = new ushort[9 * 9];

        EditorGUI.BeginChangeCheck();
        for (int y = 0; y < 9; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < 9; x++)
            {
                int val = EditorGUILayout.IntField(regla[x * 9 + y], GUILayout.Width(EditorGUIUtility.singleLineHeight));
                if (val < 0) val = 0;
                else if (val > 3) val = 3;
                regla[x * 9 + y] = (ushort)val;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (EditorGUI.EndChangeCheck())
        {
            reglas.regla = regla;
        }
    }
}
#endif