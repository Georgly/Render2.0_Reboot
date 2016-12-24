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

        public Device(Canvas c)
        {
            mesh = new Model();
            sun = new Sun();
            camera = new Camera();
            canvas = c;
            camera.Position = new Vector(0, 0, 10.0f);
            camera.Target = new Vector(0, 0, 0);
        }

        public void OpenFile(string fileName)
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

        void Parse(StreamReader file, string fileName)
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

        public Vertex Project(Vector coord, Matrix transMat)
        {
            // transforming the coordinates
            Vector point = Vector.TransformCoordinate(coord, transMat);
            // The transformed coordinates will be based on coordinate system
            // starting on the center of the screen. But drawing on screen normally starts
            // from top left. We then need to transform them again to have x:0, y:0 on top left.
            var x = point.X * canvas.ActualWidth + canvas.ActualWidth / 2.0f;
            var y = -point.Y * canvas.ActualHeight + canvas.ActualHeight / 2.0f;
            return (new Vertex(x, y));
        }

        public void DrawPoint(Vertex point)
        {
            // Clipping what's visible on screen
            if (point.X >= 0 && point.Y >= 0 && point.X < canvas.ActualWidth && point.Y < canvas.ActualHeight)
            {
                Ellipse vertex = new Ellipse();
                vertex.Width = 1;
                vertex.Height = 1;
                vertex.Fill = Brushes.YellowGreen;
                vertex.Stroke = Brushes.YellowGreen;
                vertex.StrokeThickness = 0.5;
                vertex.Margin = new Thickness(point.X, point.Y, 0, 0);
                canvas.Children.Add(vertex);
            }
        }

        public void DrawLine(Vertex point0, Vertex point1)
        {
            var dist = (point1 - point0).Length();

            // If the distance between the 2 points is less than 2 pixels
            // We're exiting
            if (dist < 2)
                return;

            // Find the middle point between first & second point
            Vertex middlePoint = point0 + (point1 - point0) / 2.0;
            // We draw this point on screen
            DrawPoint(middlePoint);
            // Recursive algorithm launched between first & middle point
            // and between middle & second point
            DrawLine(point0, middlePoint);
            DrawLine(middlePoint, point1);
        }

        public void RenderPoint()
        {
            Matrix transformMatrix = FindTransformMatrix();
            foreach (Vector vertex in mesh.Vertices)
            {
                // project the 3D coordinates into the 2D space
                Vertex point = Project(vertex, transformMatrix);
                DrawPoint(point);
            }
            //}
        }

        public void RenderLine()
        {
            Matrix transformMatrix = FindTransformMatrix();
            Vertex point0;
            Vertex point1;
            for (int i = 0; i < mesh.Lines.Count; i++)
            {
                point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], transformMatrix);
                point1 = Project(mesh.Vertices[mesh.Lines[i].B - 1], transformMatrix);
                DrawLine(point0, point1);
                point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], transformMatrix);
                point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], transformMatrix);
                DrawLine(point0, point1);
                point0 = Project(mesh.Vertices[mesh.Lines[i].B - 1], transformMatrix);
                point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], transformMatrix);
                DrawLine(point0, point1);
            }
        }

        Matrix FindTransformMatrix()
        {
            Matrix viewMatrix = Matrix.LookAtLH(camera.Position, camera.Target, /*Vector.UnitY()*/new Vector(0, 1, 0));
            Matrix projectionMatrix = Matrix.PerspectiveFovRH(90, (float)(canvas.ActualWidth / canvas.ActualHeight), 0.1f, 100.0f);
            
            Matrix worldMatrix = Matrix.Translation(mesh.Position) * Matrix.RotationYawPitchRoll(mesh.Rotation.Y, mesh.Rotation.X, mesh.Rotation.Z);

            return worldMatrix * viewMatrix * projectionMatrix;
        }

        public void Zoom(int clientCount, float zoom)
        {
            Matrix m = FindTransformMatrix() * Matrix.Scale(zoom);
            switch (clientCount)
            {
                case 1:
                    {
                        foreach (Vector vertex in mesh.Vertices)
                        {
                            // project the 3D coordinates into the 2D space
                            Vertex point = Project(vertex, m);
                            DrawPoint(point);
                        }
                        break;
                    }
                case 2:
                    {
                        Vertex point0;
                        Vertex point1;
                        for (int i = 0; i < mesh.Lines.Count; i++)
                        {
                            point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], m);
                            point1 = Project(mesh.Vertices[mesh.Lines[i].B - 1], m);
                            DrawLine(point0, point1);
                            point0 = Project(mesh.Vertices[mesh.Lines[i].A - 1], m);
                            point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], m);
                            DrawLine(point0, point1);
                            point0 = Project(mesh.Vertices[mesh.Lines[i].B - 1], m);
                            point1 = Project(mesh.Vertices[mesh.Lines[i].C - 1], m);
                            DrawLine(point0, point1);
                        }
                        break;
                    }
                //case 3:
                //    {
                //        for (int i = 0; i < _vertexes.Count; i++)
                //        {
                //            _vertexes[i].Zoom();
                //        }
                //        for (int i = 0; i < _poligones.Count; i++)
                //        {
                //            _poligones[i]._sunVect = sun;
                //            _poligones[i].Zoom(_vertexes);
                //        }
                //        break;
                //    }
                default:
                    break;
            }
        }
    }
}
