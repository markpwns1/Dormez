using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dormez.Evaluation;
using Dormez.Memory;

namespace Dormez.Types
{
    public class DTable : DObject
    {
        public DWeakFunction GetFunction(string name)
        {
            return AssertType<DWeakFunction>(members[name].value);
        }
        
        public override DObject OpPOW(DObject other)
        {
            if (MemberExists("exponent"))
            {
                return AssertType<DBool>(GetFunction("exponent").CLRCall(other));
            }

            return base.OpPOW(other);
        }

        public override DObject OpMOD(DObject other)
        {
            if (MemberExists("mod"))
            {
                return AssertType<DBool>(GetFunction("mod").CLRCall(other));
            }

            return base.OpMOD(other);
        }

        public override DBool OpLS(DObject other)
        {
            if (MemberExists("lessThan"))
            {
                return AssertType<DBool>(GetFunction("lessThan").CLRCall(other));
            }

            return base.OpLS(other);
        }

        public override DBool OpNEQ(DObject other)
        {
            if (MemberExists("notEquals"))
            {
                return AssertType<DBool>(GetFunction("notEquals").CLRCall(other));
            }

            return base.OpNEQ(other);
        }

        public override DBool OpGR(DObject other)
        {
            if (MemberExists("greaterThan"))
            {
                return AssertType<DBool>(GetFunction("greaterThan").CLRCall(other));
            }

            return base.OpGR(other);
        }

        public override Variable OpINDEX(DObject other)
        {
            throw new Exception("A table cannot be indexed. Instead, make getter and setter methods.");
        }

        public override DBool OpEQ(DObject other)
        {
            if (MemberExists("equals"))
            {
                return AssertType<DBool>(GetFunction("equals").CLRCall(other));
            }

            return base.OpEQ(other);
        }

        public override DObject OpNEG()
        {
            if (MemberExists("negate"))
            {
                return GetFunction("negate").CLRCall();
            }

            return base.OpNEG();
        }

        public override DObject OpDIV(DObject other)
        {
            if (MemberExists("divide"))
            {
                return GetFunction("divide").CLRCall(other);
            }

            return base.OpDIV(other);
        }

        public override DObject OpMUL(DObject other)
        {
            if (MemberExists("multiply"))
            {
                return GetFunction("multiply").CLRCall(other);
            }

            return base.OpMUL(other);
        }

        public override DObject OpSUB(DObject other)
        {
            if (MemberExists("sub"))
            {
                return GetFunction("sub").CLRCall(other);
            }

            return base.OpSUB(other);
        }

        public override DObject OpADD(DObject other)
        {
            if(MemberExists("add"))
            {
                return GetFunction("add").CLRCall(other);
            }

            return base.OpADD(other);
        }

        public override string ToString()
        {
            if (MemberExists("toString"))
            {
                return GetFunction("toString").CLRCall().ToString();
            }

            return "table";
        }
    }
}
