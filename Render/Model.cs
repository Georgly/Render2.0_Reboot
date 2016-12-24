using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public struct Face
    {
        public int A;
        public int B;
        public int C;
    }

    class Model
    {
        //public string Name { get; set; } // maybe i will use this field, but not now
        public List<Vector> Vertices { get; private set; }
        public List<Face> Lines { get; set; }
        public Vector Position { get; set; }
        public Vector Rotation { get; set; }

        public Model(/*string name, int verticesCount*/)
        {
            Vertices = new List<Vector>();
            Lines = new List<Face>();
            //Name = name;
        }

        public void AddFace(List<int> points)
        {
            Face f = new Face();
            f.A = points[0];
            f.B = points[1];
            f.C = points[2];
            Lines.Add(f);
            if (points.Count > 3)
            {
                Face f2 = new Face();
                f2.A = points[2];
                f2.B = points[3];
                f2.C = points[0];
                Lines.Add(f2);
            }
        }
    }
}
