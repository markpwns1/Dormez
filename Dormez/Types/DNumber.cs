﻿using System;
using Dormez.Templates;

namespace Dormez.Types
{
    public class DNumber : DObject
    {
        public float value;

        public DNumber(int v)
        {
            value = v;
        }

        public DNumber(float v)
        {
            value = v;
        }

        public override bool Equals(object obj)
        {
            return value == AssertType<DNumber>(obj).value;
        }

        public override DBool OpGR(DObject other)
        {
            return (value > AssertType<DNumber>(other).value).ToDBool();
        }

        public override DBool OpLS(DObject other)
        {
            return (value < AssertType<DNumber>(other).value).ToDBool();
        }

        public override DObject OpADD(DObject other)
        {
            return (value + AssertType<DNumber>(other).value).ToDNumber();
        }

        public override DObject OpSUB(DObject other)
        {
            return (value - AssertType<DNumber>(other).value).ToDNumber();
        }

        public override DObject OpNEG()
        {
            return new DNumber(-value);
        }

        public override DObject OpMUL(DObject other)
        {
            return (value * AssertType<DNumber>(other).value).ToDNumber();
        }

        public override DObject OpDIV(DObject other)
        {
            return (value / AssertType<DNumber>(other).value).ToDNumber();
        }

        public override DObject OpPOW(DObject other)
        {
            return Math.Pow(value, AssertType<DNumber>(other).value).ToDNumber();
        }

        public override DObject OpMOD(DObject other)
        {
            return (value % AssertType<DNumber>(other).value).ToDNumber();
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public int ToInt()
        {
            return (int)Math.Round(value);
        }

        public float ToFloat()
        {
            return value;
        }

        public override DObject Clone()
        {
            return value.ToDNumber();
        }

        [Member("toChar")]
        public DChar ToChar()
        {
            return ((char)ToInt()).ToDChar();
        }
    }
}
