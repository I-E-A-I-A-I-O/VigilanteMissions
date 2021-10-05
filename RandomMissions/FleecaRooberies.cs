using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class FleecaRooberies : Mission
{
    public override bool IsMostWanted => false;
    public override bool IsJokerMission => false;

    enum Objectives
    {
        GoToLocation,
        KillTargets,
        None
    }

    Vector3 objectiveLocation;
    public override Blip ObjectiveLocationBlip { get; set; }
    int missionIndex;
    bool doorsUnlocked = false;
    Objectives currentObjective;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup hostagesRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> hostages = new List<MissionPed>();

    public FleecaRooberies()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        hostagesRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    var enemyPeds = FleecaRobberiesInit.SelectLocationRobbers(missionIndex);
                    var hostagePeds = FleecaRobberiesInit.SelectLocationHostages(missionIndex);
                    enemyPeds = MissionWorld.PedListLoadLoop(enemyPeds, FleecaRobberiesInit.SelectLocationRobbers, missionIndex);
                    hostagePeds = MissionWorld.PedListLoadLoop(hostagePeds, FleecaRobberiesInit.SelectLocationHostages, missionIndex);
                    for (var i = 0; i < enemyPeds.Count; i++)
                    {
                        enemies.Add(new MissionPed(enemyPeds[i], enemiesRelGroup));
                        enemies[i].GetPed().Accuracy = 80;
                        enemies[i].GetPed().Armor = 100;
                        Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemies[i].GetPed(), 1);
                        enemies[i].ShowBlip();
                        enemies[i].GetTask().StartScenario("WORLD_HUMAN_STAND_IMPATIENT", 0);
                    }
                    for (var i = 0; i < hostagePeds.Count; i++)
                    {
                        hostages.Add(new MissionPed(hostagePeds[i], hostagesRelGroup, true));
                        hostages[i].GetTask().HandsUp(1800000);
                        hostages[i].GetPed().BlockPermanentEvents = true;
                    }
                    ObjectiveLocationBlip.Delete();
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~bank robbers~w~.", 8000);
                    currentObjective = Objectives.KillTargets;
                    break;
                }
            case Objectives.KillTargets:
                {
                    if (Game.Player.Character.IsInRange(objectiveLocation, 20) && !doorsUnlocked)
                    {
                        var door1 = World.GetClosestProp(objectiveLocation, 100, new Model("v_ilev_genbankdoor1"));
                        var door2 = World.GetClosestProp(objectiveLocation, 100, new Model("v_ilev_genbankdoor2"));
                        Function.Call(Hash._DOOR_CONTROL, Game.GenerateHash("v_ilev_genbankdoor1"), door1.Position.X, door1.Position.Y, door1.Position.Z, false, 0, 50, 0);
                        Function.Call(Hash._DOOR_CONTROL, Game.GenerateHash("v_ilev_genbankdoor2"), door2.Position.X, door2.Position.Y, door2.Position.Z, false, 0, 50, 0);
                        doorsUnlocked = true;
                    }
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        GTA.UI.Screen.ShowSubtitle("Crime scene cleared.", 8000);
                        Game.Player.Money += 2500;
                        currentObjective = Objectives.None;
                        RemoveVehiclesAndNeutrals();
                        MissionWorld.CompleteMission();
                        MissionWorld.script.Tick -= MissionTick;
                    }
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        currentObjective = Objectives.None;
        MissionWorld.script.Tick -= MissionTick;
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        if (ObjectiveLocationBlip.Exists())
        {
            ObjectiveLocationBlip.Delete();
        }
        RemoveVehiclesAndNeutrals();
    }

    protected override void RemoveDeadEnemies()
    {
        var aliveEnemies = enemies;
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                if (enemies[i].GetPed().Killer == Game.Player.Character)
                {
                    Progress.enemiesKilledCount += 1;
                }
                enemies[i].Delete();
                aliveEnemies.RemoveAt(i);
            }
        }
        enemies = aliveEnemies;
    }

    protected override void RemoveVehiclesAndNeutrals()
    {
        foreach (MissionPed hostage in hostages)
        {
            hostage.Delete();
        }
    }

    public override bool StartMission()
    {
        var random = new Random();
        do
        {
            missionIndex = random.Next(0, 6);
            FleecaRobberiesInit.SelectLocation(missionIndex, out objectiveLocation);
        } while (Game.Player.Character.IsInRange(objectiveLocation, 200));

        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
        ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.Name = "Fleeca robbery";
        ObjectiveLocationBlip.ShowRoute = true;

        currentObjective = Objectives.GoToLocation;
        MissionWorld.script.Tick += MissionTick;
        return true;
    }
}
