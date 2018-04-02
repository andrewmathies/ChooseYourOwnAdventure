using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLines : MonoBehaviour {

    public Material lineMat;
    public Vector3[] points;

    private MapReader map;
   // private Camera cam;

    void Start()
    {
        map = FindObjectOfType<MapReader>();
        //cam = GetComponent<Camera>();
    }

    // Connect all of the `points` to the `mainPoint`
    void DrawConnectingLines()
    {
        float lineWidth = 2f;
        //GL.Begin(GL.QUADS);
        GL.Begin(GL.LINES);
        lineMat.SetPass(0);
        GL.Color(Color.black);

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

                    //Vector3 perpindicular = (new Vector3(v2.y, 10, v1.x) - new Vector3(v1.y, 10, v2.x)).normalized * thisWidth;

                    GL.Vertex3(v1.x, 10, v1.z);
                    GL.Vertex3(v2.x, 10, v2.z);

                    //if ((v1.x >= v2.x && v1.z <= v2.z) || (v1.x <= v2.x && v1.z >= v2.z))
                    //{
                    //    GL.Vertex3(v1.x, 10, v1.z);
                    //    GL.Vertex3(v1.x - lineWidth, 10, v1.z - lineWidth);
                    //    GL.Vertex3(v2.x, 10, v2.z);
                    //    GL.Vertex3(v2.x - lineWidth, 10, v2.z - lineWidth);
                    //} else
                    //{
                    //    GL.Vertex3(v1.x, 10, v1.z);
                    //    GL.Vertex3(v1.x - lineWidth, 10, v1.z + lineWidth);
                    //    GL.Vertex3(v2.x, 10, v2.z);
                    //    GL.Vertex3(v2.x - lineWidth, 10, v2.z + lineWidth);
                    //}
                }
            }
        }
        GL.End();
    }

    // cam.ViewportToWorldPoint(

    // To show the lines in the game window when it is running
    void OnPostRender() {
        if (map.IsReady)
        {
            DrawConnectingLines();
        }
    }
}
