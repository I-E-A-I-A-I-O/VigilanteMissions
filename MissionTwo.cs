using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionTwo : Mission
{
    public override bool IsMostWanted => true;

    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    enum Enemies
    {
        LietWithTheTwoHookers,
        LietWithOneHooker,
        GuardRecordingStrippers,
        BossDrinkingTopDeck,
        GuardWithBartender,
        BossWife,
        LietWatchingBoss,
        TvRoomGuard04,
        TvRoomGuard01,
        ShipCaptain,
        CaptainGuard,
        StairsGuard,
        BossDrinkingBossRoom,
        BossTalking01,
        BossTalking02,
        BossRecordingStripper,
        BossRoomGuard,
        GuardDrinking,
        LedgeGuard,
        LowerLedgeGuard01,
        LowerLedgeGuard02,
        BayGuard,
        TvRoomGuard02,
        TvRoomGuard03
    }

    enum Neutrals
    {
        HookerTalkingToLiet01,
        HookerTalkingToLiet02,
        HookerLookingAtPhone,
        StripperDancing01,
        StripperDancing02,
        Bartender,
        StripperBeingRecorded,
        HookerWithGuard01,
        HookerWithGuard02,
        StripperTvRoom
    }

    enum Vehicles
    {
        Helicopter
    }

    Objectives currentObjective;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    Vector3 targetLocation;
    public override Blip ObjectiveLocationBlip 
    {
        get => ObjectiveLocationBlip;
        set => ObjectiveLocationBlip = value;
    }
    Vector3 helicopterDestination;
    RelationshipGroup neutralsRelGroup;
    RelationshipGroup enemiesRelGroup;
    bool messageShown = false;

    public MissionTwo()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        neutralsRelGroup = MissionWorld.RELATIONSHIP_MISSION_PEDESTRIAN;

        targetLocation = MostWantedMissions.MISSION_TWO_LOCATION;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(targetLocation, 200f))
                    {
                        return;
                    }
                    Music.IncreaseIntensity();
                    ObjectiveLocationBlip.Delete();
                    vehicles = MostWantedMissions.InitializeMissionTwoVehicles();
                    var peds = MostWantedMissions.InitializeMissionTwoPeds();
                    var neutrals = MostWantedMissions.InitializeMissionTwoCivilianPeds();
                    vehicles = MissionWorld.VehicleListLoadLoop(vehicles, MostWantedMissions.InitializeMissionTwoVehicles);
                    peds = MissionWorld.PedListLoadLoop(peds, MostWantedMissions.InitializeMissionTwoPeds);
                    neutrals = MissionWorld.PedListLoadLoop(neutrals, MostWantedMissions.InitializeMissionTwoCivilianPeds);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        if (i == (int)Enemies.CaptainGuard)
                        {
                            enemies.Add(new MissionPed(peds[i], enemiesRelGroup, false, true));
                            continue;
                        }
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

    public override void RemoveDeadEnemies()
    {
        var aliveEnemies = enemies;
        for (var i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsDead())
            {
                enemies[i].Delete();
                aliveEnemies.RemoveAt(i);
            }
        }
        enemies = aliveEnemies;
    }

    public override void RemoveVehiclesAndNeutrals()
    {
        foreach (Vehicle vehicle in vehicles)
        {
            vehicle.MarkAsNoLongerNeeded();
        }
        foreach (MissionPed neutral in neutralPeds)
        {
            neutral.Delete();
        }
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(targetLocation, 200f))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartHeistMusic();
        currentObjective = Objectives.GoToLocation;
        ObjectiveLocationBlip = World.CreateBlip(targetLocation, 150f);
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
        helicopterDestination = MostWantedMissions.MISSION_FOUR_FAIL_LOCATION;

        enemies[(int)Enemies.LietWatchingBoss].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.BossWife].GetTask().StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
        enemies[(int)Enemies.BossDrinkingTopDeck].GetTask().StartScenario("WORLD_HUMAN_DRINKING", 0);

        var taskSequence = new TaskSequence();
        taskSequence.AddTask.Wait(5000);
        taskSequence.AddTask.EnterVehicle(vehicles[(int)Vehicles.Helicopter], VehicleSeat.Driver, -1, 5);
        enemies[(int)Enemies.CaptainGuard].GetTask().PerformSequence(taskSequence);

        MissionWorld.script.Tick += CheckHelicopter;

        enemies[(int)Enemies.LietWithTheTwoHookers].GetTask().ChatTo(neutralPeds[(int)Neutrals.HookerTalkingToLiet01].GetPed());
        neutralPeds[(int)Neutrals.HookerTalkingToLiet01].GetTask().ChatTo(enemies[(int)Enemies.LietWithTheTwoHookers].GetPed());
        neutralPeds[(int)Neutrals.HookerTalkingToLiet02].GetTask().ChatTo(enemies[(int)Enemies.LietWithTheTwoHookers].GetPed());

        enemies[(int)Enemies.LietWithOneHooker].GetTask().ChatTo(neutralPeds[(int)Neutrals.HookerLookingAtPhone].GetPed());
        neutralPeds[(int)Neutrals.HookerLookingAtPhone].GetTask().UseMobilePhone();

        enemies[(int)Enemies.TvRoomGuard01].GetTask().StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.TvRoomGuard02].GetTask().StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.TvRoomGuard03].GetTask().StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.TvRoomGuard04].GetTask().StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);
        neutralPeds[(int)Neutrals.StripperTvRoom].GetTask().StartScenario("WORLD_HUMAN_YOGA", 0);

        enemies[(int)Enemies.ShipCaptain].GetTask().StartScenario("WOLRD_HUMAN_BINOCULARS", 0);

        enemies[(int)Enemies.StairsGuard].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);

        enemies[(int)Enemies.BossRecordingStripper].GetTask().StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);
        neutralPeds[(int)Neutrals.StripperBeingRecorded].GetTask().StartScenario("WORLD_HUMAN_YOGA", 0);

        neutralPeds[(int)Neutrals.StripperDancing01].GetTask().StartScenario("WORLD_HUMAN_YOGA", 0);
        neutralPeds[(int)Neutrals.StripperDancing02].GetTask().StartScenario("WORLD_HUMAN_YOGA", 0);
        enemies[(int)Enemies.GuardRecordingStrippers].GetTask().StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);

        enemies[(int)Enemies.GuardDrinking].GetTask().StartScenario("WORLD_HUMAN_DRINKING", 0);
        neutralPeds[(int)Neutrals.HookerWithGuard01].GetTask().StartScenario("WOLRD_HUMAN_PARTYING", 0);
        neutralPeds[(int)Neutrals.HookerWithGuard02].GetTask().ChatTo(enemies[(int)Enemies.GuardDrinking].GetPed());

        enemies[(int)Enemies.BossDrinkingBossRoom].GetTask().StartScenario("WOLRD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.BossTalking01].GetTask().ChatTo(enemies[(int)Enemies.BossTalking02].GetPed());
        enemies[(int)Enemies.BossTalking02].GetTask().ChatTo(enemies[(int)Enemies.BossTalking01].GetPed());

        enemies[(int)Enemies.LedgeGuard].GetTask().StartScenario("WORLD_HUMAN_BINOCULARS", 0);

        enemies[(int)Enemies.LowerLedgeGuard01].GetTask().StartScenario("WOLRD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.LowerLedgeGuard02].GetTask().ChatTo(enemies[(int)Enemies.LowerLedgeGuard01].GetPed());

        enemies[(int)Enemies.BayGuard].GetTask().StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
    }

    void CheckHelicopter(Object o, EventArgs e)
    {
        if (!MissionWorld.isMissionActive || enemies.Count == 0)
        {
            MissionWorld.script.Tick -= CheckHelicopter;
            return;
        }
        if (vehicles[(int)Vehicles.Helicopter].IsDead)
        {
            if (vehicles[(int)Vehicles.Helicopter].AttachedBlip != null)
            {
                vehicles[(int)Vehicles.Helicopter].AttachedBlip.Delete();
            }
            MissionWorld.script.Tick -= CheckHelicopter;
            return;
        }
        if (messageShown)
        {
            if (vehicles[(int)Vehicles.Helicopter].IsInRange(helicopterDestination, 100))
            {
                MissionWorld.QuitMission();
                GTA.UI.Screen.ShowSubtitle("~r~Mission failed, a target escaped!");
                MissionWorld.script.Tick -= CheckHelicopter;
            }
        }
        else if (vehicles[(int)Vehicles.Helicopter].Driver != null && vehicles[(int)Vehicles.Helicopter].Driver != Game.Player.Character)
        {
            Function.Call(Hash.TASK_HELI_MISSION, enemies[(int)Enemies.CaptainGuard].GetPed(), vehicles[(int)Vehicles.Helicopter], 0, 0, helicopterDestination.X, helicopterDestination.Y, helicopterDestination.Z, 4, 50.0, 10.0, (helicopterDestination - vehicles[(int)Vehicles.Helicopter].Position).ToHeading(), -1, -1, -1, 32);
            vehicles[(int)Vehicles.Helicopter].AddBlip();
            vehicles[(int)Vehicles.Helicopter].AttachedBlip.Sprite = BlipSprite.HelicopterAnimated;
            vehicles[(int)Vehicles.Helicopter].AttachedBlip.Color = BlipColor.Red;
            vehicles[(int)Vehicles.Helicopter].AttachedBlip.Name = "Wanted criminal";

            GTA.UI.Screen.ShowSubtitle("A ~r~target ~w~is trying to escape in a ~r~helicopter~w~!");

            messageShown = true;
        }
    }
}
