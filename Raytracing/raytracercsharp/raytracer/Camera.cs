using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    class Camera
    {
        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 forward = new Vector3(0, 0, 1);
        public Vector3 right = new Vector3(1, 0, 0);
        public Vector3 up = new Vector3(0, 1, 0);
        public bool orthographic = false;
        public float orthoWidth = 5.0f;
        public float orthoHeight = 5.0f;
        public float fov = 45.0f / 57.29f;

        public void lookAt(Vector3 target)
        {
            forward = (target - position).normalised();
            right = new Vector3(0.0f, 1.0f, 0.0f).cross(forward).normalised();
            up = forward.cross(right).normalised();
        }

        public Ray viewToRay(float x, float y)
        {
            Ray ray = new Ray();
            if (orthographic)
            {
                ray.start = position + right * x * orthoWidth + up * y * orthoHeight;
                ray.delta = forward;
            }
            else
            {
                float phi = (-y * fov + 3.14159265f / 2.0f);
                float theta = (-x * fov + 3.14159265f / 2.0f);
                ray.start = position;
                float ax = (float)(Math.Sin(phi) * Math.Cos(theta));
                float ay = (float)(Math.Cos(phi));
                float az = (float)(Math.Sin(phi) * Math.Sin(theta));
                ray.delta = (ax * right + ay * up + az * forward).normalised();
            }
            return ray;
        }
    }
}
