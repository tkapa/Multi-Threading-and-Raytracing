using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    class HitPoint
    {
        public Vector3 normal;
        public Vector3 tangent;
        public Vector3 binormal;
        public Vector3 position;
        public Ray ray;
        public float distance = 0.0f;
        public bool hit = false;
        public Renderable renderable;

        public void nearest(HitPoint hp)
        {
            if (!hit)
            {
                normal = hp.normal;
                tangent = hp.tangent;
                binormal = hp.binormal;
                position = hp.position;
                ray = hp.ray;
                distance = hp.distance;
                hit = hp.hit;
                renderable = hp.renderable;
            }
            else
            {
                if (hp.hit && hp.distance < distance)
                {
                    normal = hp.normal;
                    tangent = hp.tangent;
                    binormal = hp.binormal;
                    position = hp.position;
                    ray = hp.ray;
                    distance = hp.distance;
                    hit = hp.hit;
                    renderable = hp.renderable;
                }
            }
        }
    }
}
