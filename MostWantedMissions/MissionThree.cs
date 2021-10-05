using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionThree : Mission
{
    public override bool IsMostWanted => true;
    public override bool IsJokerMission => false;

    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    enum Enemies
    {
        Madrazo,
        HouseEntranceGuard,
        SideFrontDoor,
        SideFrontDoorLeft01,
        SideFrontDoorLeft02,
        GolfGuard01,
        GolfGuard02,
        GolfGuard03,
        ChefGuard,
        PoolGuard01,
        PoolGuard02,
        GardenGuard,
        SideBackGuard01,
        SideBackGuard02,
        SideBackGuard03,
        GarageGuard01,
        GarageGuard02,
        SideBackGuard,
        FrontGardenGuard
    }

    enum Neutrals
    {
        Patricia,
        Chef
    }

    Vector3 objectiveLocation;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    public override Blip ObjectiveLocationBlip { get; set; }
    Objectives currentObjective;

    public MissionThree()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        neutralsRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;

        objectiveLocation = MostWantedMissions.MISSION_THREE_LOCATION;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200f))
                    {
                        return;
                    }
                    Music.IncreaseIntensity();
                    ObjectiveLocationBlip.Delete();
                    var peds = MostWantedMissions.InitializeMissionThreePeds();
                    var neutrals = MostWantedMissions.InitializeMissionThreeCivilianPeds();
                    peds = MissionWorld.PedListLoadLoop(peds, MostWantedMissions.InitializeMissionThreePeds);
                    neutrals = MissionWorld.PedListLoadLoop(neutrals, MostWantedMissions.InitializeMissionTwoCivilianPeds);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
                    }
                    for (var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, true));
                    }
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~targets~w~.", 8000);
                    currentObjective = Objectives.KillTargets;
                    StartScenarios();
                    break;
                }
            case Objectives.KillTargets:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    else
                    {
                        currentObjective = Objectives.Completed;
                    }
                    break;
                }
            case Objectives.Completed:
                {
                    RemoveVehiclesAndNeutrals();
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is already in your account.");
                    Game.Player.Money += 15000;
                    Game.Player.WantedLevel = 3;
                    currentObjective = Objectives.None;
                    MissionWorld.CompleteMission();
                    MissionWorld.script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        Music.StopMusic();
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
                enemies[i].Delete();
                aliveEnemies.RemoveAt(i);
                if (enemies[i].GetPed().Killer == Game.Player.Character)
                {
                    Progress.enemiesKilledCount += 1;
                }
            }
        }
        enemies = aliveEnemies;
    }

    protected override void RemoveVehiclesAndNeutrals()
    {
        foreach (MissionPed neutral in neutralPeds)
        {
            neutral.Delete();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200f))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartHeistMusic();
        currentObjective = Objectives.GoToLocation;
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.ShowRoute = true;
        ObjectiveLocationBlip.Name = "Wanted suspect location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanated suspect", "Ok, i tracked them down, i'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~wanted suspect~w~.");

        MissionWorld.script.Tick += MissionTick;
        return true;
    }

    void StartScenarios()
    {
        enemies[(int)Enemies.Madrazo].GetTask().ChatTo(neutralPeds[(int)Neutrals.Patricia].GetPed());
        neutralPeds[(int)Neutrals.Patricia].GetTask().StartScenario("WORLD_HUMAN_MAID_CLEAN", 0);

        enemies[(int)Enemies.FrontGardenGuard].GetTask().GuardCurrentPosition();
        enemies[(int)Enemies.HouseEntranceGuard].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.SideFrontDoor].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.SideFrontDoorLeft01].GetTask().UseMobilePhone();
        enemies[(int)Enemies.SideFrontDoorLeft02].GetTask().StartScenario("WORLD_HUMAN_LEANING", 0);

        enemies[(int)Enemies.ChefGuard].GetTask().ChatTo(neutralPeds[(int)Neutrals.Chef].GetPed());
        neutralPeds[(int)Neutrals.Chef].GetTask().ChatTo(enemies[(int)Enemies.ChefGuard].GetPed());

        enemies[(int)Enemies.GardenGuard].GetTask().GuardCurrentPosition();
        enemies[(int)Enemies.GolfGuard01].GetTask().UseMobilePhone();
        enemies[(int)Enemies.GolfGuard02].GetTask().StartScenario("WORLD_HUMAN_GOLF_PlAYER", 0);
        enemies[(int)Enemies.GolfGuard03].GetTask().StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
        enemies[(int)Enemies.GarageGuard01].GetTask().StartScenario("WORLD_HUMAN_VEHICLE_MECHANIC", 0);
        enemies[(int)Enemies.GarageGuard02].GetTask().ChatTo(enemies[(int)Enemies.GarageGuard01].GetPed());
        enemies[(int)Enemies.PoolGuard01].GetTask().ChatTo(enemies[(int)Enemies.PoolGuard01].GetPed());
        enemies[(int)Enemies.PoolGuard02].GetTask().UseMobilePhone();
        enemies[(int)Enemies.SideBackGuard01].GetTask().StartScenario("WOLRD_HUMAN_DRUG_DEALER", 0);
        enemies[(int)Enemies.SideBackGuard02].GetTask().StartScenario("WORLD_HUMAN_LEANING", 0);
        enemies[(int)Enemies.SideBackGuard03].GetTask().UseMobilePhone();
        enemies[(int)Enemies.SideBackGuard].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
    }
}