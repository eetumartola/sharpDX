using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using SharpDX;
using System.Diagnostics;
using System.Globalization;


public struct HPrimitive
{

}

struct HVecAttribute
{
    public string name;
    public uint size;
    public string type;
    public Vector3 defaultVal;
    public Vector3[] attr;
}

public struct tri
{
    public uint[] verts;

    public tri(uint u0, uint u1, uint u2)
    {
        verts = new[] { u0, u1, u2 };
    }
}

//[StructLayout(LayoutKind.Sequential)]
class HClassicGeo
{
    public uint NPoints;
    public uint NPrims;
    public uint NPointGroups;
    public uint NPrimGroups;
    public uint NPointAttrib;
    public uint NVertexAttrib;
    public uint NPrimAttrib;
    public uint NAttrib;
    public Vector3[] Points;
    public tri[] Tris;
    public HVecAttribute[] PointAttributes;
    public HVecAttribute[] VertexAttributes;
    public HVecAttribute[] PrimAttributes;
}


class HClassicLoader
{

    private HClassicGeo geo;
    private StreamReader geoLines;

    public HClassicGeo Parse(string path)
    {
        geo = new HClassicGeo();
        geoLines = File.OpenText(path);
        bool headerOK = ParseHeader();
        ParsePoints();
        ParsePrims();
        ParseGroups();
        if(geo.NVertexAttrib > 0)
        {
            DuplicateVertices();
        }
        return geo;
    }

    private bool ParseHeader()
    {
        var magic = geoLines.ReadLine();
        Debug.WriteLine(magic);
        if (!magic.Equals("PGEOMETRY V5"))
        {
            Debug.WriteLine("HEader not found");
            return false;
        }
        Debug.WriteLine("HEader found");

        var line1 = geoLines.ReadLine().Split();
        geo.NPoints = Convert.ToUInt32(line1[1]);
        geo.NPrims = Convert.ToUInt32(line1[3]);
        Debug.WriteLine("Npoints " + geo.NPoints + " NPrims " + geo.NPrims);

        var line2 = geoLines.ReadLine().Split();
        geo.NPointGroups = Convert.ToUInt32(line2[1]);
        geo.NPrimGroups = Convert.ToUInt32(line2[3]);
        Debug.WriteLine("NPointGroups " + geo.NPointGroups + " NPrimGroups " + geo.NPrimGroups);

        var line3 = geoLines.ReadLine().Split();
        geo.NPointAttrib = Convert.ToUInt32(line3[1]);
        geo.NVertexAttrib = Convert.ToUInt32(line3[3]);
        geo.NPrimAttrib = Convert.ToUInt32(line3[5]);
        geo.NAttrib = Convert.ToUInt32(line3[7]);
        Debug.WriteLine("NPointAttrib " + geo.NPointAttrib + " NVertexAttrib " + geo.NVertexAttrib + " NPrimAttrib " + geo.NPrimAttrib + " NAttrib " + geo.NAttrib);

        if (geo.NPointAttrib > 0)
        {
            var dummyline = geoLines.ReadLine();
            Debug.WriteLine(dummyline);
            geo.PointAttributes = new HVecAttribute[geo.NPointAttrib];
            for (int i = 0; i < geo.NPointAttrib; i++)
            {
                var line = geoLines.ReadLine().Split();
                Debug.WriteLine(i + "  " + line[0]);
                geo.PointAttributes[i].name = line[0];
                geo.PointAttributes[i].size = Convert.ToUInt32(line[1]);
                geo.PointAttributes[i].type = line[2];
                geo.PointAttributes[i].defaultVal = new Vector3(Convert.ToSingle(line[3], CultureInfo.InvariantCulture), Convert.ToSingle(line[4], CultureInfo.InvariantCulture), Convert.ToSingle(line[5], CultureInfo.InvariantCulture));
                geo.PointAttributes[i].attr = new Vector3[geo.NPoints];
            }
        }

        return true; //parsing the header was a success
    }

    private bool ParsePoints()
    {
        geo.Points = new Vector3[geo.NPoints];
        for (int i = 0; i < geo.NPoints; i++)
        {
            var splitLine = geoLines.ReadLine().Split(new char[] { '(', ')' });

            var lineP = splitLine[0].Split();
            //Debug.WriteLine(i + " : " + splitLine[0]);
            float x = Convert.ToSingle(lineP[0], CultureInfo.InvariantCulture);
            float y = Convert.ToSingle(lineP[1], CultureInfo.InvariantCulture);
            float z = Convert.ToSingle(lineP[2], CultureInfo.InvariantCulture);
            geo.Points[i] = new Vector3(x, y, z);

            var lineAttr = splitLine[1].Split();
            //Debug.WriteLine(i + " : " + splitLine[1] + " " + lineAttr.Length);
            for (int k = 0; k < geo.NPointAttrib; k++)
            {
                float a1 = Convert.ToSingle(lineAttr[3 * k + 0], CultureInfo.InvariantCulture);
                float a2 = Convert.ToSingle(lineAttr[3 * k + 1], CultureInfo.InvariantCulture);
                float a3 = Convert.ToSingle(lineAttr[3 * k + 2], CultureInfo.InvariantCulture);
                geo.PointAttributes[k].attr[i] = new Vector3(a1, a2, a3);
                //Debug.WriteLine(i + " : " + geo.PointAttributes[k].attr[i]);
            }
        }
        return true;
    }

