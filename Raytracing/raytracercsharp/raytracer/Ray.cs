using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    class Ray
    {
        public Vector3 start;
        public Vector3 delta;

        public Ray()
        {

        }

        public Ray(Vector3 s, Vector3 d)
        {
            start = s;
            delta = d;
        }

        public Vector3 interpolate(float value)
        {
            return start + delta * value;
        }
    }
}
