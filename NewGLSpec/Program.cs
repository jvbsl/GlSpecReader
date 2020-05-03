using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NewGLSpec.CodeGenerator;
using NewGLSpec.SpecLoader.Xml;

namespace NewGLSpec
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var st = new Stopwatch();
            st.Start();
            
            var loader = new GLSpecLoader();
            var spec = loader.LoadSpecification();
            
            var glGen = new GLGenerator();

            Parallel.ForEach(spec.Features, (f) =>
            {
                glGen.Generate(spec, f);
            });

            st.Stop();


            Console.WriteLine(st.ElapsedMilliseconds);
        }
    }
}