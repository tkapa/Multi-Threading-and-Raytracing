using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace raytracer
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] screenBuffer;
            int screenWidth = 1024;
            int screenHeight = 1024;
            bool progressiveDisplay = true;
            int progressiveCount = 10;

            screenBuffer = new byte[screenWidth * screenHeight * 4];

            ContextSettings contextSettings = new ContextSettings();
            contextSettings.DepthBits = 32;

            // Create the main window
            RenderWindow window = new RenderWindow(new VideoMode((uint)screenWidth, (uint)screenHeight), "SFML graphics with OpenGL", Styles.Default, contextSettings);
            window.SetVerticalSyncEnabled(false);
            window.Closed += new EventHandler(OnClosed);

            Texture screenTex = new Texture((uint)screenWidth, (uint)screenHeight);
            Sprite screenSprite = new Sprite(screenTex);

            Scene scene = new Scene();
            scene.skyColour.set(0.1f, 0.3f, 0.8f);
            scene.skyTexture = new Image("KKZ5V.bmp");
            scene.camera.position.set(0.0f, 0.0f, -10.0f);
            scene.camera.lookAt(new Vector3(0.0f, 0.0f, 0.0f));
            scene.camera.fov = 0.5f;

            Light light1 = new Light();
            light1.position.set(4.0f, 15.0f, 0.0f);
            light1.colour.set(1.0f, 1.0f, 1.0f);
            scene.lights.Add(light1);

            Light light2 = new Light();
            light2.position.set(-10.0f, 3.0f, -3.0f);
            light2.colour.set(0.7f, 0.01f, 0.01f);
            scene.lights.Add(light2);

            for (float angle = 0; angle <= 1080.0f; angle += 20)
            {
                Sphere s = new Sphere();
                s.position.set((float)Math.Cos(angle / 57.29f) * 2.0f, angle / 170.0f - 4.0f, (float)Math.Sin(angle / 57.29) * 2.0f);
                s.radius = 0.8f;
                Material m = new Material();
                m.diffuse.set(0.3f, 0.3f, 0.3f);
                m.specular.set(1.0f, 1.0f, 1.0f);
                m.specularPower = 10.0f;
                m.reflective.set(0.7f, 0.7f, 0.7f);
                s.material = m;
                scene.renderables.Add(s);
            }
            for (float gridY = 0; gridY < 30; ++gridY)
            {
                for (float gridX = 0; gridX < 30; ++gridX)
                {
                    Sphere s = new Sphere();
                    s.position.set((gridX - 15.0f) * 0.8f, -3.0f, (gridY - 15.0f) * 0.8f);
                    s.radius = 0.4f;
                    Material m = new Material();
                    m.diffuse.set(0.3f, 0.8f, 0.3f);
                    m.specular.set(0.2f, 0.2f, 0.2f);
                    m.specularPower = 4.0f;
                    m.reflective.set(0.05f, 0.05f, 0.05f);
                    s.material = m;
                    scene.renderables.Add(s);
                }
            }



            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            for (int y = 0; y < screenHeight; y++)
            {
                for (int x = 0; x < screenWidth; ++x)
                {
                    Vector3 output = scene.trace((x - screenWidth / 2.0f) / (screenWidth / 2.0f), -(y - screenHeight / 2.0f) / (screenHeight / 2.0f) * (((float)(screenHeight) / screenWidth)));
                    output.x = Math.Min(output.x, 1.0f);
                    output.y = Math.Min(output.y, 1.0f);
                    output.z = Math.Min(output.z, 1.0f);

                    screenBuffer[x * 4 + y * screenWidth * 4 + 0] = (byte)(255 * output.x);
                    screenBuffer[x * 4 + y * screenWidth * 4 + 1] = (byte)(255 * output.y);
                    screenBuffer[x * 4 + y * screenWidth * 4 + 2] = (byte)(255 * output.z);
                    screenBuffer[x * 4 + y * screenWidth * 4 + 3] = 0xff;
                }
                //Console.WriteLine(y);
                if (progressiveDisplay)
                {
                    if (y % progressiveCount == 0)
                    {
                        screenTex.Update(screenBuffer);
                        window.Draw(screenSprite);
                        window.Display();
                    }
                }
            }
            watch.Stop();
            window.SetTitle(watch.Elapsed.ToString());
            screenTex.Update(screenBuffer);
            Image save = screenTex.CopyToImage();
            save.SaveToFile("output.png");
            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear();
                window.Draw(screenSprite);
                window.Display();
            }
        }

        static void OnClosed(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }
    }
}
