using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;


class MissionOne : Mission
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
    public override Blip ObjectiveLocationBlip { get; set; }
    RelationshipGroup enemiesRelGroup;

    public MissionOne()
    {
        enemiesRelGroup = MissionWorld.RELATIONSHIP_MISSION_NEUTRAL;
        targetLocation = MostWantedMissions.MISSION_ONE_LOCATION;
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
                    Music.IncreaseIntensity();
                    ObjectiveLocationBlip.Delete();
                    vehicles = MostWantedMissions.InitializeMissionOneVehicles();
                    var peds = MostWantedMissions.IntializeMissionOnePeds();
                    vehicles = MissionWorld.VehicleListLoadLoop(vehicles, MostWantedMissions.InitializeMissionOneVehicles);
                    peds = MissionWorld.PedListLoadLoop(peds, MostWantedMissions.IntializeMissionOnePeds);
                    for (var i = 0; i < peds.Count; i++)
                    {

                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup));
                        enemies[i].ShowBlip();
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
        enemies[(int)Enemies.Target].GetTask().UseMobilePhone();
        enemies[(int)Enemies.ElectricBoxGuard01].GetTask().StartScenario("WORLD_HUMAN_DRUG_DEALER", 0);
        enemies[(int)Enemies.ElectricBoxGuard02].GetTask().StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.WallGuard].GetTask().StartScenario("WORLD_HUMAN_LEANING", 0);
        enemies[(int)Enemies.ScoutGuard].GetTask().StartScenario("WORLD_HUMAN_BINOCULARS", 0);
        enemies[(int)Enemies.GuardTrash01].GetTask().UseMobilePhone();
        enemies[(int)Enemies.GuardTrash02].GetTask().ChatTo(enemies[(int)Enemies.GuardTrash01].GetPed());
        enemies[(int)Enemies.HallwayGuard].GetTask().GuardCurrentPosition();
        enemies[(int)Enemies.PedGroup01].GetTask().ChatTo(enemies[(int)Enemies.PedGroup02].GetPed());
        enemies[(int)Enemies.PedGroup02].GetTask().ChatTo(enemies[(int)Enemies.PedGroup01].GetPed());
        enemies[(int)Enemies.PedGroup03].GetTask().StartScenario("WORLD_HUMAN_SMOKING", 0);
        enemies[(int)Enemies.EntranceGuard01].GetTask().StartScenario("WORLD_HUMAN_GUARD_STAND", 0);
        enemies[(int)Enemies.EntranceGuard02].GetTask().StartScenario("WORLD_HUMAN_AA_SMOKE", 0);
    }
}
