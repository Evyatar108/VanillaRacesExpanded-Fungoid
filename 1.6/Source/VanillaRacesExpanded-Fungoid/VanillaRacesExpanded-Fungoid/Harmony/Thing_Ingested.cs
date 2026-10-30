using HarmonyLib;
using RimWorld;
using Verse;

namespace VanillaRacesExpandedFungoid
{

    // Fungus is human meat to a pawn with fungal flesh. Record that only when the pawn actually eats it:
    // FoodUtility.ThoughtsFromIngesting also runs during food searches, food packing and mood predictions,
    // so a postfix there recorded the event for meals nobody ate.
    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("Ingested")]
    public static class VanillaRacesExpandedFungoid_Thing_Ingested_Patch
    {

        [HarmonyPrefix]
        public static void AddPorkThought(Thing __instance, Pawn ingester)
        {
            // Vanilla logs an error and eats nothing in these two cases.
            if (__instance.Destroyed || !__instance.IngestibleNow)
            {
                return;
            }
            if (__instance.def.IsFungus && ingester?.genes?.HasActiveGene(InternalDefOf.VRE_FungalFlesh) == true)
            {
                ingester.mindState.lastHumanMeatIngestedTick = Find.TickManager.TicksGame;
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.AteHumanMeat, ingester.Named(HistoryEventArgsNames.Doer)), canApplySelfTookThoughts: true);
            }
        }
    }

}