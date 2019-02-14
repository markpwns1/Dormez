using System;
using System.Collections.Generic;
using Dormez.Functions;
using Dormez.Memory;

namespace Dormez.Types
{
    public class DTable : DObject
    {
        public void SetMembers(Dictionary<string, Member> members)
        {
            this.members = members;
        }
        public override DObject OpPOW(DObject other)
        {
            if (HasMember("exponent"))
            {
                return AssertType<DBool>(CLRCallFunction("exponent", other));
            }

            return base.OpPOW(other);
        }

        public override DObject OpMOD(DObject other)
        {
            if (HasMember("mod"))
            {
                return AssertType<DBool>(CLRCallFunction("mod", other));
            }

            return base.OpMOD(other);
        }

        public override DBool OpLS(DObject other)
        {
            if (HasMember("lessThan"))
            {
                return AssertType<DBool>(CLRCallFunction("lessThan", other));
            }

            return base.OpLS(other);
        }

        public override DBool OpNEQ(DObject other)
        {
            if (HasMember("notEquals"))
            {
                return AssertType<DBool>(CLRCallFunction("notEquals", other));
            }

            return base.OpNEQ(other);
        }

        public override DBool OpGR(DObject other)
        {
            if (HasMember("greaterThan"))
            {
                return AssertType<DBool>(CLRCallFunction("greaterThan", other));
            }

            return base.OpGR(other);
        }

        public override Member OpINDEX(DObject other)
        {
            return GetMember(other.ToString());
            //throw new Exception("A table cannot be indexed. Instead, make getter and setter methods.");
        }

        public override DBool OpEQ(DObject other)
        {
            if (HasMember("equals"))
            {
                return AssertType<DBool>(CLRCallFunction("equals", other));
            }

            return base.OpEQ(other);
        }

        public override DObject OpNEG()
        {
            if (HasMember("negate"))
            {
                return CLRCallFunction("negate");
            }

            return base.OpNEG();
        }

        public override DObject OpDIV(DObject other)
        {
            if (HasMember("divide"))
            {
                return CLRCallFunction("divide", other);
            }

            return base.OpDIV(other);
        }

        public override DObject OpMUL(DObject other)
        {
            if (HasMember("multiply"))
            {
                return CLRCallFunction("multiply", other);
            }

            return base.OpMUL(other);
        }

        public override DObject OpSUB(DObject other)
        {
            if (HasMember("sub"))
            {
                return CLRCallFunction("sub", other);
            }

            return base.OpSUB(other);
        }

        public override DObject OpADD(DObject other)
        {
            if(HasMember("add"))
            {
                return CLRCallFunction("add", other);
            }

            return base.OpADD(other);
        }

        public override string ToString()
        {
            if (HasMember("toString"))
            {
                return CLRCallFunction("toString").ToString();
            }

            return "table";
        }
    }
}
