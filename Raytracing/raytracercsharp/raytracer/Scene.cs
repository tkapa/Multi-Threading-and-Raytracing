using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    class Scene
    {
        public List<Renderable> renderables = new List<Renderable>();
        public List<Light> lights = new List<Light>();
        public Vector3 ambientLight;
        public Vector3 skyColour = new Vector3(0.1f, 0.3f, 0.8f);
        public Camera camera = new Camera();
        public bool shadows = true;
        public int recurseLimit = 4;
        public SFML.Graphics.Image skyTexture;

        public HitPoint intersect(Ray ray)
        {
            HitPoint hp = new HitPoint();
            foreach (Renderable r in renderables)
            {
                hp.nearest(r.intersect(ray));
            }
            return hp;
        }

        public Vector3 trace(float x, float y)
        {
            Ray ray = camera.viewToRay(x, y);
            return trace(ray, 0);
        }

        float remap(float src_range_start, float src_range_end, float dst_range_start, float dst_range_end, float value_to_remap)
        {
            return ((dst_range_end - dst_range_start) * (value_to_remap - src_range_start)) / (src_range_end - src_range_start) + dst_range_start;
        }

        public Vector3 trace(Ray ray, int recurseDepth)
        {
            Vector3 output = skyColour;
            if (recurseDepth > recurseLimit)
            {
                output.set(0, 0, 0);
                return output;
            }

            if (skyTexture != null)
            {
                // Get the colour of the skybox that this ray would hit.
                int tx = (int)(remap(-3.14159265f, 3.14159265f, 0, skyTexture.Size.X - 1.0f, (float)Math.Atan2(ray.delta.x, ray.delta.z)));
                int ty = (int)(remap(1.0f, -1.0f, 0, skyTexture.Size.Y - 1.0f, ray.delta.y));
                SFML.Graphics.Color c = skyTexture.GetPixel((uint)tx, (uint)ty);
                output.x = c.R / 255.0f;
                output.y = c.G / 255.0f;
                output.z = c.B / 255.0f;
            }

            HitPoint hp = intersect(ray);
            if (hp.hit)
            {
                Vector3 specColour = new Vector3(0, 0, 0);
                Vector3 diffuseColour = new Vector3(0, 0, 0);

                // Loop through every light in the scene.
                foreach (Light light in lights)
                {
                    // Direction and distance from the intersect point to the current light.
                    Vector3 lightDirection = light.position - hp.position;
                    float lightDistance = lightDirection.normalise();

                    Vector3 lightColour = light.colour;
                    if (shadows)
                    {
                        // Cast a ray from the intersect point to each light. If anything is in the way, a shadow is being cast on the intersect point.
                        Ray shadowRay = new Ray();
                        shadowRay.start = hp.position + lightDirection * 0.001f; // The lightDirection*0.001f bit is to stop floating point error making the ray collide with it's starting point.
                        shadowRay.delta = lightDirection;
                        HitPoint shadowhp = intersect(shadowRay);

                        // If a shadow is detected, the light's contribution is zero.
                        if (shadowhp.hit && shadowhp.distance < lightDistance)
                        {
                            lightColour.set(0, 0, 0);
                        }
                    }

                    // Calculate standard lambert diffuse lighting
                    float lambert = hp.normal.dot(lightDirection);
                    if (lambert < 0)
                        lambert = 0;

                    // Accumulate diffuse lighting.
                    diffuseColour = diffuseColour + hp.renderable.material.diffuse * lightColour * lambert;

                    // Accumulate specular lighting.
                    Vector3 rl = lightDirection.reflect(hp.normal);
                    float specular = rl.dot(ray.delta);
                    if (specular > 0)
                    {
                        specColour += hp.renderable.material.specular * light.colour * (float)Math.Pow(specular, hp.renderable.material.specularPower);
                    }
                }

                Vector3 reflectionColour = new Vector3();
                Ray reflectionRay = new Ray();
                // If the hit object's material has reflection, cast a new ray recursively.
                if (hp.renderable.material.reflective.length() > 0)
                {
                    reflectionRay.delta = (ray.delta.reflect(hp.normal)).normalised();
                    reflectionRay.start = hp.position + reflectionRay.delta * 0.001f;
                    reflectionColour = trace(reflectionRay, recurseDepth + 1) * hp.renderable.material.reflective;
                }

                // Final output is diffuse + specular + reflection.
                output = ambientLight + diffuseColour + specColour + reflectionColour;
            }
            return output;
        }
    }
}
