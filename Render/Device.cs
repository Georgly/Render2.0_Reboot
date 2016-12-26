using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Render
{
    class Device
    {
        Sun sun;
        Camera camera;
        Canvas canvas;
        Model mesh;
        List<float> zBuffer;
        float Zoom;

        public Device(Canvas c)
        {
            mesh = new Model();
            sun = new Sun();
            camera = new Camera();
            canvas = c;
            zBuffer = new List<float>();//new float[(int)((canvas.ActualWidth/* + 1*/) * canvas.ActualHeight)];
            camera.Position = new Vector(0, 0, 15.0f);
            camera.Target = new Vector(0, 0, 0);
        }

        public void OpenFile(string fileName)//"обёртка" для "безопасной" работы с файлом
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader(fileName);
                Parse(file, fileName);
            }
            catch (FileNotFoundException)
            {
                Error.NoFile();
            }
            catch (IOException)
            {
                Error.EnterError();
            }
            catch (Exception)
            {
                Error.SomeError();
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        void Parse(StreamReader file, string fileName)//чтение данных(вершины, индексы вершин) из файла 3д модели
        {
            StreamReader parsFile = file;
            mesh.Vertices.Clear();
            mesh.Lines.Clear();

            double sumX = 0;
            double sumY = 0;
            double sumZ = 0;
            int count = 0;

            string textLine;
            while ((textLine = parsFile.ReadLine()) != null)
            {
                if (textLine != "")
                {
                    string[] tempt = textLine.Split(' ');
                    switch (tempt[0])
                    {
                        case "v":
                            {
                                float[] points = new float[3];
                                for (int i = 1, j = 0; i < tempt.Length; i++)
                                {
                                    if (tempt[i] != "")
                                    {
                                        points[j] = (float)Convert.ToDouble(tempt[i].Replace('.', ','));
                                        j++;
                                    }
                                }
                                Vector point = new Vector(points[0], points[1], points[2]);
                                sumX += points[0];
                                sumY += points[1];
                                sumZ += points[2];
                                count++;
                                mesh.Vertices.Add(point);
                                break;
                            }
                        case "f":
                            {
                                List<int> points = new List<int>();
                                for (int i = 1; i < tempt.Length; i++)
                                {
                                    int j = 0;
                                    string coord = "";
                                    while (j < tempt[i].Length && tempt[i][j] >= 48 && tempt[i][j] <= 57)
                                    {
                                        coord += tempt[i][j];
                                        j++;
                                    }
                                    if (coord != "")
                                    {
                                        points.Add(Convert.ToInt32(coord));
                                    }
                                }
                                mesh.AddFace(points);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            //figureCenter = new Vector(sumX / count, sumY / count, sumZ / count);
            //camera = new Vector(500, 500, 5500, figureCenter.PointVector.X, figureCenter.PointVector.Y, figureCenter.PointVector.Z);
            ////Normales of poligones in another thread
            //worker = new BackgroundWorker();
            //worker.DoWork += Worker_DoWork;
            //worker.WorkerSupportsCancellation = true;
            //worker.RunWorkerAsync();
            parsFile.Close();
        }

        void Clear()
        {
            canvas.Children.Clear();
            zBuffer.Clear();
            for (var index = 0; index < (int)((canvas.ActualWidth + 1) * canvas.ActualHeight); index++)
            {
                zBuffer.Add(float.MinValue);
            }
        }

        Vector Project(Vector coord, Matrix transMat)
        {
            // transforming the coordinates
            Vector point = Vector.TransformCoordinate(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            float x = (float)(point.X * canvas.ActualWidth + canvas.ActualWidth / 2.0f);
            float y = (float)(-point.Y * canvas.ActualHeight + canvas.ActualHeight / 2.0f);
            return (new Vector(x, y, point.Z));
        }

        void DrawPoint(Vertex point, Color color)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < canvas.ActualWidth && point.Y < canvas.ActualHeight)
            {
                Ellipse vertex = new Ellipse();
                vertex.Width = 1;
                vertex.Height = 1;
                vertex.Fill = new SolidColorBrush(color);
                vertex.Stroke = new SolidColorBrush(color);
                vertex.StrokeThickness = 0.5;
                vertex.Margin = new Thickness(point.X, point.Y, 0, 0);
                canvas.Children.Add(vertex);
            }
        }

        float Clamp(float value, float min = 0, float max = 1)
        {
            return Math.Max(min, Math.Min(value, max));
        }

        float Interpolate(float min, float max, float gradient)
        {
            return min + (max - min) * Clamp(gradient);
        }

        void ProcessScanLine(int y, Vector pa, Vector pb, Vector pc, Vector pd, Color color)//TODO не рисует некоторые грани
        {
            if (y > 0 && y < canvas.ActualHeight)
            {
                // Thanks to current Y, we can compute the gradient to compute others values like
                // the starting X (sx) and ending X (ex) to draw between
                // if pa.Y == pb.Y or pc.Y == pd.Y, gradient is forced to 1
                var gradient1 = pa.Y != pb.Y ? (y - pa.Y) / (pb.Y - pa.Y) : 1;
                var gradient2 = pc.Y != pd.Y ? (y - pc.Y) / (pd.Y - pc.Y) : 1;

                int sx = (int)Math.Max(0, Math.Min(Interpolate(pa.X, pb.X, gradient1), canvas.ActualWidth));
                int ex = (int)Math.Max(0, Math.Min(Interpolate(pc.X, pd.X, gradient2), canvas.ActualWidth));

                float z1 = Interpolate(pa.Z, pb.Z, gradient1);
                float z2 = Interpolate(pc.Z, pd.Z, gradient2);

                int stX = Math.Min(sx, ex);
                for (var x = sx; x < ex; x++)
                {
                    float gradient = (x - sx) / (float)(ex - sx);
                    float z = Interpolate(z1, z2, gradient);
                    int index = (int)(x + y * canvas.ActualWidth);
                    if (zBuffer[index/* - 1*/] > z)
                    {
                        DrawLine(sx, x, y, color);
                        while (zBuffer[index] > z && x < ex)
                        {
                            zBuffer[index] = z;
                            gradient = (x - sx) / (float)(ex - sx);
                            z = Interpolate(z1, z2, gradient);
                            index = (int)(x + y * canvas.ActualWidth);
                            x++;
                        }
                        stX = x;
                        x--;
                    }
                }
                DrawLine(stX, ex, y, color);
            }
        }

        void DrawTriangle(Vector p1, Vector p2, Vector p3, Color color)
        {
            // Sorting the points in order to always have this order on screen p1, p2 & p3
            // with p1 always up (thus having the Y the lowest possible to be near the top screen)
            // then p2 between p1 & p3
            if (p1.Y > p2.Y)
            {
                Vector temp = p2;
                p2 = p1;
                p1 = temp;
            }

            if (p2.Y > p3.Y)
            {
                Vector temp = p2;
                p2 = p3;
                p3 = temp;
            }

            if (p1.Y > p2.Y)
            {
                Vector temp = p2;
                p2 = p1;
                p1 = temp;
            }

            // inverse slopes
            float dP1P2, dP1P3;
            
            // Computing inverse slopes
            if (p2.Y - p1.Y > 0)
                dP1P2 = (p2.X - p1.X) / (p2.Y - p1.Y);
            else
                dP1P2 = 0;

            if (p3.Y - p1.Y > 0)
                dP1P3 = (p3.X - p1.X) / (p3.Y - p1.Y);
            else
                dP1P3 = 0;

            
            if (dP1P2 > dP1P3)
            {
                for (int y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        ProcessScanLine(y, p1, p3, p1, p2, color);
                    }
                    else
                    {
                        ProcessScanLine(y, p1, p3, p2, p3, color);
                    }
                }
            }
            else
            {
                for (int y = (int)p1.Y; y <= (int)p3.Y; y++)
                {
                    if (y < p2.Y)
                    {
                        ProcessScanLine(y, p1, p2, p1, p3, color);
                    }
                    else
                    {
                        ProcessScanLine(y, p2, p3, p1, p3, color);
                    }
                }
            }
        }

        public void Render(float zoom)
        {
            Zoom = zoom;
            Clear();
            Matrix transformMatrix = FindTransformMatrix(Zoom);

            DrawFaces(transformMatrix);
        }

        private static byte ColorFloatToByte(float val)
        {
            if (!(val > 0.0)) // Handles NaN case too
                return (0);
            else if (val <= 0.0031308)
                return ((byte)((255.0f * val * 12.92f) + 0.5f));
            else if (val < 1.0)
                return ((byte)((255.0f * ((1.055f *
                       (float)Math.Pow((double)val, (1.0 / 2.4))) - 0.055f)) + 0.5f));
            else
                return (255);
        }

        void DrawFaces(Matrix transformMatrix)
        {
            var faceIndex = 0;
            foreach (var face in mesh.Lines)
            {
                Vector vertexA = mesh.Vertices[face.A - 1];
                Vector vertexB = mesh.Vertices[face.B - 1];
                Vector vertexC = mesh.Vertices[face.C - 1];

                Vector pixelA = Project(vertexA, transformMatrix);
                Vector pixelB = Project(vertexB, transformMatrix);
                Vector pixelC = Project(vertexC, transformMatrix);

                var color = 0.25f + (faceIndex % mesh.Lines.Count) * 0.75f / mesh.Lines.Count;
                DrawTriangle(pixelA, pixelB, pixelC, Color.FromArgb(255, ColorFloatToByte(color), ColorFloatToByte(color), ColorFloatToByte(color))/*.FromScRgb(255, color, color, color)*/);
                faceIndex++;
            }
        }

        void DrawLine(int sx, int ex, int y, Color color)
        {
            if ((sx <= 0 && ex <= 0) || (sx >= canvas.ActualWidth && ex >= canvas.ActualWidth))
            {
                return;
            }
            Line line = new Line();
            line.Y1 = y;
            line.Y2 = y;
            line.Stroke = new SolidColorBrush(color);
            line.StrokeThickness = 1;
            line.X1 = sx;
            line.X2 = ex;
            canvas.Children.Add(line);
        }

        //public void RenderLine()
        //{
        //    Matrix transformMatrix = FindTransformMatrix();
        //    Vertex point0;
        //    Vertex point1;
        //    for (int i = 0; i < mesh.Lines.Count; i++)
        //    {
        //        point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], transformMatrix);
        //        point1 = Project(mesh.Vertices[mesh.Lines[i].B - 1], transformMatrix);
        //        DrawLine(point0, point1);
        //        point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], transformMatrix);
        //        point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], transformMatrix);
        //        DrawLine(point0, point1);
        //        point0 = Project(mesh.Vertices[mesh.Lines[i].B - 1], transformMatrix);
        //        point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], transformMatrix);
        //        DrawLine(point0, point1);
        //    }
        //}
//public void DrawLine(Vertex point0, Vertex point1)
        //{
        //    //var dist = (point1 - point0).Length();

        //    //// If the distance between the 2 points is less than 2 pixels
        //    //// We're exiting
        //    //if (dist < 2)
        //    //    return;

        //    //// Find the middle point between first & second point
        //    //Vertex middlePoint = point0 + (point1 - point0) / 2.0;
        //    //// We draw this point on screen
        //    //DrawPoint(middlePoint);
        //    //// Recursive algorithm launched between first & middle point
        //    //// and between middle & second point
        //    //DrawLine(point0, middlePoint);
        //    //DrawLine(middlePoint, point1);
        //    Line line = new Line();
        //    line.X1 = point0.X;
        //    line.X2 = point1.X;
        //    line.Y1 = point0.Y;
        //    line.Y2 = point1.Y;
        //    line.Stroke = Brushes.GreenYellow;
        //    line.StrokeThickness = 1;
        //    canvas.Children.Add(line);
        //}

        Matrix FindTransformMatrix(float zoom)
        {
            Matrix viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, /*Vector.UnitY()*/new Vector(0, 1, 0));
            Matrix projectionMatrix = Matrix.PerspectiveFovRH(/*0.78f*/90, (float)(canvas.ActualWidth / canvas.ActualHeight), 0.1f, 100.0f);
            
            Matrix worldMatrix = Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) * Matrix.Translation(mesh.Position) * Matrix.Scale(zoom);

            return worldMatrix * viewMatrix * projectionMatrix;
        }

        //Matrix ZoomTransformMatrix(float zoom)
        //{
        //    Matrix viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, /*Vector.UnitY()*/new Vector(0, 1, 0));
        //    Matrix projectionMatrix = Matrix.PerspectiveFovRH(/*0.78f*/90, (float)(canvas.ActualWidth / canvas.ActualHeight), 0.1f, 100.0f);

        //    Matrix worldMatrix = Matrix.Translation(mesh.Position) * Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z) * Matrix.Scale(zoom);

        //    return worldMatrix * viewMatrix * projectionMatrix;
        //}

        public void ZoomModel(/*int clientCount,*/ float zoom)
        {
            Clear();
            Zoom = zoom;
            Matrix m = /*ZoomTransformMatrix(zoom)*/FindTransformMatrix(Zoom)/* * Matrix.Scale(zoom)*/;
            DrawFaces(m);
            //switch (clientCount)
            //{
            //    case 1:
            //        {
            //            foreach (Vector vertex in mesh.Vertices)
            //            {
            //                // project the 3D coordinates into the 2D space
            //                Vertex point = Project(vertex, m);
            //                DrawPoint(point);
            //            }
            //            break;
            //        }
            //    case 2:
            //        {
            //            //Vertex point0;
            //            //Vertex point1;
            //            //for (int i = 0; i < mesh.Lines.Count; i++)
            //            //{
            //            //    point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], m);
            //            //    point1 = Project(mesh.Vertices[mesh.Lines[i].B - 1], m);
            //            //    DrawLine(point0, point1);
            //            //    point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], m);
            //            //    point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], m);
            //            //    DrawLine(point0, point1);
            //            //    point0 = Project(mesh.Vertices[mesh.Lines[i].B - 1], m);
            //            //    point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], m);
            //            //    DrawLine(point0, point1);
            //            //}
            //            break;
            //        }
            //    //case 3:
            //    //    {
            //    //        for (int i = 0; i < _vertexes.Count; i++)
            //    //        {
            //    //            _vertexes[i].Zoom();
            //    //        }
            //    //        for (int i = 0; i < _poligones.Count; i++)
            //    //        {
            //    //            _poligones[i]._sunVect = sun;
            //    //            _poligones[i].Zoom(_vertexes);
            //    //        }
            //    //        break;
            //    //    }
            //    default:
            //        break;
            //}
        }

        public void Rotation(int x, int y, int z)
        {
            Clear();
            mesh.Rotation = new Vector(mesh.Rotation.X + x / 50.0f, mesh.Rotation.Y + y / 50.0f, mesh.Rotation.Z + z / 50.0f);
            Matrix m = FindTransformMatrix(Zoom)/* * Matrix.Rotate(x, y, z)*/;
            DrawFaces(m);
        }
    }
}
