using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines : MonoBehaviour {

    public Material lineMat;
    public Vector3[] points;

    private MapReader map;

    void Start()
    {
        map = FindObjectOfType<MapReader>();
    }

    // Connect all of the `points` to the `mainPoint`
    void DrawConnectingLines()
    {
        foreach (OsmWay w in map.ways)
        {
            if (w.Visible)
            {
                for (int i = 1; i < w.NodeIDs.Count; i++)
                {
                    OsmNode p1 = map.nodes[w.NodeIDs[i - 1]];
                    OsmNode p2 = map.nodes[w.NodeIDs[i]];

                    Vector3 v1 = p1 - map.bounds.Centre;
                    Vector3 v2 = p2 - map.bounds.Centre;

                    GL.Begin(GL.LINES);
                    lineMat.SetPass(0);
                    GL.Color(new Color(lineMat.color.r, lineMat.color.g, lineMat.color.b, lineMat.color.a));
                    GL.Vertex3(v1.x, v1.y, v1.z);
                    GL.Vertex3(v2.x, v2.y, v2.z);
                    GL.End();
                }
            }
        }
    }

    // To show the lines in the game window whne it is running
    void OnPostRender() {
        if (map.IsReady)
        {
            DrawConnectingLines();
        }
    }
}