    private bool ParsePrims()
    {

        if (geo.NVertexAttrib > 0)
        {
            var vattribline = geoLines.ReadLine();
            Debug.WriteLine(vattribline); //should be Run xxxx Poly
            if (!vattribline.Equals("VertexAttrib")) Debug.WriteLine(" ERROR, line should be VertexAttrib");

            geo.VertexAttributes = new HVecAttribute[geo.NVertexAttrib];
            for (int i = 0; i < geo.NVertexAttrib; i++)
            {
                var line = geoLines.ReadLine().Split();
                Debug.WriteLine(i + "  " + line[0]);
                geo.VertexAttributes[i].name = line[0];
                geo.VertexAttributes[i].size = Convert.ToUInt32(line[1]);
                geo.VertexAttributes[i].type = line[2];
                geo.VertexAttributes[i].defaultVal = new Vector3(Convert.ToSingle(line[3], CultureInfo.InvariantCulture), Convert.ToSingle(line[4], CultureInfo.InvariantCulture), Convert.ToSingle(line[5], CultureInfo.InvariantCulture));
                geo.VertexAttributes[i].attr = new Vector3[geo.NPrims * 3];
            }
        }

        if (geo.NPrimAttrib > 0)
        {
            var prAttribline = geoLines.ReadLine();
            Debug.WriteLine(prAttribline);
            if (!prAttribline.Equals("PrimitiveAttrib")) Debug.WriteLine(" ERROR, line should be PrimitiveAttrib");

            geo.PrimAttributes = new HVecAttribute[geo.NPrimAttrib];
            for (int i = 0; i < geo.NPrimAttrib; i++)
            {
                var line = geoLines.ReadLine().Split();
                Debug.WriteLine(i + "  " + line[0]);
                //geo.PrimAttributes[i].name = line[0];
                //geo.PrimAttributes[i].size = Convert.ToUInt32(line[1]);
                //geo.PrimAttributes[i].type = line[2];
                //geo.PrimAttributes[i].defaultVal = new Vector3(Convert.ToSingle(line[3], CultureInfo.InvariantCulture), Convert.ToSingle(line[4], CultureInfo.InvariantCulture), Convert.ToSingle(line[5], CultureInfo.InvariantCulture));
                //geo.PrimAttributes[i].attr = new Vector3[geo.NPrims];
            }
        }

        var dummyline = geoLines.ReadLine();
        Debug.WriteLine(dummyline); //should be Run xxxx Poly

        geo.Tris = new tri[geo.NPrims];
        for (int i = 0; i < geo.NPrims; i++)
        {
            var splitLine = geoLines.ReadLine().Split(new char[] { '[', ']' }); //splitLine[0] = vertex stuff, splitLine[1] = prim stuff

            var vLine = splitLine[0].Split(new char[] { '(', ')', ' ' });
           
            int vIdx = 1; //skip initial whitespace(tab?)
            
            //for (int k = 0; k < vLine.Length; k++) Debug.WriteLine(k + " : " + vLine[k]);

            if (!vLine[vIdx].Equals("3"))
            {
                Debug.WriteLine("Only triangles supported, sorry");
                return false;
            }

            vIdx += 2; //skip open/closed tag ('<' or ':')

            uint[] indices = new uint[3];
            for (int vert = 0; vert < 3; vert++)
            {
                indices[vert] = Convert.ToUInt32(vLine[vIdx], CultureInfo.InvariantCulture);
                //Debug.WriteLine("prim " + i + " vert " + vert + " : " + vLine[vIdx]);
                vIdx++;
                vIdx++;
                for (int a = 0; a < geo.NVertexAttrib; a++)
                {
                    float[] vecVal = new float[geo.VertexAttributes[a].size];
                    for (int b = 0; b < geo.VertexAttributes[a].size; b++)
                    {
                        vecVal[b] = Convert.ToSingle(vLine[vIdx], CultureInfo.InvariantCulture);
                        vIdx++;
                    }
                    geo.VertexAttributes[a].attr[3 * i + vert] = new Vector3(vecVal);
                }
                vIdx++;

            }
            //uint i1 = Convert.ToUInt32(vLine[4], CultureInfo.InvariantCulture);
            //uint i2 = Convert.ToUInt32(vLine[5], CultureInfo.InvariantCulture);
            geo.Tris[i] = new tri(indices[0], indices[1], indices[2]);
            //Debug.WriteLine("prim " + i + " vert indices: " + indices[0] + " " + indices[1] + " " + indices[2]);

            var pLine = splitLine[1].Split(); // per primitive attributes
        }
        return true;
    }

