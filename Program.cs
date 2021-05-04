using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using BrickonEditor;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace BasicTriangle
{
    sealed class Program : GameWindow
    {
        public Program()
: base(500, 500, GraphicsMode.Default, "Brickon Client", GameWindowFlags.Default, DisplayDevice.Default, 4, 5, GraphicsContextFlags.Default)
        {
            VSync = VSyncMode.Adaptive;
           X = 55555;


        }



        // A simple vertex shader possible. Just passes through the position vector.
        const string VertexShaderSource = @"
            #version 330 core

            layout(location = 0) in vec4 position;
            layout(location = 1) in vec3 normaldata;
            layout(location = 2) in vec2 texinf;
            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;
            out vec3 normal;
            out vec3 fragPos;
            out vec2 texCoords;
            void main(void)
            {
                normal = normaldata;
                texCoords = texinf;
                
                gl_Position = position* model  * view *projection;
fragPos = vec3(model*position);
            }
        ";

        // A simple fragment shader. Just a constant red color.
        const string FragmentShaderSource = @"
            #version 330 core

            out vec4 outputColor;

           in vec3 normal;
            in vec3 fragPos;
            uniform vec3 col;
            uniform sampler2D texdt;
            uniform int useTexture;
            in vec2 texCoords;
            void main(void)
            {
vec3 norm = normalize(normal);
vec3 lightDir = normalize(vec3(0,1,55)); 
float diff = max(dot(fragPos+0.5f, lightDir), -0.1);
vec3 diffuse = vec3(diff) ;
if(useTexture == 1){
                outputColor = texture(texdt, texCoords)*vec4(col*diffuse,1.0);
}
if(useTexture == 0){
                outputColor = vec4(col*diffuse,1.0);
}
            }
        ";
        int facedt = 0;
        Texture.Texture face;

        // Points of a triangle in normalized device coordinates.
        List<Vector3> Points = new List<Vector3>();
        List<Vector4> Col = new List<Vector4>();
        int VertexShader;
        int FragmentShader;
        public static int ShaderProgram;
        int VertexBufferObject;
        int VertexArrayObject;
        int VertexColBuffer;

        Model arm1 = new Model("arm1.obj");
        Model arm = new Model("arm.obj");
        Model torso = new Model("torso.obj");
        Model head = new Model("head.obj");
        Model leg1 = new Model("leg1.obj");
        Model leg = new Model("leg.obj");
        
        protected override void OnLoad(EventArgs e)
        {
            face = new Texture.Texture("face.png", out facedt);
            //Brickon.Model.Model.Load("leg.3d", out Points, out Col);
            // Points.Add(new Vector3(0, 1, 0));

            // Load the source of the vertex shader and compile it.
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);

            // Load the source of the fragment shader and compile it.
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);

            // Create the shader program, attach the vertex and fragment shaders and link the program.
            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);
            model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(1));
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(55f), this.Size.Width / (float)this.Size.Height, 0.1f, 100.0f);
            view = Matrix4.CreateTranslation(0.0f, 0.0f, -3.0f);

            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "projection"), true, ref projection);

            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "model"), true, ref model);

            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "view"), true, ref view);

            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "texdt"), facedt);

            // Create the vertex buffer object (VBO) for the vertex data.
            VertexBufferObject = GL.GenBuffer();
            VertexColBuffer = GL.GenBuffer();
            // Bind the VBO and copy the vertex data into it.
            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //Send data to bound buffer in the specified "slot"
            //The kind of buffer, The size of the array i(the array length * size of an element), The data(because we had a list we convert it to an array), Buffer Usage, doesn't matter that much nowadays
            GL.BufferData(BufferTarget.ArrayBuffer, Points.Count * Vector3.SizeInBytes, Points.ToArray(), BufferUsageHint.StaticDraw);
            //Same thing but for colors
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexColBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Col.Count * Vector4.SizeInBytes, Col.ToArray(), BufferUsageHint.StaticDraw);


            // Retrive the position location from the program.
            var positionLocation = GL.GetAttribLocation(ShaderProgram, "position");
            var colLocation = GL.GetAttribLocation(ShaderProgram, "coldt");
            // Create the vertex array object (VAO) for the program.
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            //No clue what I did exactly but array locations might be hardcoded in the shader
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            //Set some info for the shader to know how big each buffer, how many bytes each element take, their offset, etc their kind
            //Bind the buffer we want to be used in the "slot" we specify in the next call
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //We have a vector 3 for the positions so 3 floats, not normalized, the stride is 3 because we don't store anything else in this buffer and offset 0 because we start from start
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            //Same but for the colors
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexColBuffer);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 4 * sizeof(float), 0);

            //Some bad equivalent to GL.End
            //Bind the buffer which to be used
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //Send data to bound buffer in the specified "slot"
            //The kind of buffer, The size of the array i(the array length * size of an element), The data(because we had a list we convert it to an array), Buffer Usage, doesn't matter that much nowadays
            GL.BufferData(BufferTarget.ArrayBuffer, Points.Count * Vector3.SizeInBytes, Points.ToArray(), BufferUsageHint.StaticDraw);
            //Same thing but for colors
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexColBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, Col.Count * Vector4.SizeInBytes, Col.ToArray(), BufferUsageHint.StaticDraw);
            // Set the clear color to blue

            arm1.setup1();
            arm.setup1();
            torso.setup1();
            head.setup1();
            leg1.setup1();
            leg.setup1();

            GL.ClearColor(0.0f, 0.0f, 1.0f, 1.0f);



            Console.WriteLine(GL.GetShaderInfoLog(VertexShader));
            Console.WriteLine(GL.GetShaderInfoLog(FragmentShader));


            base.OnLoad(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            // Unbind all the resources by binding the targets to 0/null.
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            // Delete all the resources.
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteProgram(ShaderProgram);
            GL.DeleteShader(FragmentShader);
            GL.DeleteShader(VertexShader);

            base.OnUnload(e);
        }

        protected override void OnResize(EventArgs e)
        {
            // Resize the viewport to match the window size.
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        Matrix4 projection;
        Matrix4 view;
        public static Matrix4 model;
        public Bitmap TakeScreenshot()
        {
 
            int w = ClientSize.Width;
            int h = ClientSize.Height;
            Bitmap bmp = new Bitmap(w, h);
            System.Drawing.Imaging.BitmapData data =
                bmp.LockBits(ClientRectangle, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, w, h, OpenTK.Graphics.OpenGL4.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }
       public static int time = 0;
        public static string faceurl = "faceurl";
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the color buffer.
            time++;
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit |ClearBufferMask.DepthBufferBit);

            arm1.setup();
            arm.setup();
            torso.setup();
            head.setup();
            leg1.setup();
            leg.setup();

            //Mouse.GetState().Y
            var model = Matrix4.Identity ;
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(55f), this.Size.Width / (float)this.Size.Height, 0.1f, 100.0f);
            view = Matrix4.CreateRotationY((float)MathHelper.DegreesToRadians(15)) * Matrix4.CreateTranslation(-0.1f, -5.0f, -8.0f) ; //Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(35))  * 
            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "projection"), true, ref projection)  ;

            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "model"), true, ref model);

            GL.UniformMatrix4(GL.GetUniformLocation(ShaderProgram, "view"), true, ref view);
            // Bind the VBO
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);

           
            // Bind the VAO
            GL.BindVertexArray(VertexArrayObject);
            // Use/Bind the program
            GL.UseProgram(ShaderProgram);
            // This draws the triangle.
            // GL.DrawArrays(PrimitiveType.Quads, 0, Points.Count);
            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "useTexture"), 0);
            ModelService.InstertModel(arm1, new Vector3(1, 1, 1), new Vector3(0.5f, -0.5f, -5.5f), rightHandCol / 255);
            ModelService.InstertModel(arm, new Vector3(1, 1, 1), new Vector3(0.5f, -0.5f, -5.5f), leftHandCol / 255);
            ModelService.InstertModel(torso, new Vector3(1.1f, 0.85f, 1.1f), new Vector3(0.5f, 0f, -5.35f), torsoCol / 255);
            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "useTexture"), 1);
            //  GL.Enable(EnableCap.Texture2D);
            ModelService.InstertModel(head, new Vector3(1, 1, 1), new Vector3(0.5f, -0.5f, -5.5f), new Vector3(1, 1, 1));
            GL.Uniform1(GL.GetUniformLocation(ShaderProgram, "useTexture"), 0);
            // GL.Disable(EnableCap.Texture2D);
            ModelService.InstertModel(leg1, new Vector3(1, 0.85f, 1), new Vector3(0.5f, -0.30f, -5.5f), leftLegCol / 255);
            ModelService.InstertModel(leg, new Vector3(1, 0.85f, 1), new Vector3(0.5f, -0.30f, -5.5f), rightLegCol/ 255);
            Console.WriteLine(arm1.Points.Count);
            // Swap the front/back buffers so what we just rendered to the back buffer is displayed in the window.
            Context.SwapBuffers();
           // Console.WriteLine(Mouse.GetState().X);
            if(time>10)
             {

                 var bitmap = TakeScreenshot();
                 bitmap.MakeTransparent(Color.Blue);
               Bitmap bmp = (Bitmap)cropAtRect(bitmap, new Rectangle(115, 215, 315, 285));
               // bitmap = bmp;
                     bmp.Save(tosave,ImageFormat.Png);
            Exit();
             }
            base.OnRenderFrame(e);
        }
        public static Bitmap cropAtRect( Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            using (Graphics g = Graphics.FromImage(nb))
            {
                g.DrawImage(b, -r.X, -r.Y);
                return nb;
            }
        }
        private static Image crop(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }
        public Vector3 leftHandCol = new Vector3();
        public Vector3 rightHandCol = new Vector3();
        public Vector3 rightLegCol = new Vector3();
        public Vector3 leftLegCol = new Vector3();
        public Vector3 torsoCol = new Vector3();
        public string tosave = "bruh.png";
        [STAThread]
        static void Main(string[] args)
        {
            var program = new Program();
            if(args.Length>1)
            {
                if (args[0] == "left_arm")
            {

                    //after receiving hex col information lets convert them  and divide by / 255 to prevent issues
                    Color larm_col = ColorTranslator.FromHtml("#"+args[1]);
                    program.leftHandCol.X = larm_col.R ;
                    program.leftHandCol.Y = larm_col.G ;
                    program.leftHandCol.Z = larm_col.B ;


                    /*program.leftHandCol.X = Single.Parse(args[1]);
                    program.leftHandCol.Y = Single.Parse(args[1+1]);
                    program.leftHandCol.Z = Single.Parse(args[1+1+1]);*/
                }
                if (args[1+1] == "right_arm")
                {

                    Color rarm_col = ColorTranslator.FromHtml("#" + args[1+1+1]);
                    program.rightHandCol.X = rarm_col.R ;
                    program.rightHandCol.Y = rarm_col.G ;
                    program.rightHandCol.Z = rarm_col.B ;
                    program.tosave = args[1+1+1+1]+".png";
                }
                if (args[1 + 1+1+1+1] == "right_leg")
                {

                    Color rleg_col = ColorTranslator.FromHtml("#" + args[1 + 1 + 1+1+1+1]);
                    program.rightLegCol.X = rleg_col.R;
                    program.rightLegCol.Y = rleg_col.G;
                    program.rightLegCol.Z = rleg_col.B;
                    
                }
                if (args[1 + 1 + 1 + 1 + 1 + 1+1] == "left_leg")
                {

                    Color lleg_col = ColorTranslator.FromHtml("#" + args[1 + 1 + 1 + 1 + 1 + 1+1+1]);
                    program.leftLegCol.X = lleg_col.R;
                    program.leftLegCol.Y = lleg_col.G;
                    program.leftLegCol.Z = lleg_col.B;

                }
                if (args[1 + 1 + 1 + 1 + 1 + 1 + 1+1+1] == "torso")
                {

                    Color torso_col = ColorTranslator.FromHtml("#" + args[1 + 1 + 1 + 1 + 1 + 1 + 1 + 1+1+1]);
                    program.torsoCol.X = torso_col.R;
                    program.torsoCol.Y = torso_col.G;
                    program.torsoCol.Z = torso_col.B;

                }
            }
            program.Run();
        }
    }
}