﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloacaTests;
using LanguageImplementation;
using NUnit.Framework;

namespace CloacaNativeTests
{
    // Bootstrap tests for initial codebase/tests construction
    [TestFixture]
    public class BootstrapTests : NativeRunCodeTest
    {
        private void CreateTestFile()
        {
            if (File.Exists("test.dat")) return;
            using (var writer = new StreamWriter("test.dat"))
            {
                writer.WriteLine("Look at my waistcoat.");
                writer.WriteLine("It's made of roast beef!");
            }
        }

        [Test]
        public void CreateTestContext()
        {
            CreateTestFile();
            runBasicTest(
                "s = open(\"test.dat\", \"r\")\n" +
                "line = s.readline()\n",
                new VariableMultimap(new TupleList<string, object>())
                , 1);
        }

        [Test]
        public void Inheritance()
        {
            CreateTestFile();
            runBasicTest(
                "s = open(\"test.dat\", \"r\")\n" +
                "s.test()\n",
                new VariableMultimap(new TupleList<string, object>())
                , 1);
        }

        [Test]
        public void InheritanceCreate()
        {
            CreateTestFile();
            runBasicTest(
                "s = PyTextIOWrapper();",
                new VariableMultimap(new TupleList<string, object>())
                , 1);
        }
    }
}