    private void DuplicateVertices()
    {
        //Dictionary<int, UnweldVertex> vertexBuffer = new Dictionary<int, UnweldVertex>();

        List<Vector3> newPoints = new List<Vector3>();
        List<Vector3> newUVs = new List<Vector3>();
        List<Vector3> newNormals = new List<Vector3>();

        Dictionary<uint, Vector3> uvBuffer = new Dictionary<uint, Vector3>();

        /*
        // first fill in uvs for the existing points
        for (uint triIdx = 0; triIdx < geo.Tris.Length; triIdx++)
        {
            for (uint triVertIdx = 0; triVertIdx < 3; triVertIdx++)
            {
                uint vertPoint = geo.Tris[ triIdx ].verts[ triVertIdx ];
                Vector3 vertUV = geo.VertexAttributes[0].attr[3 * triIdx + triVertIdx];
                if( !uvBuffer.ContainsKey(vertPoint))
                {
                    uvBuffer.Add(vertPoint, vertUV);
                }
            }
        }
        */



        uint newVertIdx = 0;

        for (uint triIdx = 0;  triIdx < geo.Tris.Length; triIdx++)
        {
            for (uint triVertIdx = 0; triVertIdx < 3; triVertIdx++)
            {
                uint vertPoint = geo.Tris[triIdx].verts[triVertIdx];
                //Vector3 vertPointPos = geo.Points[ vertPoint ];
                Vector3 vertUV = geo.VertexAttributes[0].attr[ 3 * triIdx + triVertIdx ];

                if (!uvBuffer.ContainsKey(vertPoint))
                {
                    uvBuffer.Add(vertPoint, vertUV); // fill point uvs with first hit
                }
                else
                {
                    geo.Tris[triIdx].verts[triVertIdx] = geo.NPoints + newVertIdx;
                    newVertIdx++;
                    newPoints.Add(geo.Points[vertPoint]);
                    newNormals.Add(geo.PointAttributes[0].attr[vertPoint]);
                    newUVs.Add(vertUV);
                }


                /*
                for (uint scanIdx = 0; scanIdx < geo.NPoints; scanIdx++)
                {
                    if(geo.VertexAttributes[0].attr[ scanIdx ] != vertUV)
                    {
                        newPoints.Add(geo.Points[vertPoint]);
                        //newNormals.Add(geo.PointAttributes[0].attr[  ]);
                        //newUVs.Add(geo.VertexAttributes[0].attr[i]);
                    }
                }*/
            }
        }
        Debug.WriteLine("DuplicateVertices(): NPoints " + geo.NPoints + " uvBuffer.Count " + uvBuffer.Count);

        Array.Resize(ref geo.PointAttributes, geo.PointAttributes.Length + 1);
        geo.NPointAttrib++;

        geo.PointAttributes[geo.PointAttributes.Length - 1].name = "pointuv";
        geo.PointAttributes[geo.PointAttributes.Length - 1].size = 3;
        geo.PointAttributes[geo.PointAttributes.Length - 1].type = "float";
        geo.PointAttributes[geo.PointAttributes.Length - 1].defaultVal = new Vector3(0.0f, 0.0f, 0.0f);

        /*
        for (int i = 0; i < geo.NPointAttrib; i++)
        {
            Array.Resize(ref geo.PointAttributes[i].attr, (int)geo.NPoints);
        }*/

        geo.PointAttributes[geo.PointAttributes.Length - 1].attr = new Vector3[geo.NPoints];
        for (uint i = 0; i < geo.NPoints; i++)
        {
            geo.PointAttributes[geo.PointAttributes.Length - 1].attr[i] = uvBuffer[i];
        }
        geo.PointAttributes[geo.PointAttributes.Length - 1].attr = geo.PointAttributes[geo.PointAttributes.Length - 1].attr.Concat(newUVs.ToArray()).ToArray(); ;

        geo.NPoints += newVertIdx;
        geo.Points = geo.Points.Concat(newPoints.ToArray()).ToArray();
        geo.PointAttributes[0].attr = geo.PointAttributes[0].attr.Concat(newNormals.ToArray()).ToArray();

        Debug.WriteLine("DuplicateVertices(): new NPoints " + geo.NPoints + " Points.Length " + geo.Points.Length);
        Debug.WriteLine("DuplicateVertices(): newPoints " + newPoints.Count + " newUVs " + newUVs.Count);
        Debug.WriteLine("DuplicateVertices(): P0.L " + geo.PointAttributes[0].attr.Length + " P1.L " + geo.PointAttributes[1].attr.Length);



    }

    private void ParseGroups()
    {
        var line = "";
        while(line != null)
        {
            line = geoLines.ReadLine();
            //Debug.WriteLine(line);
        }
    }

}
