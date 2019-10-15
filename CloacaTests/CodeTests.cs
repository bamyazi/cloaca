﻿using System.Numerics;
using System.Collections.Generic;

using NUnit.Framework;

using LanguageImplementation.DataTypes;

namespace CloacaTests
{
    [TestFixture]
    public class Basics : RunCodeTest
    {
        [Test]
        public void SimpleAssignment()
        {
            runBasicTest("a = 10\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(10) }
            }), 1);
        }

        [Test]
        public void SimpleIntMath()
        {
            runBasicTest("a = 10 * (2 + 4) / 3\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(20) }
            }), 1);
        }

        [Test]
        public void SimpleFloatMath()
        {
            runBasicTest("a = 10.0 * (2.0 + 4.0) / 3.0\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyFloat(20.0) }
            }), 1);
        }

        [Test]
        public void SimpleStrAssign()
        {
            runBasicTest("a = 'Hello!'\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", "Hello!" }
            }), 1);

            runBasicTest("a = \"Hello!\"\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", "Hello!" }
            }), 1);
        }

        [Test]
        public void SimpleBoolAssign()
        {
            runBasicTest("a = True\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", true }
            }), 1);

            runBasicTest("a = False\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", false }
            }), 1);
        }

        [Test]
        public void Comparisons()
        {
            runBasicTest("a = 10\n" +
                "b = a < 10\n" +
                "c = a == 10\n" +
                "d = a != 10\n" +
                "e = a > 10\n" +
                "f = a <= 10\n" +
                "g = a >= 10\n" +
                "h = a <> 10\n",
                new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(10) },
                { "b", false },
                { "c", true },
                { "d", false },
                { "e", false },
                { "f", true },
                { "g", true },
                { "h", false },
            }), 1);
        }

        [Test]
        public void IsNoneIsNotNone()
        {
            runBasicTest("b = a is None\n" +
                "c = a is not None\n",
            new Dictionary<string, object>
            {
                { "a", NoneType.Instance }
            },
            new VariableMultimap(new TupleList<string, object>
            {
                { "a", NoneType.Instance },
                { "b", true },
                { "c", false }
            }), 1);

            // Now let's flip A around and make sure we're still cool.
            runBasicTest("b = a is None\n" +
                "c = a is not None\n",
            new Dictionary<string, object>
            {
                { "a", new PyInteger(10) }
            },
            new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(10) },
                { "b", false },
                { "c", true }
            }), 1);
        }

        [Test]
        public void BasicWait()
        {
            runBasicTest(
                "a = 10 * (2 + 4) / 3\n" +
                "wait\n" +
                "b = a + 3\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(20) },
                { "b", new PyInteger(23) }
            }), 2);
        }

        [Test]
        public void BasicConditionalTrue()
        {
            runBasicTest(
                "a = 10\n" +
                "if a == 10:\n" +
                "   a = 1\n" +
                "a = a + 1\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(2) }
            }), 1);
        }

        [Test]
        public void BasicConditionalOffTheEnd()
        {
            // Conditional is last opcode. We want to fall-through without going out of bounds
            runBasicTest(
                "a = 9\n" +
                "if a == 10:\n" +
                "   a = a + 1\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(9) }
            }), 1);
        }

        [Test]
        public void BasicConditionalFalse()
        {
            runBasicTest(
                "a = 10\n" +
                "if a != 10:\n" +
                "   a = 1\n" +
                "a = a + 1\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(11) }
            }), 1);
        }

        [Test]
        public void WhileBasic()
        {
            runBasicTest(
                "a = 0\n" +
                "while a < 3:\n" +
                "   a = a + 1\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", new PyInteger(3) }
            }), 1);
        }

        [Test]
        public void WhileElse()
        {
            string program =
                "while a < 3:\n" +
                "   a = a + 1\n" +
                "else:\n" +
                "   a = a + 100\n";

            // Runs while loop, then the else clause
            runBasicTest(program,
                new Dictionary<string, object>
                {
                    { "a", new PyInteger(0) }
                }, new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(103) }
                }), 1);

            // Skips the while loop, runs the else clause
            runBasicTest(program,
                new Dictionary<string, object> 
                {
                    { "a", new PyInteger(10) }
                }, new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(110) }
                }), 1);
        }

        [Test]
        public void SingleLayerIfElifElse()
        {
            string program =
                "if a == 10:\n" +
                "   a = 1\n" +
                "elif a == 11:\n" +
                "   a = 3\n" +
                "else:\n" +
                "   a = 5\n" +
                "a = a + 1\n";

            runBasicTest(program,
                new Dictionary<string, object>
                {
                    { "a", new PyInteger(10) }
                }, new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(2) }
                }), 1);

            runBasicTest(program,
                new Dictionary<string, object>
                {
                    { "a", new PyInteger(11) }
                }, new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(4) }
                }), 1);

            runBasicTest(program,
                new Dictionary<string, object>
                {
                    { "a", new PyInteger(12) }
                }, new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(13) }
                }), 1);
        }

        // TODO: Add test for basic parse error of things like missing newlines and poor indentation.
    }

    [TestFixture]
    public class FunctionTests : RunCodeTest
    {
        [Test]
        public void VoidIntFunction()
        {
            string program =
                "def foo():\n" +
                "   return 1\n" +
                "a = foo()\n";

            runBasicTest(program,
                new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(1) }
                }), 1, new string[] { "foo" });
        }

        [Test]
        public void InnerAndOuterScopesLocal()
        {
            string program =
                "a = 1\n" +
                "def foo():\n" +
                "   a = 2\n" +
                "foo()\n";

            runBasicTest(program,
                new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(1) }
                }), 1, new string[] { "foo" });
        }

        [Test]
        public void InnerGlobal()
        {
            string program =
                "a = 1\n" +
                "def foo():\n" +
                "   global a\n" +
                "   a = 2\n" +
                "foo()\n";

            runBasicTest(program,
                new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(2) }
                }), 1, new string[] { "foo" });
        }

        [Test]
        public void ImplicitlyUsesGlobal()
        {
            string program =
                "a = 1\n" +
                "def foo():\n" +
                "   b = a + 1\n" +
                "   return b\n" +
                "a = foo()\n";

            runBasicTest(program,
                new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(2) }
                }), 1, new string[] { "foo" });
        }

        [Test]
        public void IntIntFunction()
        {
            string program =
                "def foo(x):\n" +
                "   return x+1\n" +
                "a = foo(3)\n";

            runBasicTest(program,
                new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(4) }
                }), 1, new string[] { "foo" });
        }

        [Test]
        public void Int2IntFunction()
        {
            // Using a non-communative operator will help validate ordering
            string program =
                "def foo(x, y):\n" +
                "   return x - y\n" +
                "a = foo(6, 2)\n";

            runBasicTest(program,
                new VariableMultimap(new TupleList<string, object>
                {
                    { "a", new PyInteger(4) }
                }), 1, new string[] { "foo" });
        }
    }
}
