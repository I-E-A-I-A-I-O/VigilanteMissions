using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;


class MissionOne : Mission
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
        Target,
        ElectricBoxGuard01,
        ElectricBoxGuard02,
        WallGuard,
        ScoutGuard,
        GuardTrash01,
        GuardTrash02,
        PedGroup01,
        PedGroup02,
        PedGroup03,
        HallwayGuard,
        EntranceGuard01,
        EntranceGuard02,
    }

    Objectives currentObjective;
    List<MissionPed> enemies = new List<MissionPed>();
    List<Vehicle> vehicles = new List<Vehicle>();
    Vector3 targetLocation;
    Blip objectiveLocationBlip;
    Music music;
    MostWantedMissions mostWantedMissions;
    Script script;
    RelationshipGroup enemiesRelGroup;
    MissionWorld missionWorld;

    public MissionOne(Script script, RelationshipGroup relationshipGroup, MissionWorld missionWorld)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        enemiesRelGroup = relationshipGroup;

        music = new Music();
        mostWantedMissions = new MostWantedMissions();
        targetLocation = mostWantedMissions.MISSION_ONE_LOCATION;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        if (Game.Player.WantedLevel >= 2)
        {
            Game.Player.WantedLevel = 1;
        }
        switch(currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(targetLocation, 200f))
                    {
                        return;
                    }
                    music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    vehicles = mostWantedMissions.InitializeMissionOneVehicles();
                    var peds = mostWantedMissions.IntializeMissionOnePeds();
                    Script.Wait(1000);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup, targetLocation, script));
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

    void StartScenarios()
    {
        enemies[(int)Enemies.Target].ped.Task.UseMobilePhone();
        enemies[(int)Enemies.ElectricBoxGuard01].ped.Task.StartScenario("WORLD_HUMAN_DRUG_DEALER", 0);
        enemies[(int)Enemies.ElectricBoxGuard02].ped.Task.StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.WallGuard].ped.Task.StartScenario("WORLD_HUMAN_LEANING", 0);
        enemies[(int)Enemies.ScoutGuard].ped.Task.StartScenario("WORLD_HUMAN_BINOCULARS", 0);
        enemies[(int)Enemies.GuardTrash01].ped.Task.UseMobilePhone();
        enemies[(int)Enemies.GuardTrash02].ped.Task.ChatTo(enemies[(int)Enemies.GuardTrash01].ped);
        enemies[(int)Enemies.HallwayGuard].ped.Task.GuardCurrentPosition();
        enemies[(int)Enemies.PedGroup01].ped.Task.ChatTo(enemies[(int)Enemies.PedGroup02].ped);
        enemies[(int)Enemies.PedGroup02].ped.Task.ChatTo(enemies[(int)Enemies.PedGroup01].ped);
        enemies[(int)Enemies.PedGroup03].ped.Task.StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.EntranceGuard01].ped.Task.StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.EntranceGuard02].ped.Task.StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
    }
}
