using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

class MissionFive : Mission
{
    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    Vector3 objectiveLocation;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    MissionWorld missionWorld;
    Script script;
    Music music;
    Objectives currentObjective;
    Blip objectiveLocationBlip;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    MostWantedMissions mostWantedMissions;

    public MissionFive(Script script, MissionWorld missionWorld, RelationshipGroup enemiesRelGroup, RelationshipGroup neutralsRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemiesRelGroup = enemiesRelGroup;
        this.neutralsRelGroup = neutralsRelGroup;

        music = new Music();
        mostWantedMissions = new MostWantedMissions();
        objectiveLocation = mostWantedMissions.MISSION_FIVE_LOCATION;
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
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200f))
                    {
                        return;
                    }
                    music.IncreaseIntensity();
                    objectiveLocationBlip.Delete();
                    var peds = mostWantedMissions.InitializeMissionFivePeds();
                    var neutrals = mostWantedMissions.InitializeMissionFiveCivilianPeds();
                    Script.Wait(1000);
                    for (var i = 0; i < peds.Count; i++)
                    {
                        enemies.Add(new MissionPed(peds[i], enemiesRelGroup, objectiveLocation, script));
                    }
                    for (var i = 0; i < neutrals.Count; i++)
                    {
                        neutralPeds.Add(new MissionPed(neutrals[i], neutralsRelGroup, objectiveLocation, script, true));
                    }
                    foreach (MissionPed enemy in enemies)
                    {
                        enemy.ShowBlip();
                    }
                    GTA.UI.Screen.ShowSubtitle("Save the ~g~woman~w~, kill the ~r~target~w~ before it's too late!", 8000);
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
        music.StartTedBundyMusic();
        currentObjective = Objectives.GoToLocation;
        objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
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
        enemies[0].ped.CanSufferCriticalHits = false;
        enemies[0].ped.Armor = 600;
        enemies[0].ped.MaxHealth = 600;
        enemies[0].ped.Health = 600;
        enemies[0].ped.CanRagdoll = true;
        enemies[0].ped.CanWrithe = false;

        var bundySequence = new TaskSequence();
        bundySequence.AddTask.FollowToOffsetFromEntity(neutralPeds[0].ped, new Vector3(), 25, 15000);
        bundySequence.AddTask.ShootAt(neutralPeds[0].ped, -1, FiringPattern.FullAuto);
        neutralPeds[0].ped.Task.FleeFrom(enemies[0].ped);
        enemies[0].ped.Task.PerformSequence(bundySequence);
        neutralPeds[0].ped.AddBlip();
        neutralPeds[0].ped.AttachedBlip.Scale = 0.8f;
        neutralPeds[0].ped.AttachedBlip.Color = BlipColor.Green;
        neutralPeds[0].ped.AttachedBlip.Name = "Victim";
        neutralPeds[0].ped.AttachedBlip.IsFlashing = true;

        script.Tick += CheckWomanStatus;
    }

    private void CheckWomanStatus(object sender, EventArgs e)
    {
        if (!missionWorld.isMissionActive || enemies.Count == 0)
        {
            script.Tick -= CheckWomanStatus;
            return;
        }
        if (neutralPeds[0].ped.IsDead)
        {
            neutralPeds[0].ped.AttachedBlip.Delete();
            missionWorld.QuitMission();
            GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the woman was killed!", 8000);
            script.Tick -= CheckWomanStatus;
            return;
        }
    }
}
