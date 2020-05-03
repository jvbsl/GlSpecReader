using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NewGLSpec.Extensions;

namespace NewGLSpec.SpecLoader.Xml
{
    public class GLFeatureLoader : IGLPartLoader<XElement>
    {
        private void LoadStuff(HashSet<GLChangeReference> typeReferences, XElement e)
        {
            foreach (var r in e.Elements())
            {
                string name = r.Attribute("name")?.Value;
                typeReferences.Add(new GLChangeReference(r.Name.LocalName, name));
            }
        }
        public void LoadPart(IGLSpecification spec, XElement root)
        {
            var features = spec.Features;
            foreach (var f in root.Elements("feature"))
            {
                var feature = new GLFeature(f.Attribute("api")?.Value, Version.Parse(f.Attribute("number")?.Value), f.Attribute("name")?.Value);

                foreach (var require in f.Elements("require"))
                {
                    string profileName = require.Attribute("profile")?.Value;
                    var profile = feature.GetOrCreateProfile(profileName);
                    LoadStuff(profile.Additions, require);
                }
                
                foreach (var removal in f.Elements("remove"))
                {
                    string profileName = removal.Attribute("profile")?.Value;
                    var profile = feature.GetOrCreateProfile(profileName);
                    LoadStuff(profile.Removals, removal);
                }
                
                features.Add(feature);
            }
            
            features.Sort(((a, b) =>
            {
                var apiCompare = string.Compare(a.Api, b.Api, StringComparison.Ordinal);
                return apiCompare == 0 ? a.Number.CompareTo(b.Number) : apiCompare;
            }));

            var previousFeature = features[0];
            for (var i = 1; i < features.Count; i++)
            {
                var feature = features[i];
                if (previousFeature.Api == feature.Api)
                    feature.InitializePreviousFeature(previousFeature);
                previousFeature = feature;

            }
        }
    }
}