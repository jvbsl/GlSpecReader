using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NewGLSpec
{
    public class GLSpecification : IGLSpecification
    {
        public Dictionary<string, GLEnum> Enums { get; }
        
        public Dictionary<string, GLMethod> Methods { get; }
        
        public Dictionary<string, GLTypeBase> BasicTypes { get; }
        
        public List<GLFeature> Features { get; }


        public IReadOnlyDictionary<string, GLTypeBase> Types { get; }

        public GLTypeBase GetType(string name)
        {
            name = name.Trim();
            
            if (name.StartsWith("const "))
                return new GLConstType(GetType(name.Substring("const ".Length)));
            if (!Types.TryGetValue(name, out var ret))
            {
                if (name.EndsWith("*"))
                    return new GLPointerType(GetType(name.Substring(0, name.Length - 2)));

                int stInd, eInd;
                if ((stInd = name.IndexOf('(')) != -1 && (eInd = name.LastIndexOf(')')) != -1)
                {
                    var m = new GLMethod(GetType(name.Substring(0, stInd)), "");
                    var psplt = name.Substring(stInd + 1, eInd - stInd - 1).Split(',');
                    for (var i = 0; i < psplt.Length; i++)
                    {
                        var p = psplt[i].Trim();
                        int lastPart = Array.FindLastIndex(p.ToCharArray(), (c) => !char.IsLetterOrDigit(c));
                        lastPart = lastPart == -1 ? p.Length : lastPart + 1;
                        var pName = p.Substring(lastPart).Trim();
                        pName = string.IsNullOrWhiteSpace(pName) ? $"p{i + 1}" : pName;
                        var pType = GetType(p.Substring(0, lastPart).Trim());

                        if (psplt.Length == 1 && pType.Name == "void")
                            continue;

                        m.Parameters.Add(new GLParameter(pType, pName));
                    }

                    return m;
                }
            }
            else
            {
                return ret;
            }

            return new GLMissingType(name);
        }

        public GLSpecification()
        {
            BasicTypes = new Dictionary<string, GLTypeBase>()
            {
                {"void*", new GLType("System.IntPtr")},
                {"void", new GLType("void")},
                {"char", new GLType("System.SByte")},
                {"short", new GLType("System.Int16")},
                {"int", new GLType("System.Int32")},
                {"unsigned short", new GLType("System.UInt16")},
                {"unsigned int", new GLType("System.UInt32")},
                {"unsigned char", new GLType("System.SByte")},
                
                {"double", new GLType("System.Double")},
                {"float", new GLType("System.Single")},
                {"khronos_float_t", new GLType("System.Single")},
                {"khronos_int8_t", new GLType("System.SByte")},
                {"khronos_uint8_t", new GLType("System.Byte")},
                {"khronos_int16_t", new GLType("System.Int16")},
                {"khronos_uint16_t", new GLType("System.UInt16")},
                {"khronos_int32_t", new GLType("System.Int32")},
                {"khronos_int64_t", new GLType("System.Int64")},
                {"khronos_uint64_t", new GLType("System.UInt64")},
                
                {"khronos_intptr_t", new GLType("System.IntPtr")},
                {"khronos_ssize_t", new GLType("System.IntPtr")},
                {"khronos_usize_t", new GLType("System.UIntPtr")},
            };
            
            Features = new List<GLFeature>();
            
            Methods = new Dictionary<string, GLMethod>();
            Enums = new Dictionary<string, GLEnum>();
            
            Types = new CombinedDictionary(this);
        }

        private class CombinedDictionary : IReadOnlyDictionary<string, GLTypeBase>
        {
            private readonly GLSpecification _context;

            public CombinedDictionary(GLSpecification context)
            {
                _context = context;
            }

            private IEnumerable<KeyValuePair<string, GLTypeBase>> GetConcatenated()
            {
                IEnumerable<KeyValuePair<string, GLTypeBase>> concatenated = _context.BasicTypes;
                if (concatenated == null)
                    throw new NullReferenceException();

                concatenated = concatenated.Concat(_context.Enums.Select(x => new KeyValuePair<string, GLTypeBase>(x.Key, x.Value)));
                concatenated = concatenated.Concat(_context.Methods.Select(x => new KeyValuePair<string, GLTypeBase>(x.Key, x.Value)));

                return concatenated;
            }
            public IEnumerator<KeyValuePair<string, GLTypeBase>> GetEnumerator()
            {
                return GetConcatenated().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public int Count => _context.BasicTypes.Count + _context.Enums.Count + _context.Methods.Count;

            public bool ContainsKey(string key)
            {
                if (_context.BasicTypes.ContainsKey(key))
                    return true;
                if (_context.Enums.ContainsKey(key))
                    return true;
                if (_context.Methods.ContainsKey(key))
                    return true;
                
                return false;
            }

            public bool TryGetValue(string key, out GLTypeBase value)
            {
                if (_context.BasicTypes.TryGetValue(key, out value))
                    return true;
                if (_context.Enums.TryGetValue(key, out var tmpEnum))
                {
                    value = tmpEnum;
                    return true;
                }

                if (_context.Methods.TryGetValue(key, out var tmpMethod))
                {
                    value = tmpMethod;
                    return true;
                }
                
                return false;
            }

            public GLTypeBase this[string key] => TryGetValue(key, out var res) ? res : default;

            public IEnumerable<string> Keys => GetConcatenated().Select(x => x.Key);
            public IEnumerable<GLTypeBase> Values  => GetConcatenated().Select(x => x.Value);
        }
    }
}