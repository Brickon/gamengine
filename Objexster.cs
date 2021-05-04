using BrickonEditor;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickonAvatar
{
    public static class Objexster

    {
        public static void LoadObj(string file,out List<Vector3> Points,out List<Vector3>norm ,out List<Vector2> texdt)
        {
            Model model = new Model();
            IEnumerable<string> lines = File.ReadLines(file);

            foreach (string line in lines)
            {
                if (line.Length == 0)
                {
                    continue;
                }

                char firstChar = line[0];
                switch (firstChar)
                {
                    case '#': // It's an OBJ Comment
                        break;

                    case 'v':
                        {
                            switch (line[1])
                            {
                                case ' ':
                                    {
                                        //Vertex position
                                        string[] raw = line.Substring(2).Split(' ');
                                        Vector3 position = new Vector3(
                                          float.Parse(raw[0].Replace(".",",")), //x
                                          float.Parse(raw[1].Replace(".", ",")), //y
                                          float.Parse(raw[2].Replace(".", ",")) //z
                                        );
                                        model.Pointsbuf.Add(position);
                                    }
                                    break;

                                case 'n':
                                    {
                                        //Vertex position

                                        string[] raw = line.Substring(3).Split(' ');

                                        Vector3 normal = new Vector3
                                        (
                                          float.Parse(raw[0].Replace(".", ",")), //x
                                          float.Parse(raw[1].Replace(".", ",")), //y
                                          float.Parse(raw[2].Replace(".", ",")) //z
                                          
                                        );

                                        model.Colbuf.Add(normal);
                                    }
                                    break;

                                case 't':
                                    {
                                        //Texture coords
                                        string[] raw = line.Substring(3).Split(' ');
                                        Vector2 uv = new Vector2
                                        (
                                            float.Parse(raw[0].Replace(".", ",")),     //x
                                            -float.Parse(raw[1].Replace(".", ","))      //y
                                        );
                                        model.texbuf.Add(uv);
                                    }break;

                            }
                            break;

                        }
                    //It's a vertex
                    case 'f':
                        {

                            string[] indices = line.Substring(2).Split();
                            foreach (string index in indices)
                            {
                                string[] raw = index.Split('/');

                                Vector3 Position = model.Pointsbuf[int.Parse(raw[0]) - 1];
                                Vector2 tex = model.texbuf[int.Parse(raw[1]) - 1];
                                Vector3 Normal = model.Colbuf[int.Parse(raw[2]) - 1];
                                // Console.WriteLine(Position.X);
                                /* TextureCoordinates = currentModel.Uvs[int.Parse(raw[1]) - 1],
                                 Normal = currentModel.Normals[int.Parse(raw[2]) - 1]*/
                                model.Points.Add(Position);
                                model.tex.Add(tex);
                                model.Col.Add(Normal);

                            }
                        }
                        break;

                        // }

                }
            }
            Points = model.Points;
            texdt = model.tex;
            norm = model.Col;
        }
    }
}