using System;
using System.Collections.Generic;

namespace NewGLSpec
{
    public class GLFeature
    {
        public string Api { get; }
        
        public string Name { get; }
        
        public Version Number { get; }
        
        public GLFeature PreviousFeature { get; private set; }

        public GLFeature(string api, Version number, string name)
        {
            Api = api;
            Number = number;
            Name = name;

            BaseProfile = new GLProfile(string.Empty);
            
            Profiles = new Dictionary<string, GLProfile>();
            Profiles.Add(BaseProfile.Name, BaseProfile);
        }

        internal void InitializePreviousFeature(GLFeature previousFeature)
        {
            PreviousFeature = previousFeature;

            foreach (var profile in PreviousFeature.Profiles)
            {
                if (string.IsNullOrEmpty(profile.Key))
                    continue;

                if (!Profiles.ContainsKey(profile.Key))
                    Profiles.Add(profile.Key, profile.Value);
            }
        }

        public class GLProfile : GLChange
        {
            public GLProfile(string name) : base(name)
            {
            }
        }
        
        public GLProfile BaseProfile { get; }
        
        public Dictionary<string, GLProfile> Profiles { get; }

        public GLProfile GetOrCreateProfile(string profileName)
        {
            if (profileName == null)
                return BaseProfile;
            if (!Profiles.TryGetValue(profileName, out var profile))
            {
                profile = new GLProfile(profileName);
                Profiles.Add(profileName, profile);
            }

            return profile;
        }

        private Dictionary<string, HashSet<GLChangeReference>> _collectedFeatures = new Dictionary<string, HashSet<GLChangeReference>>();

        private HashSet<GLChangeReference> GetOrCreateProfileFeature(string profile)
        {
            lock (this)
            {
                if (_collectedFeatures.TryGetValue(profile, out var collectedFeatures))
                    return collectedFeatures;
                collectedFeatures = new HashSet<GLChangeReference>();
                _collectedFeatures.Add(profile, collectedFeatures);
                return collectedFeatures;
            }
        }

        private void AddAndRemove(HashSet<GLChangeReference> collectedFeatures, string profileName)
        {
            if (!Profiles.TryGetValue(profileName, out var profile))
                return;
                
            
            foreach (var e in profile.Additions)
                collectedFeatures.Add(e);
            foreach (var e in profile.Removals)
                collectedFeatures.Remove(e);

            return;
        }
        public HashSet<GLChangeReference> CollectFeatures(string profileName)
        {
            var collectedFeatures = GetOrCreateProfileFeature(profileName);
            if (collectedFeatures.Count != 0)
                return collectedFeatures;

            lock (this)
            {
                
                if (PreviousFeature != null)
                {
                    foreach (var e in PreviousFeature.CollectFeatures(profileName))
                        collectedFeatures.Add(e);
                }

                if (!string.IsNullOrEmpty(profileName))
                    AddAndRemove(collectedFeatures, string.Empty);
                AddAndRemove(collectedFeatures, profileName);
                
                

                return collectedFeatures;
            }
        }
    }
}