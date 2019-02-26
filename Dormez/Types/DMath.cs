using System;
using Dormez.Memory;
using Dormez.Templates;

namespace Dormez.Types
{
    [Static("math")]
    public class DMath : DObject
    {
        [Member("pi")]
        public DNumber Pi { get { return Math.PI.ToDNumber(); } }

        [Member("abs")]
        public double Abs(DNumber n)
        {
            return Math.Abs(n.ToFloat());
        }

        [Member("acos")]
        public double Acos(DNumber n)
        {
            return Math.Acos(n.ToFloat());
        }

        [Member("asin")]
        public double Asin(DNumber n)
        {
            return Math.Asin(n.ToFloat());
        }

        [Member("atan")]
        public double Atan(DNumber n)
        {
            return Math.Atan(n.ToFloat());
        }

        [Member("atan2")]
        public double Atan2(DNumber n1, DNumber n2)
        {
            return Math.Atan2(n1.ToFloat(), n2.ToFloat());
        }

        [Member("ceil")]
        public double Ceiling(DNumber n)
        {
            return Math.Ceiling(n.ToFloat());
        }

        [Member("cos")]
        public double Cos(DNumber n)
        {
            return Math.Cos(n.ToFloat());
        }

        [Member("cosh")]
        public double Cosh(DNumber n)
        {
            return Math.Cosh(n.ToFloat());
        }

        [Member("exp")]
        public double Exp(DNumber n)
        {
            return Math.Exp(n.ToFloat());
        }

        [Member("floor")]
        public double Floor(DNumber n)
        {
            return Math.Floor(n.ToFloat());
        }

        [Member("ln")]
        public double Ln(DNumber n)
        {
            return Math.Log(n.ToFloat());
        }

        [Member("log")]
        public double Log(DNumber n)
        {
            return Math.Log10(n.ToFloat());
        }

        [Member("max")]
        public double Max(DNumber n1, DNumber n2)
        {
            return Math.Max(n1.ToFloat(), n2.ToFloat());
        }

        [Member("min")]
        public double Min(DNumber n1, DNumber n2)
        {
            return Math.Min(n1.ToFloat(), n2.ToFloat());
        }

        [Member("limit")]
        public double Limit(DNumber value, DNumber lower, DNumber upper)
        {
            return Math.Min(upper.ToFloat(), Math.Max(lower.ToFloat(), value.ToFloat()));
        }

        [Member("round")]
        public double Round(DNumber n)
        {
            return Math.Round(n.ToFloat());
        }

        [Member("sign")]
        public int Sign(DNumber n)
        {
            return Math.Sign(n.ToFloat());
        }

        [Member("sin")]
        public double Sin(DNumber n)
        {
            return Math.Sin(n.ToFloat());
        }

        [Member("sinh")]
        public double Sinh(DNumber n)
        {
            return Math.Sinh(n.ToFloat());
        }

        [Member("sqrt")]
        public double Sqrt(DNumber n)
        {
            return Math.Sqrt(n.ToFloat());
        }

        [Member("tan")]
        public double Tan(DNumber n)
        {
            return Math.Tan(n.ToFloat());
        }

        [Member("tanh")]
        public double Tanh(DNumber n)
        {
            return Math.Tanh(n.ToFloat());
        }

        [Member("nthRoot")]
        public double NthRoot(DNumber value, DNumber root)
        {
            if (root.ToFloat() % 2 == 0 || value.ToFloat() >= 0)
            {
                // if even
                return Math.Pow(value.ToFloat(), 1.0f / root.ToFloat());
            }
            else
            {
                // if odd
                return (-Math.Pow(-value.ToFloat(), 1.0f / root.ToFloat()));
            }
        }
    }
}
