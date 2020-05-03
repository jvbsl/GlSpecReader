using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewGLSpec
{
    public class GLEnum : GLTypeBase
    {

        public GLEnum(string name, string vendor)
            : base(name)
        {
            Entries = new Dictionary<string, EnumEntry>();
            Vendor = vendor;
        }
        
        public string Vendor { get; }
        
        public override bool IsEnum => true;
        
        public Dictionary<string, EnumEntry> Entries { get; }

        public void AddEntry(string name, ulong value)
        {
            if (Entries.TryGetValue(name, out var alreadyExisting))
            {
                if (alreadyExisting.Value != value)
                {
                    Console.WriteLine(
                        $"Enum entry '{name}' already exists within {Name} but with value 0x{alreadyExisting.Value:X} instead of 0x{value:X}."); // TODO: fix
                    //throw new ArgumentException($"Enum entry '{name}' already exists within {Name} but with value 0x{alreadyExisting.Value:X} instead of 0x{value:X}.");
                }
                else
                    Console.WriteLine($"warning: Enum entry '{name}' already exists within {Name}.");
                return;
            }
            Entries.Add(name, new EnumEntry(name, value));
        }

        public class EnumEntry
        {
            public EnumEntry(string name, ulong value)
            {
                Name = name;
                Value = value;
            }
            public string Name { get; }
            public ulong Value { get; }

            public override string ToString()
            {
                return $"{Name} = 0x{Value:X}";
            }
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"enum {Name}");
            sb.AppendLine("{");
            foreach (var entry in Entries)
            {
                sb.AppendLine("\t" + entry.Value.ToString() + ",");
            }
            
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}