using BasicTriangle;
using BrickonAvatar;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickonEditor
{
    public class Model
    {




        public List<Vector3> Points = new List<Vector3>();
        public List<Vector3> Pointsbuf = new List<Vector3>();
        public List<Vector3> Colbuf = new List<Vector3>();
        public List<Vector3> Col = new List<Vector3>();
        public List<Vector2> tex = new List<Vector2>();
        public List<Vector2> texbuf = new List<Vector2>();

        int VertexBufferObject;
        int VertexArrayObject;
        int VertexColBuffer;
        int VertexTexBuffer;
        public Model(string file)
        { Objexster.LoadObj(file,out  Points,out Col,out tex); }

        public Model()
        {  }


        public void setup1()
        {
            VertexArrayObject = GL.GenVertexArray();

            VertexColBuffer = GL.GenBuffer();
            VertexTexBuffer = GL.GenBuffer();
            VertexBufferObject = GL.GenBuffer();

            
            GL.BindVertexArray(VertexArrayObject);

            //No clue what I did exactly but array locations might be hardcoded in the shader
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            //Set some info for the shader to know how big each buffer, how many bytes each element take, their offset, etc their kind
            //Bind the buffer we want to be used in the "slot" we specify in the next call
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //We have a vector 3 for the positions so 3 floats, not normalized, the stride is 3 because we don't store anything else in this buffer and offset 0 because we start from start
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexColBuffer);
            //We have a vector 3 for the positions so 3 floats, not normalized, the stride is 3 because we don't store anything else in this buffer and offset 0 because we start from start
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //Same but for the colors
             GL.BindBuffer(BufferTarget.ArrayBuffer, VertexTexBuffer);
             GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);

            //Some bad equivalent to GL.End
            //Bind the buffer which to be used
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //Send data to bound buffer in the specified "slot"
            //The kind of buffer, The size of the array i(the array length * size of an element), The data(because we had a list we convert it to an array), Buffer Usage, doesn't matter that much nowadays
            GL.BufferData(BufferTarget.ArrayBuffer, Points.Count* Vector3.SizeInBytes, Points.ToArray(), BufferUsageHint.StaticDraw);
            //Same thing but for colors
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexColBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Col.Count* Vector3.SizeInBytes, Col.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexTexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, tex.Count * Vector2.SizeInBytes, tex.ToArray(), BufferUsageHint.StaticDraw);

        }
        public void setup()
        {
            
            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //Send data to bound buffer in the specified "slot"
            //The kind of buffer, The size of the array i(the array length * size of an element), The data(because we had a list we convert it to an array), Buffer Usage, doesn't matter that much nowadays
            GL.BufferData(BufferTarget.ArrayBuffer, Points.Count* Vector3.SizeInBytes, Points.ToArray(), BufferUsageHint.StaticDraw);
            //Same thing but for colors
           GL.BindBuffer(BufferTarget.ArrayBuffer, VertexColBuffer);
           GL.BufferData(BufferTarget.ArrayBuffer, Col.Count* Vector3.SizeInBytes, Col.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexTexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, tex.Count * Vector2.SizeInBytes, tex.ToArray(), BufferUsageHint.StaticDraw);
        }
        public void renderModel()
        {
            GL.BindVertexArray(VertexArrayObject);
        }
        public void render()
        {
            
            GL.DrawArrays(PrimitiveType.Triangles, 0, Points.Count);
        }
    }
    public static class ModelService
    {
        public static void InstertModel(Model model,Vector3 scale,Vector3 pos,Vector3 col)
        {
            model.renderModel();
            Vector3 feetCol = col ;//new Vector3(1, 0, 1)
            GL.Uniform3(GL.GetUniformLocation(Program.ShaderProgram, "col"), ref feetCol);
            //Matrix4.CreateRotationY(Program.time) *
            Program.model = Matrix4.Identity * Matrix4.CreateScale(scale*1.7f)*  Matrix4.CreateTranslation(pos) ;
            GL.UniformMatrix4(GL.GetUniformLocation(Program.ShaderProgram, "model"), true, ref Program.model);
            model.render();
        }

    }
}
