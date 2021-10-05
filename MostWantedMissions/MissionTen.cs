using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionTen : Mission
{
    public override bool IsMostWanted => true;
    public override bool IsJokerMission => false;

    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Complete,
        None
    }

    enum Enemies
    {
        OuterEntrance01,
        OuterEntrance02,
        InnerEntrance01,
        BathroomGuard,
        DanceFloorDoor,
        InnerEntrance02,
        InnerEntrance03,
        OuterBar01,
        BarDoor,
        DanceFloorSeats,
        CashierGuard,
        InnerBar01,
        InnerBar02,
        Target,
        TargetBody01,
        TargetBody02,
        TargetBody03,
        DanceFloorDouble,
        DJ,
        OuterBar02
    }

    Objectives currentObjective;
    Vector3 objectiveLocation;
    public override Blip ObjectiveLocationBlip { get; set; }
    List<MissionPed> enemies = new List<MissionPed>();
    Ped hooker;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup hookerRelGroup;

    public MissionTen()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        hookerRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;

        objectiveLocation = MostWantedMissions.MISSION_TEN_LOCATION;
    }

    protected override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch(currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    Music.IncreaseIntensity();
                    ObjectiveLocationBlip.Delete();
                    var peds = MostWantedMissions.InitializeMissionTenEnemies();
                    hooker = MostWantedMissions.InitializeMissionTenNeutralPed();
                    peds = MissionWorld.PedListLoadLoop(peds, MostWantedMissions.InitializeMissionTenEnemies);
                    hooker = (Ped)MissionWorld.EntityLoadLoop(hooker, MostWantedMissions.InitializeMissionTenNeutralPed);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
                    }
                    hooker.RelationshipGroup = hookerRelGroup;
                    StartScenearios();

                    GTA.UI.Screen.ShowSubtitle("Kill ~r~Billy Russo and his gang~w~.", 8000);
                    currentObjective = Objectives.KillTargets;
                    break;
                }
            case Objectives.KillTargets:
                {
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    } else
                    {
                        currentObjective = Objectives.Complete;
                    }
                    break;
                }
            case Objectives.Complete:
                {
                    MissionWorld.CompleteMission();
                    RemoveVehiclesAndNeutrals();
                    currentObjective = Objectives.None;
                    Game.Player.Money += 18000;
                    Game.Player.WantedLevel = 3;
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is in your account.");
                    MissionWorld.script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        Music.StopMusic();
        if (hooker != null)
        {
            hooker.MarkAsNoLongerNeeded();
        }
        if (ObjectiveLocationBlip != null)
        {
            ObjectiveLocationBlip.Delete();
        }
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        MissionWorld.script.Tick -= MissionTick;
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
        if (hooker != null)
        {
            hooker.MarkAsNoLongerNeeded();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartFunkyTwo();
        ObjectiveLocationBlip = World.CreateBlip(objectiveLocation);
        ObjectiveLocationBlip.DisplayType = BlipDisplayType.BothMapSelectable;
        ObjectiveLocationBlip.Color = BlipColor.Yellow;
        ObjectiveLocationBlip.Name = "Wanted suspect location";
        ObjectiveLocationBlip.ShowRoute = true;

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "Found him, ~r~Billy Russo and his gang~w~ are hanging out at the ~y~Bahama nighclub~w~.");

        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Bahama nightclub~w~.", 8000);

        currentObjective = Objectives.GoToLocation;
        MissionWorld.script.Tick += MissionTick;
        return true;
    }

    void StartScenearios()
    {
        hooker.Task.UseMobilePhone();
        enemies[(int)Enemies.BarDoor].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.BathroomGuard].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.CashierGuard].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DanceFloorDoor].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DanceFloorDouble].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DanceFloorSeats].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.DJ].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.OuterBar01].GetTask().StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.OuterBar02].GetTask().StartScenario("WORLD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.InnerEntrance01].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.InnerEntrance02].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.InnerEntrance03].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.InnerBar01].GetTask().ChatTo(enemies[(int)Enemies.InnerBar02].GetPed());
        enemies[(int)Enemies.InnerBar02].GetTask().ChatTo(enemies[(int)Enemies.InnerBar01].GetPed());
        enemies[(int)Enemies.OuterEntrance01].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.OuterEntrance02].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.Target].GetTask().StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
        enemies[(int)Enemies.TargetBody01].GetTask().ChatTo(enemies[(int)Enemies.Target].GetPed());
        enemies[(int)Enemies.TargetBody02].GetTask().ChatTo(enemies[(int)Enemies.Target].GetPed());
        enemies[(int)Enemies.TargetBody03].GetTask().ChatTo(enemies[(int)Enemies.Target].GetPed());
    }
}
