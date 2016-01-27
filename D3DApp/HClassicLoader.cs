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

struct HPointAttribute
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
    public HPointAttribute[] PointAttributes;
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
            geo.PointAttributes = new HPointAttribute[geo.NPointAttrib];
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
            }
        }
        return true;
    }

    private bool ParsePrims()
    {
        var dummyline = geoLines.ReadLine();
        Debug.WriteLine(dummyline); //should be Run xxxx Poly

        geo.Tris = new tri[geo.NPrims];
        for (int i = 0; i < geo.NPrims; i++)
        {
            var bigline = geoLines.ReadLine();
            var line = bigline.Split();
            //Debug.WriteLine(i + " : " + bigline + " 0: " + line[1]);
            if(!line[1].Equals("3"))
            {
                Debug.WriteLine("Only triangles supported, sorry");
                return false;
            }
            uint i0 = Convert.ToUInt32(line[3], CultureInfo.InvariantCulture);
            uint i1 = Convert.ToUInt32(line[4], CultureInfo.InvariantCulture);
            uint i2 = Convert.ToUInt32(line[5], CultureInfo.InvariantCulture);
            geo.Tris[i] = new tri(i0, i1, i2);
        }
        return true;
    }

    private void ParseGroups()
    {
        var line = "";
        while(line != null)
        {
            line = geoLines.ReadLine();
            Debug.WriteLine(line);
        }
    }

}
