﻿using System.Numerics;
using System.Collections.Generic;

using NUnit.Framework;

using LanguageImplementation.DataTypes;

namespace CloacaTests
{
    public class GotAnInt
    {
        public int TheInt;
    }

    [TestFixture]
    public class SysModule : RunCodeTest
    {
        [Test]
        public void SchedulerAnotherFunctionNoArgs()
        {
            runBasicTest(
                "import sys\n" +
                "\n" +
                "a = 0\n" +
                "def change_a():\n" +
                "   global a\n" +
                "   a = 1\n" +
                "\n" + 
                "sys.scheduler.schedule(change_a)\n", new VariableMultimap(new TupleList<string, object>
            {
                { "a", PyInteger.Create(1) }
            }), 2);
        }

        [Test]
        public void SchedulerParentContextPreserved()
        {
            // This test was added due to a lot of paranoia around how the subcontext call stack is created.
            // It's taking the existing call stack and just shoving on change_a. The interpreter then runs
            // through everything as normal. What I expect to happen then is that after change_a finishes,
            // it attempts to run the parent context. This is a no-no! This test will show if this happens
            // based on the result of the variable 'b'. If it's greater than 1, then it got run more than
            // once.
            //
            // We use an external entity to track this because its reference is independent of different
            // contexts.
            var intHaver = new GotAnInt();
            runBasicTest(
                "import sys\n" +
                "\n" +
                "a = 0\n" +
                "def change_a():\n" +
                "   global a\n" +
                "   a = 1\n" +
                "\n" +
                "b.TheInt += 1\n" +
                "sys.scheduler.schedule(change_a)\n", 
                new Dictionary<string, object>()
                {
                    { "b", intHaver }
                }, new VariableMultimap(new TupleList<string, object>
            {
                { "a", PyInteger.Create(1) },
            }), 2);

            Assert.That(intHaver.TheInt, Is.EqualTo(1));
        }

        [Test]
        public void SchedulerPassOneArg()
        {
            var intHaver = new GotAnInt();
            runBasicTest(
                "import sys\n" +
                "\n" +
                "def change_a(got_an_int):\n" +
                "   got_an_int.TheInt = 1\n" +
                "\n" +
                "sys.scheduler.schedule(change_a, container)\n",
                new Dictionary<string, object>()
            {
                { "container", intHaver }
            }, new VariableMultimap(new TupleList<string, object>
            {
            }), 2);

            Assert.That(intHaver.TheInt, Is.EqualTo(1));
        }
    }
}
