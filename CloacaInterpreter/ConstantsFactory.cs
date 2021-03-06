﻿using System;
using System.Numerics;
using System.Text.RegularExpressions;
using LanguageImplementation.DataTypes;

using Antlr4.Runtime.Tree;

namespace CloacaInterpreter
{
    public class ConstantsFactory
    {
        public static Regex LongStringDoubleQuoteRegex = new Regex("^\\\"\\\"\\\"(.+)\\\"\\\"\\\"$");
        public static Regex LongStringSingleQuoteRegex = new Regex("^\\\'\\\'\\\'(.+)\\\'\\\'\\\'$");
        public static Regex DecimalPointNumberRegex = new Regex("\\d+\\.\\d+");

        public static PyString CreateString(IParseTree context)
        {
            // TODO: Expand to include literals, unicode, f-strings...
            // For now, we just clip off the single/double quotes
            string rawText = context.GetText();

            // Check for long string.
            var match = LongStringDoubleQuoteRegex.Match(rawText);
            if(match.Success)
            {
                return PyString.Create(match.Groups[1].Value);
            }

            match = LongStringSingleQuoteRegex.Match(rawText);
            if (match.Success)
            {
                return PyString.Create(match.Groups[1].Value);
            }

            // At this point, just assume quotes on each end and clip them off.
            string finalString = rawText.Substring(1, rawText.Length - 2);
            return PyString.Create(finalString);
        }

        public static object CreateNumber(IParseTree context)
        {
            string rawText = context.GetText();
            if (DecimalPointNumberRegex.Match(rawText).Success)
            {
                return PyFloat.Create(Decimal.Parse(rawText));
            }
            return PyInteger.Create(BigInteger.Parse(context.GetText()));
        }

        public static PyBool CreateBool(IParseTree context)
        {
            // We can make a lot of assumptions here thanks to the ANTLR rules.
            if(context.GetText() == "True")
            {
                return PyBool.True;
            }
            else
            {
                return PyBool.False;
            }
        }
    }
}
