// Jeremy Anderson
// HarmonyPatch_DefGenerator_GenerateImpliedDefs_PreResolve.cs
// 2018-04-17

using System.Collections.Generic;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace RimTES
{
    [HarmonyPatch(typeof(GenStep_ScatterRuinsSimple), "ScatterAt", new [] { typeof(IntVec3), typeof(Map), typeof(int)})]
    public class HarmonyPatch_GenStep_ScatterRuinsSimple
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(GenStep_ScatterShrines), "ScatterAt", new [] { typeof(IntVec3), typeof(Map), typeof(int) })]
    public class HarmonyPatch_GenStep_ScatterShrines
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(ItemCollectionGenerator_AncientTempleContents), "Generate", new[] { typeof(ItemCollectionGeneratorParams), typeof(List<Thing>) })]
    public class HarmonyPatch_GenerateAncientTempleContents
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(ItemCollectionGenerator_AncientPodContents), "Generate", new[] { typeof(ItemCollectionGeneratorParams), typeof(List<Thing>)})]
    public class HarmonyPatch_GeneratePodContents
    {
        public static bool Prefix()
        {
            return false;
        }
    }

    [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
    public class HarmonyPatch_DefGenerator_GenerateImpliedDefs_PreResolve
    {
        public static void Prefix()
        {
            RemoveCoreFactions();
            //RemoveCoreRaces(); 
            // Corpses and Meat are generated automatically.  I would need to rewrite these methods:
            //   ThingDefGenerator_Corpses.ImpliedCorpseDefs()
            //   ThingDefGenerator_Meat.ImpliedMeatDefs()
            RemoveCoreScenarios();
            RemoveCoreResearchProjects();
            //RemoveCoreItems();
            // Apparel is generated automatically.  There must be a ThingDef:Wool
            //   PawnApparelGenerator.Reset()?
            //RemoveCoreBuildings();
            RemoveBiomes();
            //RemovePawnKinds();
            RemoveSongs();

            AddArcaneSource();
            AddManaSource();
        }

        private static void RemoveSongs()
        {
            List<SongDef> allSongDefs = DefDatabase<SongDef>.AllDefsListForReading;
            List<SongDef> songDefsToRemove = new List<SongDef>();
            foreach (SongDef songDef in allSongDefs)
            {
                if (songDef.label == null) // vanilla songs have nothing in their label.
                    songDefsToRemove.Add(songDef);
            }
            foreach (SongDef songDef in songDefsToRemove)
            {
                allSongDefs.Remove(songDef);
                Logger.DebugMessage("'" + songDef.clipPath + "' removed.");
            }
        }

        private static void AddManaSource()
        {
            List<string> bodyPartDefNamesToAddManaSourceTo = new List<string>()
            {
                "Heart","LeftLung","RightLung","LeftKidney","RightKidney","Liver","Stomach",
                "Brain"
            };
            List<BodyPartDef> allBodyPartDefs = DefDatabase<BodyPartDef>.AllDefsListForReading;
            List<BodyPartDef> bodyPartDefsToAddManaSourceTo = new List<BodyPartDef>();
            foreach (BodyPartDef bodyPartDef in allBodyPartDefs)
            {
                if (DefPresent(bodyPartDef, ref bodyPartDefNamesToAddManaSourceTo, true))
                    bodyPartDefsToAddManaSourceTo.Add(bodyPartDef);
            }
            foreach (BodyPartDef bodyPartDef in bodyPartDefsToAddManaSourceTo)
            {
                int index = allBodyPartDefs.IndexOf(bodyPartDef);
                allBodyPartDefs[index].tags.Add("ManaSource");
            }
        }

        private static void AddArcaneSource()
        {
            List<string> bodyPartDefNamesToAddArcaneSourceTo = new List<string>()
            {
                "Heart","LeftLung","RightLung","LeftKidney","RightKidney","Liver","Stomach",
                "Brain"
            };
            List<BodyPartDef> allBodyPartDefs = DefDatabase<BodyPartDef>.AllDefsListForReading;
            List<BodyPartDef> bodyPartDefsToAddArcaneSourceTo = new List<BodyPartDef>();
            foreach (BodyPartDef bodyPartDef in allBodyPartDefs)
            {
                if (DefPresent(bodyPartDef, ref bodyPartDefNamesToAddArcaneSourceTo, true))
                    bodyPartDefsToAddArcaneSourceTo.Add(bodyPartDef);
            }
            foreach (BodyPartDef bodyPartDef in bodyPartDefsToAddArcaneSourceTo)
            {
                int index = allBodyPartDefs.IndexOf(bodyPartDef);
                allBodyPartDefs[index].tags.Add("ArcaneSource");
            }
        }

        private static void RemovePawnKinds()
        {
            List<string> pawnKindNames = new List<string>()
            {
                "Colonist"
            };

            List<PawnKindDef> allPawnKinds = DefDatabase<PawnKindDef>.AllDefsListForReading;
            List<PawnKindDef> pawnKindsToRemove = new List<PawnKindDef>();
            foreach (PawnKindDef pawnKind in allPawnKinds)
            {
                if (DefPresent(pawnKind, ref pawnKindNames, true))
                    pawnKindsToRemove.Add(pawnKind);
            }
            foreach (PawnKindDef pawnKind in pawnKindsToRemove)
                allPawnKinds.Remove(pawnKind);

            PrintMissingDefs("PawnKindDef", pawnKindNames);
        }

        private static void RemoveBiomes()
        {
            List<string> biomeDefNames = new List<string>()
            {
                "Desert","ExtremeDesert","AridShrubland",
                "TemperateForest","TemperateSwamp",
                "TropicalRainforest","TropicalSwamp",
                "BorealForest","ColdBog","Tundra",
                // Advanced Biomes!
                "Savanna","Wetland","PoisonForest","Volcano","Wasteland"
            };

            List<BiomeDef> allBiomes = DefDatabase<BiomeDef>.AllDefsListForReading;
            List<BiomeDef> biomesToRemove = new List<BiomeDef>();
            foreach (BiomeDef biome in allBiomes)
            {
                if (DefPresent(biome, ref biomeDefNames, true))
                    biomesToRemove.Add(biome);
            }
            foreach (BiomeDef biome in biomesToRemove)
                allBiomes.Remove(biome);

            PrintMissingDefs("BiomeDef", biomeDefNames);
        }

        private static void RemoveCoreBuildings()
        {
            List<string> buildingDefNames = new List<string>()
            {
                "AncientConcreteBarrier","AncientLamppost",
                "SculptureSmall","SculptureLarge","SculptureGrand","Snowman",
                "CrashedPsychicEmanatorShipPart","CrashedPoisonShipPart","ShipChunk",
                "SleepingSpot","Bed","DoubleBed","RoyalBed","HospitalBed","Bedroll","BedrollDouble","AnimalSleepingSpot","AnimalSleepingBox","AnimalBed","Stool","DiningChair","Armchair","EndTable","Table1x2c","Table2x2c","Table2x4c","Table3x3c","PlantPot","TorchLamp","StandingLamp","StandingLamp_Red","StandingLamp_Green","StandingLamp_Blue","SunLamp","Shelf","Dresser",
                "HorseshoesPin","HoopstoneRing","ChessTable","BilliardsTable","PokerTable","TubeTelevision","FlatscreenTelevision","MegascreenTelevision","Telescope",
                "OrbitalTradeBeacon","CommsConsole","FirefoamPopper","MoisturePump","GroundPenetratingScanner","LongRangeMineralScanner","PodLauncher","TransportPod","MultiAnalyzer","VitalsMonitor","ToolCabinet","Grave","Sarcophagus","CryptosleepCasket","AncientCryptosleepCasket","MarriageSpot","PartySpot","CaravanPackingSpot",
                "CollapsedRocks","MineableSteel","MineableSilver","MineableGold","MineableUranium","MineablePlasteel","MineableJade","MineableComponents","SteamGeyser","Hive","GlowPod",
                "PowerConduit","PowerSwitch","WoodFiredGenerator","ChemfuelPoweredGenerator","Battery","SolarGenerator","GeothermalGenerator",
                "WindTurbine",
                "CraftingSpot","TableSculpting","TableButcher","HandTailoringBench","ElectricTailoringBench","FueledSmithy","ElectricSmithy","TableMachining","ElectricStove","FueledStove","TableStonecutter","Brewery","DrugLab","ElectricSmelter","Refinery","ComponentAssemblyBench","SimpleResearchBench","HiTechResearchBench","ElectricCrematorium","HydroponicsBasin","FermentingBarrel","DeepDrill","NutrientPasteDispenser","Hopper",
                "Sandbags","TurretGun","Bullet_TurretImprovised","Gun_TurretImprovised","Turret_Mortar","Artillery_Mortar","TrapDeadfall","Building_TrapExplosive","TrapIED_HighExplosive","TrapIED_Incendiary","TrapIED_EMP","TrapIED_Firefoam","TrapIED_AntigrainWarhead",
                "Ship_Beam","Ship_CryptosleepCasket","Ship_ComputerCore","Ship_Reactor","Ship_Engine","Ship_SensorCluster",
                "PsychicEmanator","VanometricPowerCell","InfiniteChemreactor",
                "Door","Autodoor","Wall",
                "Campfire","PassiveCooler","Heater","Cooler","Vent"
            };

            List<ThingDef> allBuildings = DefDatabase<ThingDef>.AllDefsListForReading;
            List<ThingDef> buildingsToRemove = new List<ThingDef>();
            foreach (ThingDef building in allBuildings)
            {
                if (DefPresent(building, ref buildingDefNames, true))
                    buildingsToRemove.Add(building);
            }
            foreach (ThingDef building in buildingsToRemove)
                allBuildings.Remove(building);

            PrintMissingDefs("ThingDef", buildingDefNames);
        }

        private static void RemoveCoreItems()
        {
            List<string> itemDefNames = new List<string>()
            {
                "PsychicInsanityLance","PsychicShockLance","PsychicAnimalPulser","PsychicSoothePulser",
                "SimpleProstheticLeg","SimpleProstheticArm","BionicEye","BionicArm","BionicLeg","PowerClaw","ScytherBlade",
                "AIPersonaCore","MechSerumNeurotrainer","MechSerumHealer","MechSerumResurrector","TechprofSubpersonaCore","ThrumboHorn","ElephantTusk",
                "MealSurvivalPack","MealNutrientPaste","MealSimple","MealFine","MealLavish","Kibble","Pemmican",
                "Joywire","Painstopper",
                "Chocolate",
                "Milk","InsectJelly","EggChickenUnfertilized","EggChickenFertilized","EggCobraFertilized","EggIguanaFertilized","EggTortoiseFertilized","EggCassowaryFertilized","EggEmuFertilized","EggOstrichFertilized","EggTurkeyFertilized",
                "HerbalMedicine","Medicine","GlitterworldMedicine","Component","AdvancedComponent","Neutroamine","Chemfuel",
                "RawPotatoes","RawFungus","RawRice","RawAgave","RawCorn","RawBerries","Hay","RawHops","PsychoidLeaves","SmokeleafLeaves",
                "Shell_HighExplosive","Bullet_Shell_HighExplosive","Shell_Incendiary","Bullet_Shell_Incendiary","Shell_EMP","Bullet_Shell_EMP","Shell_Firefoam","Bullet_Shell_Firefoam","Shell_AntigrainWarhead","Bullet_Shell_AntigrainWarhead"/*,
                "Silver","Gold","Steel"*/,"Plasteel"/*,"WoodLog","Uranium"*/,"Jade",/*"Cloth",*/"Synthread","DevilstrandCloth","Hyperweave","WoolMegasloth","WoolMuffalo",/*"WoolCamel",*/"WoolAlpaca",
                "UnfinishedSculpture","UnfinishedWeapon","UnfinishedTechArmor","UnfinishedMetallicTechArmor","UnfinishedApparel","UnfinishedBelt","UnfinishedComponent","MinifiedSculpture","MinifiedFurniture"
            };

            List<ThingDef> allItems = DefDatabase<ThingDef>.AllDefsListForReading;
            List<ThingDef> itemsToRemove = new List<ThingDef>();
            foreach (ThingDef item in allItems)
            {
                if (DefPresent(item, ref itemDefNames, true))
                    itemsToRemove.Add(item);
            }
            foreach (ThingDef item in itemsToRemove)
                allItems.Remove(item);

            PrintMissingDefs("ThingDef", itemDefNames);
        }

        private static void RemoveCoreResearchProjects()
        {
            List<string> projectDefNames = new List<string>()
            {
                "RecurveBow","Pemmican","PassiveCooler","Devilstrand","PsychoidBrewing","Brewing","Bedrolls","Beds","CarpetMaking","Smithing","Stonecutting","ComplexClothing","DrugProduction","Electricity",
                "PsychiteRefining","WakeUpProduction","GoJuiceProduction","PenoxycylineProduction","LongBlades","Greatbows","Batteries","Refining","NutrientPaste","SolarPanels","AirConditioning","Autodoors","Hydroponics","ElectricSmelting","PackagedSurvivalMeal","ElectricCremation","ColoredLights","Machining","IEDs","IEDIncendiary","Mortars","Gunsmithing","BlowbackOperation","GasOperation","PrecisionRifling","SmokepopBelt","MicroelectronicsBasics","TubeTelevision","GunTurrets",
                "Firefoam","MoisturePump","GeothermalPower","HospitalBed","TransportPod","MedicineProduction","MultiAnalyzer","LongRangeMineralScanner",
                "GroundPenetratingScanner","DeepDrilling","VitalsMonitor","ComponentAssembly","MultibarrelWeapons","Cryptosleep","PoweredArmor","ShieldBelt","ChargedShot",
                "ShipBasics","ShipCryptosleep","ShipReactor","ShipEngine","ShipComputerCore","ShipSensorCluster"
            };

            List<ResearchProjectDef> allProjects = DefDatabase<ResearchProjectDef>.AllDefsListForReading;
            List<ResearchProjectDef> projectsToRemove = new List<ResearchProjectDef>();
            foreach (ResearchProjectDef project in allProjects)
            {
                if (DefPresent(project, ref projectDefNames, true))
                    projectsToRemove.Add(project);
            }
            foreach (ResearchProjectDef project in projectsToRemove)
                allProjects.Remove(project);

            PrintMissingDefs("ResearchProjectDef", projectDefNames);
        }

        private static void RemoveCoreScenarios()
        {
            List<string> scenarioDefNames = new List<string>()
            {
                "Crashlanded","LostTribe","TheRichExplorer"
            };

            List<ScenarioDef> allScenarios = DefDatabase<ScenarioDef>.AllDefsListForReading;
            List<ScenarioDef> scenariosToRemove = new List<ScenarioDef>();
            foreach (ScenarioDef scenario in allScenarios)
            {
                if (DefPresent(scenario, ref scenarioDefNames, true))
                    scenariosToRemove.Add(scenario);
            }
            foreach (ScenarioDef scenario in scenariosToRemove)
                allScenarios.Remove(scenario);

            PrintMissingDefs("ScenarioDef", scenarioDefNames);
        }

        private static void RemoveCoreRaces()
        {
            // A race is considered a Thing.
            List<string> thingDefNames = new List<string>()
            {
                "Human"
            };

            List<ThingDef> allThings = DefDatabase<ThingDef>.AllDefsListForReading;
            List<ThingDef> racesToRemove = new List<ThingDef>();
            foreach (ThingDef thing in allThings)
            {
                if (DefPresent(thing, ref thingDefNames, true))
                    racesToRemove.Add(thing);
            }
            foreach (ThingDef thing in racesToRemove)
                allThings.Remove(thing);

            PrintMissingDefs("ThingDef", thingDefNames);
        }

        private static void RemoveCoreFactions()
        {
            List<string> factionDefNames = new List<string>()
            {
                "Mechanoid","Insect","Spacer","SpacerHostile",
                "Outlander","Tribe","Pirate",
                "PlayerColony","PlayerTribe"
            };

            List<FactionDef> allFactions = DefDatabase<FactionDef>.AllDefsListForReading;
            List<FactionDef> factionsToRemove = new List<FactionDef>();
            foreach (FactionDef faction in allFactions)
            {
                if (DefPresent(faction, ref factionDefNames, true))
                    factionsToRemove.Add(faction);
            }
            foreach (FactionDef faction in factionsToRemove)
                allFactions.Remove(faction);
            PrintMissingDefs("FactionDef", factionDefNames);
        }

        /// <summary>
        /// DEBUG ONLY - Verify that all defNames were found and removed.
        /// </summary>
        /// <param name="defType">Type of Def being checked.</param>
        /// <param name="defNames">List of defNames that were supposed to be found and removed.</param>
        private static void PrintMissingDefs(string defType, List<string> defNames)
        {
#if DEBUG
            foreach (string defName in defNames)
            {
                Logger.DebugWarning(defType + ": " + defName + " was not removed.");
            }
#endif
        }

        /// <summary>
        /// Compares the defName of the def being passed to a list of valid defNames that should be removed.
        /// </summary>
        /// <param name="def">The Def being checked.</param>
        /// <param name="defNames">Referenced list of defNames.</param>
        /// <param name="shouldRemove">Removes the match from defNames.</param>
        /// <returns></returns>
        private static bool DefPresent(Def def, ref List<string> defNames, bool shouldRemove = false)
        {
            foreach (string defName in defNames)
            {
                if (defName == def.defName)
                {
                    if (shouldRemove)
                        defNames.Remove(defName);

                    return true;
                }
            }
            return false;
        }

    }
}