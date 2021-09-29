using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionElevenPartOne : Mission
{
    enum Objectives
    {
        GoToMeeting,
        KillDirtyCia,
        GrabKeycard,
        GoToServerFarm,
        EnterServer,
        PlaceBackDoor,
        LeaveServerFarm,
        LoseCops,
        GoToChase,
        Complete,
        None
    }

    Objectives currentObjective;
    Vector3 objectiveLocation;
    Blip objectiveLocationBlip;
    List<MissionPed> enemies = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    Prop prop;
    int loadingStartTime;
    int loadingCurrentTime;
    bool loadingTimerStarted;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralGroup;

    public MissionElevenPartOne()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_AGGRESSIVE;
        neutralGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_SERVER_LOCATION;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToMeeting:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    objectiveLocationBlip.Delete();
                    var ped = MostWantedMissions.InitializeMissionElevenMeetingPed();
                    while (!MissionWorld.IsEntityLoaded(ped))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingStartTime = Game.GameTime;
                            loadingTimerStarted = true;
                        } else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                ped = MostWantedMissions.InitializeMissionElevenMeetingPed();
                                loadingTimerStarted = false;
                            }
                        }
                    }
                    enemies.Add(new MissionPed(ped, neutralGroup));
                    enemies[0].GetTask().StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
                    enemies[0].ShowBlip();
                    enemies[0].GetBlip().Name = "Corrupt IAA agent";
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~corrupt agent~w~.", 8000);
                    currentObjective = Objectives.KillDirtyCia;
                    break;
                }
            case Objectives.KillDirtyCia:
                {
                    if (!enemies[0].IsDead())
                    {
                        return;
                    }
                    prop = World.CreateProp(new Model(), enemies[0].GetPosition(), true, true);
                    prop.AddBlip();
                    prop.AttachedBlip.Scale = 0.7f;
                    prop.AttachedBlip.Color = BlipColor.Green;
                    prop.AttachedBlip.Name = "Keycard";
                    RemoveDeadEnemies();
                    currentObjective = Objectives.GrabKeycard;
                    break;
                }
            case Objectives.GrabKeycard:
                {
                    if (!Game.Player.Character.IsInRange(prop.Position, 1.5f))
                    {
                        return;
                    }
                    if (Game.LastInputMethod == InputMethod.GamePad)
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to grab the ~g~keycard");
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to grab the ~g~keycard");
                    }
                    if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                    {
                        prop.Delete();
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FACILITY_LOCATION;
                        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
                        objectiveLocationBlip.Color = BlipColor.Yellow;
                        objectiveLocationBlip.Name = "IAA Server farm";
                        GTA.UI.Screen.ShowSubtitle("Go to the ~y~Server farm~w~.", 8000);
                        currentObjective = Objectives.GoToServerFarm;
                    }
                    break;
                }
            case Objectives.GoToServerFarm:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200))
                    {
                        return;
                    }
                    var peds = MostWantedMissions.InitializeMissionElevenServerGuards();
                    while(!MissionWorld.IsPedListLoaded(peds))
                    {
                        Script.Wait(1);
                        if (!loadingTimerStarted)
                        {
                            loadingStartTime = Game.GameTime;
                            loadingTimerStarted = true;
                        } else
                        {
                            loadingCurrentTime = Game.GameTime;
                            if (loadingCurrentTime - loadingStartTime >= 3000)
                            {
                                foreach (Ped ped in peds)
                                {
                                    if (ped != null)
                                    {
                                        ped.Delete();
                                    }
                                    peds = MostWantedMissions.InitializeMissionElevenServerGuards();
                                }
                            }
                        }
                    }
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].GetPed().Armor = 100;
                    }
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FACILITY_INSIDE_LOCATION;
                    currentObjective = Objectives.EnterServer;
                    break;
                }
            case Objectives.EnterServer:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        return;
                    }
                    Music.IncreaseIntensity();
                    Game.Player.WantedLevel = 4;
                    objectiveLocation = MostWantedMissions.MISSION_ELEVEN_SERVER_LOCATION;
                    objectiveLocationBlip.Delete();
                    foreach (MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                        enemy.GetBlip().Name = "IAA agent";
                        enemy.GetTask().GuardCurrentPosition();
                    }
                    objectiveLocationBlip = World.CreateBlip(objectiveLocation);
                    objectiveLocationBlip.Color = BlipColor.Yellow;
                    objectiveLocationBlip.Name = "Target server";
                    GTA.UI.Screen.ShowSubtitle("Insert the device in the ~y~server~w~.", 8000);
                    currentObjective = Objectives.PlaceBackDoor;
                    break;
                }
            case Objectives.PlaceBackDoor:
                {
                    Game.Player.WantedLevel = 4;
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 1))
                    {
                        return;
                    }
                    if (Game.LastInputMethod == InputMethod.GamePad)
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame("Press DPad Right to insert the device");
                    }
                    else
                    {
                        GTA.UI.Screen.ShowHelpTextThisFrame($"Press {VigilanteMissions.interactKey} to insert the device");
                    }
                    if ((Game.LastInputMethod == InputMethod.MouseAndKeyboard && Game.IsKeyPressed(VigilanteMissions.interactMissionKey)) || (Game.LastInputMethod == InputMethod.GamePad && Game.IsControlJustReleased(GTA.Control.ScriptPadRight)))
                    {
                        var sequence = new TaskSequence();
                        sequence.AddTask.AchieveHeading(Vector3.RelativeBack.ToHeading());
                        sequence.AddTask.PlayAnimation("weapons@first_person@aim_rng@generic@projectile@thermal_charge@", "plant_vertical", 8, -1, AnimationFlags.UpperBodyOnly);
                        Game.Player.Character.Task.PerformSequence(sequence);
                        Script.Wait(2000);
                        objectiveLocationBlip.Delete();
                        GTA.UI.Screen.ShowSubtitle("Leave the server farm.", 8000);
                        objectiveLocation = MostWantedMissions.MISSION_ELEVEN_FACILITY_LOCATION;
                        currentObjective = Objectives.LeaveServerFarm;
                    }
                    break;
                }
            case Objectives.LeaveServerFarm:
                {
                    Game.Player.WantedLevel = 4;
                    if (enemies.Count > 0)
                    {
                        RemoveDeadEnemies();
                    }
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 20))
                    {
                        return;
                    }
                    foreach (MissionPed ped in enemies)
                    {
                        ped.Delete();
                    }
                    GTA.UI.Screen.ShowSubtitle("Lose the cops.", 8000);
                    Music.LowerIntensinty();
                    currentObjective = Objectives.LoseCops;
                    break;
                }
            case Objectives.LoseCops:
                {
                    if (Game.Player.WantedLevel > 0)
                    {
                        return;
                    }
                    currentObjective = Objectives.Complete;
                    break;
                }
            case Objectives.Complete:
                {
                    RemoveVehiclesAndNeutrals();
                    GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted Suspect", "Good job, your cut of the reward is already in your account.");
                    currentObjective = Objectives.None;
                    //Game.Player.Money += 15000;
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
        if (objectiveLocationBlip.Exists())
        {
            objectiveLocationBlip.Delete();
        }
        if (prop.Exists())
        {
            if (prop.AttachedBlip.Exists())
            {
                prop.AttachedBlip.Delete();
            }
            prop.MarkAsNoLongerNeeded();
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
    }

    public override bool StartMission()
    {
        if (Game.Player.Character.IsInRange(objectiveLocation, 200))
        {
            GTA.UI.Notification.Show("Mission not available.");
            return false;
        }
        Music.StartFunky();
        GTA.UI.Notification.Show(GTA.UI.NotificationIcon.Lester, "Lester", "Wanted suspect", "We are going to need access to the IAA servers if we want to find this fucker. I set up a fake ~y~meeting~w~ with a corrupt agent, go get his ~g~keycard~w~.");
        objectiveLocationBlip = World.CreateBlip(objectiveLocation);
        objectiveLocationBlip.Color = BlipColor.Yellow;
        objectiveLocationBlip.Name = "Meeting with IAA agent";
        objectiveLocationBlip.ShowRoute = true;
        GTA.UI.Screen.ShowSubtitle("Go to the ~y~meeting~w~.", 8000);
        currentObjective = Objectives.GoToMeeting;
        MissionWorld.script.Tick += MissionTick;
        return true;
    }
}
