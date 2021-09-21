using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionTwo : Mission
{
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
    MissionWorld missionWorld;
    MostWantedMissions mostWantedMissions;
    Script script;
    Music music;
    Blip objectiveLocationBlip;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    Vector3 helicopterDestination;
    bool messageShown = false;

    public MissionTwo(Script script, MissionWorld missionWorld, RelationshipGroup enemiesRelGroup, RelationshipGroup neutralsRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemiesRelGroup = enemiesRelGroup;
        this.neutralsRelGroup = neutralsRelGroup;

        mostWantedMissions = new MostWantedMissions();
        targetLocation = mostWantedMissions.MISSION_TWO_LOCATION;

        music = new Music();
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
                    music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    vehicles = mostWantedMissions.InitializeMissionTwoVehicles();
                    var peds = mostWantedMissions.InitializeMissionTwoPeds();
                    var neutrals = mostWantedMissions.InitializeMissionTwoCivilianPeds();
                    Script.Wait(1000);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup, targetLocation, script));
                    }
                    for (var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, targetLocation, script, true));
                    }
                    foreach (MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
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
                    missionWorld.CompleteMission();
                    script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        music.StopMusic();
        currentObjective = Objectives.None;
        script.Tick -= MissionTick;
        foreach (MissionPed enemy in enemies)
        {
            enemy.Delete();
        }
        if (objectiveLocationBlip.Exists())
        {
            objectiveLocationBlip.Delete();
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
        music.StartHeistMusic();
        currentObjective = Objectives.GoToLocation;
        objectiveLocationBlip = World.CreateBlip(targetLocation, 150f);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.ShowRoute = true;
        objectiveLocationBlip.Name = "Wanted suspect location";

        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanated suspect", "Ok, i tracked them down, i'm sending you the location.");
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~wanted suspect~w~.");

        script.Tick += MissionTick;
        return true;
    }

    void StartScenarios()
    {
        helicopterDestination = mostWantedMissions.MISSION_FOUR_FAIL_LOCATION;

        enemies[(int)Enemies.BossDrinkingTopDeck].ped.Task.EnterVehicle(vehicles[(int)Vehicles.Helicopter], VehicleSeat.Passenger);
        enemies[(int)Enemies.BossWife].ped.Task.EnterVehicle(vehicles[(int)Vehicles.Helicopter], VehicleSeat.ExtraSeat1);

        Function.Call(Hash.TASK_HELI_MISSION, enemies[(int)Enemies.LietWatchingBoss].ped, vehicles[(int)Vehicles.Helicopter], 0, 0, helicopterDestination.X, helicopterDestination.Y, helicopterDestination.Z, 4, 50.0, 10.0, (helicopterDestination - vehicles[(int)Vehicles.Helicopter].Position).ToHeading(), -1, -1, -1, 32);

        script.Tick += CheckHelicopter;

        enemies[(int)Enemies.LietWithTheTwoHookers].ped.Task.ChatTo(neutralPeds[(int)Neutrals.HookerTalkingToLiet01].ped);
        neutralPeds[(int)Neutrals.HookerTalkingToLiet01].ped.Task.ChatTo(enemies[(int)Enemies.LietWithTheTwoHookers].ped);
        neutralPeds[(int)Neutrals.HookerTalkingToLiet02].ped.Task.ChatTo(enemies[(int)Enemies.LietWithTheTwoHookers].ped);

        enemies[(int)Enemies.LietWithOneHooker].ped.Task.ChatTo(neutralPeds[(int)Neutrals.HookerLookingAtPhone].ped);
        neutralPeds[(int)Neutrals.HookerLookingAtPhone].ped.Task.UseMobilePhone();

        enemies[(int)Enemies.TvRoomGuard01].ped.Task.StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.TvRoomGuard02].ped.Task.StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.TvRoomGuard03].ped.Task.StartScenario("WORLD_HUMAN_CHEERING", 0);
        enemies[(int)Enemies.TvRoomGuard04].ped.Task.StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);
        neutralPeds[(int)Neutrals.StripperTvRoom].ped.Task.StartScenario("WORLD_HUMAN_YOGA", 0);

        enemies[(int)Enemies.ShipCaptain].ped.Task.StartScenario("WOLRD_HUMAN_BINOCULARS", 0);
        enemies[(int)Enemies.CaptainGuard].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);

        enemies[(int)Enemies.StairsGuard].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);

        enemies[(int)Enemies.BossRecordingStripper].ped.Task.StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);
        neutralPeds[(int)Neutrals.StripperBeingRecorded].ped.Task.StartScenario("WORLD_HUMAN_YOGA", 0);

        neutralPeds[(int)Neutrals.StripperDancing01].ped.Task.StartScenario("WORLD_HUMAN_YOGA", 0);
        neutralPeds[(int)Neutrals.StripperDancing02].ped.Task.StartScenario("WORLD_HUMAN_YOGA", 0);
        enemies[(int)Enemies.GuardRecordingStrippers].ped.Task.StartScenario("WORLD_HUMAN_MOBILE_FILM_SHOCKING", 0);

        enemies[(int)Enemies.GuardDrinking].ped.Task.StartScenario("WORLD_HUMAN_DRINKING", 0);
        neutralPeds[(int)Neutrals.HookerWithGuard01].ped.Task.StartScenario("WOLRD_HUMAN_PARTYING", 0);
        neutralPeds[(int)Neutrals.HookerWithGuard02].ped.Task.ChatTo(enemies[(int)Enemies.GuardDrinking].ped);

        enemies[(int)Enemies.BossDrinkingBossRoom].ped.Task.StartScenario("WOLRD_HUMAN_DRINKING", 0);
        enemies[(int)Enemies.BossTalking01].ped.Task.ChatTo(enemies[(int)Enemies.BossTalking02].ped);
        enemies[(int)Enemies.BossTalking02].ped.Task.ChatTo(enemies[(int)Enemies.BossTalking01].ped);

        enemies[(int)Enemies.LedgeGuard].ped.Task.StartScenario("WORLD_HUMAN_BINOCULARS", 0);

        enemies[(int)Enemies.LowerLedgeGuard01].ped.Task.StartScenario("WOLRD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.LowerLedgeGuard02].ped.Task.ChatTo(enemies[(int)Enemies.LowerLedgeGuard01].ped);

        enemies[(int)Enemies.BayGuard].ped.Task.StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
    }

    void CheckHelicopter(Object o, EventArgs e)
    {
        if (!missionWorld.isMissionActive || enemies.Count == 0)
        {
            script.Tick -= CheckHelicopter;
            return;
        }
        if (vehicles[(int)Vehicles.Helicopter].IsDead)
        {
            if (vehicles[(int)Vehicles.Helicopter].AttachedBlip != null)
            {
                vehicles[(int)Vehicles.Helicopter].AttachedBlip.Delete();
            }
            script.Tick -= CheckHelicopter;
            return;
        }
        if (messageShown)
        {
            if (vehicles[(int)Vehicles.Helicopter].IsInRange(helicopterDestination, 100))
            {
                missionWorld.QuitMission();
                GTA.UI.Screen.ShowSubtitle("~r~Mission failed, a target escaped!");
                script.Tick -= CheckHelicopter;
            }
        }
        else if (!vehicles[(int)Vehicles.Helicopter].IsStopped && vehicles[(int)Vehicles.Helicopter].Driver != null)
        {
            vehicles[(int)Vehicles.Helicopter].AddBlip();
            vehicles[(int)Vehicles.Helicopter].AttachedBlip.Sprite = BlipSprite.HelicopterAnimated;
            vehicles[(int)Vehicles.Helicopter].AttachedBlip.Color = BlipColor.Red;
            vehicles[(int)Vehicles.Helicopter].AttachedBlip.Name = "Wanted criminal";

            GTA.UI.Screen.ShowSubtitle("A ~r~target ~w~is trying to escape in a ~r~helicopter~w~!");

            messageShown = true;
        }
    }
}
