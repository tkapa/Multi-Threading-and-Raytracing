using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace raytracer
{
    struct Vector3
    {
        public float x, y, z;

        public Vector3(float xx = 0.0f, float yy = 0.0f, float zz = 0.0f)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public void set(float xx = 0.0f, float yy = 0.0f, float zz = 0.0f)
        {
            x = xx;
            y = yy;
            z = zz;
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
        }

        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
        }

        public static Vector3 operator *(Vector3 lhs, float rhs)
        {
            return new Vector3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
        }

        public static Vector3 operator *(float lhs, Vector3 rhs)
        {
            return new Vector3(lhs * rhs.x, lhs * rhs.y, lhs * rhs.z);
        }

        public static Vector3 operator *(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x * rhs.x, lhs.y * rhs.y, lhs.z * rhs.z);
        }

        public static Vector3 operator /(Vector3 lhs, float rhs)
        {
            return new Vector3(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
        }

        public static Vector3 operator /(float lhs, Vector3 rhs)
        {
            return new Vector3(lhs / rhs.x, lhs / rhs.y, lhs / rhs.z);
        }

        public float dot(Vector3 rhs)
        {
            return x * rhs.x + y * rhs.y + z * rhs.z;
        }

        public Vector3 cross(Vector3 rhs)
        {
            return new Vector3(y * rhs.z - z * rhs.y,
                            z * rhs.x - x * rhs.z,
                            x * rhs.y - y * rhs.x);
        }

        public float normalise()
        {
            float l = (float)Math.Sqrt(x * x + y * y + z * z);
            if (l != 0.0f)
            {
                x /= l;
                y /= l;
                z /= l;
            }
            else
            {
                x = 0;
                y = 0;
                z = 0;
            }
            return l;
        }

        public Vector3 normalised()
        {
            Vector3 temp = new Vector3(x, y, z);
            temp.normalise();
            return temp;
        }

        public float length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public Vector3 reflect(Vector3 n)
        {
            return this - 2.0f * n * dot(n);
        }
    }
}
