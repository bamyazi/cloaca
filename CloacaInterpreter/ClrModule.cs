﻿using System.Collections.Generic;
using System.Reflection;

using LanguageImplementation;
using LanguageImplementation.DataTypes;

namespace CloacaInterpreter
{
    /// <summary>
    /// Taking this on to the frame context as a means to track imported clr modules... and whatever else might come later.
    /// I don't think Python.NET nor IronPython do it this way but I can't be bothered to mimick them.
    /// </summary>
    public class ClrContext
    {
        public Dictionary<string, Assembly> AddedReferences;
        public const string FrameContextTokenName = "__clr__";

        public ClrContext()
        {
            AddedReferences = new Dictionary<string, Assembly>();
        }
    }

    /// <summary>
    /// Start of a CLR module a la IronPython or Python.NET. The internals are obfuscated and expodes as a PyModule.
    /// This is not yet connected to anything and only does the basic assembly load; it's not connected into the
    /// namespace yet.
    /// </summary>
    public class ClrModuleInternals
    {
        private List<Assembly> references;

        public ClrModuleInternals()
        {
            references = new List<Assembly>();
        }

        private void addReference(FrameContext context, string name)
        {            
            var assembly = Assembly.Load(name);
            if(!references.Contains(assembly))
            {
                references.Add(assembly);
            }

            ClrContext clrContext = null;
            if(context.HasVariable(ClrContext.FrameContextTokenName))
            {
                clrContext = (ClrContext) context.GetVariable(ClrContext.FrameContextTokenName);
            }
            else
            {
                clrContext = new ClrContext();
            }

            if(!clrContext.AddedReferences.ContainsKey(name))
            {
                clrContext.AddedReferences.Add(name, assembly);
            }

            context.SetVariable(ClrContext.FrameContextTokenName, new ClrContext());
        }

        /// <summary>
        /// Intended to be called once by the Cloaca interpreter to create the clr module in an injectable
        /// module loader.
        /// </summary>
        /// <returns></returns>
        public static PyModule CreateClrModule()
        {
            var internals = new ClrModuleInternals();
            var module = PyModule.Create("clr");
            module.__dict__.Add("AddReference", internals.GetType().GetMethod("addReference"));
            module.__dict__.Add("References", internals.references);
            return module;
        }
    }
}
