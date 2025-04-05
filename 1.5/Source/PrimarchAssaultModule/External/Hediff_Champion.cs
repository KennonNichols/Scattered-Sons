using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using PrimarchAssault;
using Verse;
using Verse.AI;

namespace PrimarchAssault.External
{
    public class Hediff_Champion: Hediff
    {
        private List<ChampionStage> _stages = new List<ChampionStage>();
        private List<ThingDefCountClass> _droppedThings;
        private ChallengeDef _challenge;
        private bool _doesQueuePhaseTwo;



        private List<ChampionStage> _tmpStagesToRemove = new List<ChampionStage>();
        public void SetupHediff(List<ThingDefCountClass> droppedThings, List<ChampionStage> stages, ChallengeDef challenge, bool doesQueuePhaseTwo = false)
        {
            _droppedThings = droppedThings;
            _stages = stages;
            _challenge = challenge;
            _doesQueuePhaseTwo = doesQueuePhaseTwo;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref _stages, "stages", LookMode.Deep);
            Scribe_Collections.Look(ref _droppedThings, "droppedThings", LookMode.Deep);
            Scribe_Defs.Look(ref _challenge, "challenge");
            Scribe_Values.Look(ref _doesQueuePhaseTwo, "doesQueuePhaseTwo");
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);

            //Trigger all on-kill effects
            _stages?.Where(stage => stage is ChampionEventStage { triggerOnChampionKilled: true }).Do(stage => stage.Apply(pawn, pawn.Corpse.Map)) ;

<<<<<<< Updated upstream
<<<<<<< Updated upstream

            if (pawn.MapHeld != null)
            {
	            if (GameComponent_ChallengeManager.Instance.ConditionsCreatedByEvent.ContainsKey(pawn.MapHeld.info.Tile))
	            {
		            foreach (GameCondition condition in GameComponent_ChallengeManager.Instance.ConditionsCreatedByEvent[pawn.MapHeld.info.Tile].SelectMany(conditionDef => pawn.MapHeld.gameConditionManager.ActiveConditions.Where(condition => condition.def == conditionDef)))
		            {
			            condition.End();
		            }
	            }
            }
            
            
            
            if (_droppedThing != null) GenSpawn.Spawn(_droppedThing, pawn.Position, pawn.Corpse.Map);
=======
=======
>>>>>>> Stashed changes
            if (_droppedThings != null)
            {
	            foreach (ThingDefCountClass droppedThing in _droppedThings)
	            {
		            // Thing thing = ThingMaker.MakeThing(droppedThing.thingDef);
		            // thing.stackCount = 
		            for (int i = 0; i < droppedThing.count; i++)
		            {
			            ThingDef stuff = null;

			            if (droppedThing.thingDef.MadeFromStuff)
			            {
				            stuff = DefDatabase<ThingDef>.GetNamed("HP_Adamantium");
			            }
				            
			            GenPlace.TryPlaceThing(ThingMaker.MakeThing(droppedThing.thingDef, stuff), pawn.Position, pawn.Corpse.Map, ThingPlaceMode.Near);
		            }
	            }

            }
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

            if (_doesQueuePhaseTwo)
            {
                GameComponent_ChallengeManager.Instance.StartPhaseTwo(_challenge);
                Find.LetterStack.ReceiveLetter("GWPA.EscapedTitle".Translate(), "GWPA.Escaped".Translate(_challenge.championName), LetterDefOf.ThreatSmall);
            }
            else
            {
                Find.LetterStack.ReceiveLetter("GWPA.GivenUpTitle".Translate(), "GWPA.GivenUp".Translate(), LetterDefOf.PositiveEvent);
            }
            
            //Get rid of the corpse
            EffecterDefOf.Skip_EntryNoDelay.Spawn(pawn.Corpse, pawn.Corpse.MapHeld).Cleanup();
            pawn.Corpse.DeSpawn();
            
            GameComponent_ChallengeManager.Instance.RemoveActiveChampion(pawn.thingIDNumber);
        }

<<<<<<< Updated upstream
=======
        public void DamageHealthBar(float amount, DamageInfo dinfo)
        {
	        _currentHp -= amount;
        }

>>>>>>> Stashed changes
        public override void Tick()
        {
            base.Tick();
            
            if (_currentHp <= 0)
            {
	            pawn.Kill(null);
	            return;
            }
            
            float percent = GetChampionStage(out float shieldValue, out float healthValue);

            if (pawn.TryGetComp(out CompGlower glower))
            {
                glower.UpdateLit(pawn.Map);
            }
            
            if (pawn.TryGetComp(out CompShield shield))
            {
                shieldValue = shield.Energy / pawn.GetStatValue(StatDefOf.EnergyShieldEnergyMax);
            }
            
            GameComponent_ChallengeManager.Instance.HealthBar.UpdateIfWilling(pawn.thingIDNumber, healthValue, shieldValue, _challenge.healthBarRelative, _challenge.shieldBarRelative);
            
            
            
            
            if (!pawn.IsHashIntervalTick(100)) return;
            
            _tmpStagesToRemove.Clear();
            foreach (var stage in _stages.Where(stage => stage.stage > percent))
            {
                stage.Apply(pawn, pawn.Map);
                _tmpStagesToRemove.Add(stage);
            }

            _stages.RemoveAll(stage => _tmpStagesToRemove.Contains(stage));
        }


        private float GetChampionStage(out float apparelValue, out float healthValue)
        {
            apparelValue = (float)pawn.apparel.WornApparel.Select(apparel => apparel.HitPoints / (double)apparel.MaxHitPoints).Average();
            healthValue = pawn.health.summaryHealth.SummaryHealthPercent;
            
            return Math.Min(apparelValue, healthValue);
        }
    }

}