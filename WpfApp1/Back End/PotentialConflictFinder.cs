using Loqui;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyCRappy;

public class PotentialConflictFinder
{
    private readonly IEnvironmentStateProvider _environmentStateProvider;
    private readonly SnapShotter _snapShotter;
    private readonly Serializer _serializer;
    public PotentialConflictFinder(IEnvironmentStateProvider environmentStateProvider, SnapShotter snapShotter, Serializer serializer)
    {
        _environmentStateProvider = environmentStateProvider;
        _snapShotter = snapShotter;
        _serializer = serializer;
    }

    public Dictionary<string, List<PotentialConflictRecord>> FindConflicts(ModKey crModKey, List<FormKey> toIgnore, SerializationType serializationType)
    {
        var potentialConflicts = new Dictionary<string, List<PotentialConflictRecord>>();

        var crRecords = _snapShotter.GetModRecords(crModKey);
        if (crRecords == null)
        {
            throw new Exception("Mod " + crModKey.ToString() + " does not exist");
        }

        List<ModKey> overrideMods = new();
        List<ModKey> overrideMasters = new();

        foreach (var record in crRecords)
        {
            var registration = LoquiRegistration.StaticRegister.GetRegister(record.GetType());
            if (registration != null)
            {
                var contexts = _environmentStateProvider.LinkCache?.ResolveAllContexts(record.FormKey, registration.GetterType).ToList() ?? new();
                foreach (var context in contexts)
                {
                    if (context.ModKey.Equals(crModKey))
                    {
                        continue;
                    }

                    if (!overrideMods.Contains(context.ModKey))
                    {
                        overrideMods.Add(context.ModKey);
                        var masters = GetMasterRecords(context.ModKey);
                        if (masters != null)
                        {
                            overrideMasters.AddRange(masters.Where(x => !overrideMasters.Contains(x)));
                        }
                    }
                }
            }
        }

        //overrideMods.RemoveWhere(x => overrideMasters.Contains(x)); // get rid of masters as they should be inherited by the mods mastered to them, and they will provide unnecessary clutter.

        Dictionary<FormKey, List<ModKey>> focusedFormModPairs = new(); // ignores master records
        Dictionary<FormKey, List<ModKey>> allFormModPairs = new(); // used only to check if a given formkey has been reassigned to a different record type (in which case it'll be missed by Mutagen's non-generic context lookup)
        Dictionary<FormKey, ILoquiRegistration?> formRegistrations = new();

        // get all formkeys from all masters of the CR mod
        foreach (var mod in overrideMods)
        {
            var masterRecords = _snapShotter.GetModRecords(mod);
            if (masterRecords == null) { continue; }

            foreach (var record in masterRecords)
            {
                if (toIgnore.Contains(record.FormKey))
                {
                    continue;
                }

                if (allFormModPairs.ContainsKey(record.FormKey))
                {
                    allFormModPairs[record.FormKey].Add(mod);
                }
                else
                {
                    allFormModPairs.Add(record.FormKey, new() { mod });
                }

                if (!formRegistrations.ContainsKey(record.FormKey))
                {
                    formRegistrations.Add(record.FormKey, LoquiRegistration.StaticRegister.GetRegister(record.GetType()));
                }

                if (!overrideMasters.Contains(mod))
                {
                    if (focusedFormModPairs.ContainsKey(record.FormKey))
                    {
                        focusedFormModPairs[record.FormKey].Add(mod);
                    }
                    else
                    {
                        focusedFormModPairs.Add(record.FormKey, new() { mod });
                    }
                }
            }
        }

        // get rid of all formkeys that only appear in one mod
        focusedFormModPairs = focusedFormModPairs.Where(kv => kv.Value.Count > 1)
                       .ToDictionary(kv => kv.Key, kv => kv.Value);

        // find records that are not ITM
        foreach (var formKey in focusedFormModPairs.Keys)
        {
            // collection record serializations
            Dictionary<ModKey, (string, string)> serializations = new();

            var registration = formRegistrations[formKey];
            if (registration == null)
            {
                throw new Exception("Could not find registration for " + formKey.ToString());
            }

            var contexts = _environmentStateProvider.LinkCache?.ResolveAllContexts(formKey, registration.GetterType).ToList() ?? new();

            if (contexts.Count != allFormModPairs[formKey].Count)
            {
                throw new Exception("Not all forms could be resolved. FormID may have been reused for a different record type");
            }

            foreach (var modKey in focusedFormModPairs[formKey])
            {
                var context = contexts.Where(x => x.ModKey.Equals(modKey)).FirstOrDefault();
                if (context == null)
                {
                    throw new Exception("Unexpected null context");
                }

                var serialization = _serializer.SerializeRecord(context, serializationType);
                serializations.Add(modKey, serialization);
            }

            // check for distinct
            var serializationGroups = serializations.GroupBy(x => x.Value.Item2).ToArray();

            // store conflict if needed
            if (serializationGroups.Length > 1)
            {
                var conflictEntry = new PotentialConflictRecord(serializations, formKey, serializationType);
                if(potentialConflicts.ContainsKey(conflictEntry.RecordType))
                {
                    potentialConflicts[conflictEntry.RecordType].Add(conflictEntry);
                }
                else
                {
                    potentialConflicts.Add(conflictEntry.RecordType, new() { conflictEntry });
                }
            }
        }


        return potentialConflicts;
    }

    private List<ModKey>? GetMasterRecords(ModKey modKey)
    {
        var modListing = _environmentStateProvider.LoadOrder?.TryGetValue(modKey);
        if (modListing == null || modListing.Mod == null)
        {
            System.Windows.MessageBox.Show("Could not find mod " + modKey.ToString() + " in current load order", "Error");
            return null;
        }

        return modListing.Mod.MasterReferences.Select(x => x.Master).ToList();
    }

    public class PotentialConflictRecord
    {
        public PotentialConflictRecord(Dictionary<ModKey, (string, string)> elements, FormKey recordFormKey, SerializationType serializationType)
        {
            foreach (var  kvp in elements)
            {
                Serializations.Add(kvp.Key, kvp.Value.Item2);
                if (RecordType == string.Empty)
                {
                    RecordType = kvp.Value.Item1;
                }
            }
            RecordFormKey = recordFormKey;
            SerializationType = serializationType;
        }

        public Dictionary<ModKey, string> Serializations { get; set; } = new();
        public string RecordType { get; set; } = string.Empty; 
        public SerializationType SerializationType { get; set; }
        public FormKey RecordFormKey { get; set; }
    }
}
