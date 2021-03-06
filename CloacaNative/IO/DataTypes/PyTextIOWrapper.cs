﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LanguageImplementation;
using LanguageImplementation.DataTypes;

namespace CloacaNative.IO.DataTypes
{
    public class PyTextIOWrapperClass : PyClass
    {
        public PyTextIOWrapperClass(CodeObject __init__) :
            base("TextIOWrapper", __init__, new[] { PyTextIOBaseClass.Instance })
        {
            __instance = this;

            Expression<Action<PyTypeObject>> expr = instance => DefaultNew<PyTextIOWrapper>(null);
            var methodInfo = ((MethodCallExpression)expr.Body).Method;
            __new__ = new WrappedCodeObject("__new__", methodInfo, this);
        }

        private static PyTextIOWrapperClass __instance;
        public static PyTextIOWrapperClass Instance => __instance ?? (__instance = new PyTextIOWrapperClass(null));

    }

    public class PyTextIOWrapper : PyTextIOBase
    {
        public PyTextIOWrapper()
        {

        }

        public PyTextIOWrapper(Handle resourceHandle, Stream nativeStream) 
            : base(PyTextIOWrapperClass.Instance,  resourceHandle, nativeStream)
        {
        }

    }
}