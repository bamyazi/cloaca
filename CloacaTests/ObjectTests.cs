﻿using System.Numerics;
using System.Collections.Generic;

using LanguageImplementation.DataTypes;

using NUnit.Framework;

namespace CloacaTests
{
    [TestFixture]
    public class ObjectTests : RunCodeTest
    {
        [Test]
        public void DeclareClass()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   pass\n", new Dictionary<string, object>(), 1);
        }

        [Test]
        public void DeclareAndCreateClassNoConstructor()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   pass\n" +
                                         "bar = Foo()\n", new Dictionary<string, object>(), 1);
            var variables = interpreter.DumpVariables();
            Assert.That(variables, Contains.Key("bar"));
        }

        [Test]
        public void DeclareAndCreateClassDefaultConstructor()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   def __init__(self):\n" +
                                         "      pass\n" +
                                         "bar = Foo()\n", new Dictionary<string, object>(), 1);

            var variables = new VariableMultimap(interpreter);
            Assert.That(variables.ContainsKey("bar"));
            var bar = variables.Get("bar", typeof(PyObject));
        }

        [Test]
        public void DeclareConstructor()
        {
            var interpreter = runProgram("a = 1\n" +
                                         "class Foo:\n" +
                                         "   def __init__(self):\n" +
                                         "      global a\n" +
                                         "      a = 2\n" +
                                         "\n" +
                                         "bar = Foo()\n", new Dictionary<string, object>(), 1);
            var variables = new VariableMultimap(interpreter);
            var reference = new VariableMultimap(new TupleList<string, object> {
                { "a", new BigInteger(2) }
            });
            Assert.DoesNotThrow(() => variables.AssertSubsetEquals(reference));
        }

        [Test]
        public void DeclareConstructorArgument()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   def __init__(self, new_a):\n" +
                                         "      self.a = new_a\n" +
                                         "\n" +
                                         "bar = Foo(2)\n", new Dictionary<string, object>(), 1);
            var variables = new VariableMultimap(interpreter);
            var bar = (PyObject)variables.Get("bar");
            Assert.That(bar.__dict__["a"], Is.EqualTo(new BigInteger(2)));
        }

        [Test]
        public void DeclareClassMember()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   def __init__(self):\n" +
                                         "      self.a = 1\n" +
                                         "\n" +
                                         "bar = Foo()\n", new Dictionary<string, object>(), 1);
            var variables = new VariableMultimap(interpreter);
            var bar = (PyObject) variables.Get("bar");
            Assert.That(bar.__dict__["a"], Is.EqualTo(new BigInteger(1)));
        }

        [Test]
        public void AccessClassMember()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   def __init__(self):\n" +
                                         "      self.a = 1\n" +
                                         "\n" +
                                         "bar = Foo()\n" + 
                                         "b = bar.a\n" +
                                         "bar.a = 2\n", new Dictionary<string, object>(), 1);
            var variables = new VariableMultimap(interpreter);
            var bar = (PyObject)variables.Get("bar");
            Assert.That(bar.__dict__["a"], Is.EqualTo(new BigInteger(2)));
        }

        [Test]
        public void AccessClassMethod()
        {
            //>>> def make_foo():
            //...   class Foo:
            //...     def __init__(self):
            //...       self.a = 1
            //...     def change_a(self, new_a):
            //...       self.a = new_a
            //...
            //>>> dis(make_foo)
            //  2           0 LOAD_BUILD_CLASS
            //              2 LOAD_CONST               1 (<code object Foo at 0x0000021BD5908D20, file "<stdin>", line 2>)
            //              4 LOAD_CONST               2 ('Foo')
            //              6 MAKE_FUNCTION            0
            //              8 LOAD_CONST               2 ('Foo')
            //             10 CALL_FUNCTION            2
            //             12 STORE_FAST               0 (Foo)
            //             14 LOAD_CONST               0 (None)
            //             16 RETURN_VALUE
            var interpreter = runProgram("class Foo:\n" +
                                         "   def __init__(self):\n" +
                                         "      self.a = 1\n" +
                                         "\n" +
                                         "   def change_a(self, new_a):\n"+
                                         "      self.a = new_a\n" +
                                         "\n" +
                                         "bar = Foo()\n" +
                                         "bar.change_a(2)\n", new Dictionary<string, object>(), 1);
            var variables = new VariableMultimap(interpreter);
            var bar = (PyObject)variables.Get("bar");
            Assert.That(bar.__dict__["a"], Is.EqualTo(new BigInteger(2)));
        }

        [Test]
        [Ignore("Subclassing not implemented yet")]
        public void SubclassBasic()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   def __init__(self):\n" +
                                         "      self.a = 1\n" +
                                         "\n" +
                                         "class Bar(Foo):\n" +
                                         "   def change_a(self, new_a):\n" +
                                         "      self.a = new_a\n" +
                                         "\n" +
                                         "bar = Bar()\n" +
                                         "bar.change_a(2)\n", new Dictionary<string, object>(), 1);
            var variables = new VariableMultimap(interpreter);
            var bar = (PyObject)variables.Get("bar");
            Assert.That(bar.__dict__["a"], Is.EqualTo(new BigInteger(2)));
        }

        [Test]
        [Ignore("Subclassing not implemented yet")]
        public void SubclassSuperconstructor()
        {
            var interpreter = runProgram("class Foo:\n" +
                                         "   def __init__(self):\n" +
                                         "      self.a = 1\n" +
                                         "\n" +
                                         "class Bar(Foo):\n" +
                                         "   def __init__(self):\n" +
                                         "      super().__init__()\n" +
                                         "      self.b = 2\n" +
                                         "\n" +
                                         "bar = Bar()\n", new Dictionary<string, object>(), 1);
            var variables = new VariableMultimap(interpreter);
            var bar = (PyObject)variables.Get("bar");
            Assert.That(bar.__dict__["a"], Is.EqualTo(new BigInteger(1)));
            Assert.That(bar.__dict__["b"], Is.EqualTo(new BigInteger(2)));
        }
    }
}
