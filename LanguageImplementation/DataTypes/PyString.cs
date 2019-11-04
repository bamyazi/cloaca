﻿using System;
using System.Linq;
using System.Numerics;

namespace LanguageImplementation.DataTypes
{
    public class PyStringClass : PyTypeObject
    {
        public PyStringClass(CodeObject __init__) :
            base("str", __init__)
        {
            var classMembers = GetType().GetMethods().Where(m => m.GetCustomAttributes(typeof(ClassMember), false).Length > 0).ToArray();

            foreach (var classMember in classMembers)
            {
                this.__dict__[classMember.Name] = new WrappedCodeObject(classMember.Name, classMember);
            }

            __instance = this;
        }

        private static PyStringClass __instance;
        public static PyStringClass Instance
        {
            get
            {
                if(__instance == null)
                {
                    __instance = new PyStringClass(null);
                }
                return __instance;
            }
        }

        private static void castOperands(PyObject self, PyObject other, out PyString selfOut, out PyString otherOut, string operation)
        {
            selfOut = self as PyString;
            otherOut = other as PyString;
            if (selfOut == null)
            {
                throw new Exception("Tried to use a non-PyString for lvalue of: " + operation);
            }
            if (otherOut == null)
            {
                throw new Exception("Tried to use a non-PyString for rvalue of: " + operation);
            }
        }

        [ClassMember]
        public static PyObject __add__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "concatenation");
            var newPyString = new PyString(a.str + b.str);
            return newPyString;
        }

        [ClassMember]
        public static PyObject __mul__(PyObject self, PyObject other)
        {
            PyString a;
            PyInteger b;
            a = self as PyString;
            b = other as PyInteger;
            if(a == null)
            {
                throw new Exception("Tried to use a non-PyString for lvalue of: multiplication");
            }

            if(b == null)
            {
                // TODO: Try to realize this as a real TypeError object in some way.
                throw new Exception("TypeError: can't multiply sequence by non-int of type 'str'");
            }
            var newPyString = new PyString(string.Concat(Enumerable.Repeat(a.str, (int) b.number)));
            return newPyString;
        }

        [ClassMember]
        public static PyObject __sub__(PyObject self, PyObject other)
        {
            // TODO: TypeError: unsupported operand type(s) for -: '[self type]' and '[other type]'
            throw new Exception("Strings do not support subtraction");
        }

        [ClassMember]
        public static PyObject __div__(PyObject self, PyObject other)
        {
            // TODO: TypeError: unsupported operand type(s) for /: '[self type]' and '[other type]'
            throw new Exception("Strings do not support division");
        }

        [ClassMember]
        public static PyBool __lt__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "less-than");
            return a.str.CompareTo(b.str) < 0;
        }

        [ClassMember]
        public static PyBool __gt__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "greater-than");
            return a.str.CompareTo(b.str) > 0;
        }

        [ClassMember]
        public static PyBool __le__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "less-than-equal");
            return a.str.CompareTo(b.str) <= 0;
        }

        [ClassMember]
        public static PyBool __ge__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "greater-than-equal");
            return a.str.CompareTo(b.str) >= 0;
        }

        [ClassMember]
        public static PyBool __eq__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "equality");
            return a.str.CompareTo(b.str) == 0;
        }

        [ClassMember]
        public static PyBool __ne__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "non-equality");
            return a.str.CompareTo(b.str) != 0;
        }

        [ClassMember]
        public static PyBool __ltgt__(PyObject self, PyObject other)
        {
            PyString a, b;
            castOperands(self, other, out a, out b, "less-than-greater-than");
            var compared = a.str.CompareTo(b.str);
            return compared < 0 && compared > 0;
        }
    }

    public class PyString : PyObject
    {
        public string str;
        public PyString(string str) : base(PyStringClass.Instance)
        {
            this.str = str;
        }

        public PyString()
        {
            str = "";
        }

        public override bool Equals(object obj)
        {
            var asPyStr = obj as PyString;
            if(asPyStr == null)
            {
                return false;
            }
            else
            {
                return asPyStr.str == str;
            }
        }

        public override int GetHashCode()
        {
            return str.GetHashCode();
        }

        public override string ToString()
        {
            return str;
        }
    }
}