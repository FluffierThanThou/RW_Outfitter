﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Outfitter
{
    public class SaveablePawn : IExposable
    {
        // Exposed members
        public Pawn Pawn;

        public bool TargetTemperaturesOverride;
        public bool AddWorkStats = true;
        public bool AddIndividualStats = true;

        public FloatRange Temperatureweight;

        public FloatRange TargetTemperatures;
        public FloatRange RealComfyTemperatures;

        public List<Saveable_Pawn_StatDef> Stats = new List<Saveable_Pawn_StatDef>();

        //  public SaveablePawn(Pawn pawn)
        //    {
        //        Pawn = pawn;
        //        Stats = new List<Saveable_Pawn_StatDef>();
        //        _lastStatUpdate = -5000;
        //        _lastTempUpdate = -5000;
        //    }


        public void ExposeData()
        {
            Scribe_References.LookReference(ref Pawn, "Pawn");
            Scribe_Values.LookValue(ref TargetTemperaturesOverride, "targetTemperaturesOverride");
            Scribe_Values.LookValue(ref TargetTemperatures, "TargetTemperatures");
            Scribe_Values.LookValue(ref RealComfyTemperatures, "RealComfyTemperatures");
            Scribe_Values.LookValue(ref Temperatureweight, "Temperatureweight");
            Scribe_Collections.LookList(ref Stats, "Stats", LookMode.Deep);
            Scribe_Values.LookValue(ref AddWorkStats, "AddWorkStats", true);
            Scribe_Values.LookValue(ref AddIndividualStats, "AddIndividualStats", true);

            if (RealComfyTemperatures == null)
            {
                RealComfyTemperatures.min = Pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMin);
                RealComfyTemperatures.max = Pawn.def.GetStatValueAbstract(StatDefOf.ComfyTemperatureMax);

                if (Pawn.story.traits.DegreeOfTrait(TraitDef.Named("TemperaturePreference")) != 0)
                {
                    //calculating trait offset because there's no way to get comfytemperaturemin without clothes
                    List<Trait> traitList = (
                        from trait in Pawn.story.traits.allTraits
                        where trait.CurrentData.statOffsets != null && trait.CurrentData.statOffsets.Any(se => se.stat == StatDefOf.ComfyTemperatureMin || se.stat == StatDefOf.ComfyTemperatureMax)
                        select trait
                        ).ToList();

                    foreach (Trait t in traitList)
                    {
                        RealComfyTemperatures.min += t.CurrentData.statOffsets.First(se => se.stat == StatDefOf.ComfyTemperatureMin).value;
                        RealComfyTemperatures.max += t.CurrentData.statOffsets.First(se => se.stat == StatDefOf.ComfyTemperatureMax).value;
                    }
                }
            }
        }
    }
}