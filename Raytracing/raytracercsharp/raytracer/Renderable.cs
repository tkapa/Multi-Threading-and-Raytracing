using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    abstract class Renderable
    {
        public Vector3 position;
        public Material material;
        public abstract HitPoint intersect(Ray ray);
    }

    class Sphere : Renderable
    {
        public float radius;

        public override HitPoint intersect(Ray ray)
        {
            HitPoint hp = new HitPoint();
            Vector3 L = position - (ray.start);

            float tca = L.dot(ray.delta);
            if (tca < 0) return hp;
            float d2 = L.dot(L) - tca * tca;
            if (d2 > radius * radius) return hp;
            float thc = (float)Math.Sqrt(radius * radius - d2);
            float t0 = tca - thc;
            float t1 = tca + thc;
            float m = Math.Min(t0, t1);
            hp.hit = true;
            hp.position = ray.interpolate(m);
            hp.renderable = this;
            hp.normal = (hp.position - position).normalised();
            hp.distance = m;

            return hp;
        }
    }

}
