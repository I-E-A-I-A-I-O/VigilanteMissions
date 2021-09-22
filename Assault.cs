using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

class Assault : Mission
{
    enum Objectives
    {
        GoToLocation,
        KillTargets,
        Completed,
        None
    }

    MissionWorld missionWorld;
    Vector3 objectiveLocation;
    RelationshipGroup enemiesRelGroup;
    RelationshipGroup neutralsRelGroup;
    List<MissionPed> enemies = new List<MissionPed>();
    List<MissionPed> neutralPeds = new List<MissionPed>();
    Objectives currentObjective;
    Script script;
    RandomMissions randomMissions;
    Blip objectiveLocationBlip;
    int actionToTake;
    int shootRange;
    bool actionTaken = false;
    int startTime;
    int currentTime;
    bool timerStarted = false;
    bool actionStarted = false;

    public Assault(Script script, MissionWorld missionWorld, RelationshipGroup enemiesRelGroup, RelationshipGroup neutralsRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemiesRelGroup = enemiesRelGroup;
        this.neutralsRelGroup = neutralsRelGroup;

        randomMissions = new RandomMissions();
    }

    public override void MissionTick(object o, EventArgs e)
    {
        switch (currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(objectiveLocation, 200f))
                    {
                        return;
                    }
                    objectiveLocationBlip.Delete();
                    var enemy = randomMissions.CreateCriminal(objectiveLocation);
                    var neutral = randomMissions.CreateVictim(objectiveLocation);
                    Script.Wait(1000);
                    enemies.Add(new MissionPed(enemy, enemiesRelGroup, objectiveLocation, script));
                    neutralPeds.Add(new MissionPed(neutral, neutralsRelGroup, objectiveLocation, script, true));
                    enemies[0].ShowBlip();
                    neutralPeds[0].ped.AddBlip();
                    neutralPeds[0].ped.AttachedBlip.Scale = 0.8f;
                    neutralPeds[0].ped.AttachedBlip.Color = BlipColor.Green;
                    neutralPeds[0].ped.AttachedBlip.IsFlashing = true;
                    neutralPeds[0].ped.AttachedBlip.Name = "Victim";
                    enemies[0].ped.Task.AimAt(neutralPeds[0].ped, 1800000);
                    neutralPeds[0].ped.Task.HandsUp(1800000);
                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~target~w~, don't let the ~g~victim~w~ get killed!", 8000);
                    currentObjective = Objectives.KillTargets;
                    break;
                }
            case Objectives.KillTargets:
                {
                    if (neutralPeds[0].ped.IsDead)
                    {
                        neutralPeds[0].ped.AttachedBlip.Delete();
                        GTA.UI.Screen.ShowSubtitle("~r~Mission failed, the victim died.", 8000);
                        missionWorld.QuitMission();
                        return;
                    }
                    if (enemies[0].ped.IsDead)
                    {
                        RemoveDeadEnemies();
                        currentObjective = Objectives.Completed;
                    }
                    else
                    {
                        if (actionStarted)
                        {
                            return;
                        }
                        if (enemies[0].ped.IsInCombatAgainst(Game.Player.Character))
                        {
                            actionStarted = true;
                            return;
                        }
                        if (Game.Player.Character.IsInRange(enemies[0].ped.Position, 55))
                        {
                            if (!actionTaken)
                            {
                                Random ran = new Random();
                                actionToTake = ran.Next(1, 11);
                                shootRange = ran.Next(15, 41);
                                actionTaken = true;
                            }
                            bool isPlayerInRange = Game.Player.Character.IsInRange(enemies[0].ped.Position, shootRange);
                            if (isPlayerInRange && !timerStarted)
                            {
                                startTime = Game.GameTime;
                                timerStarted = true;
                                enemies[0].ped.PlayAmbientSpeech("GENERIC_INSULT_HIGH", SpeechModifier.ShoutedClear);
                            } else if (!isPlayerInRange && timerStarted)
                            {
                                timerStarted = false;
                            } else if (isPlayerInRange && timerStarted) {
                                currentTime = Game.GameTime;
                                if (currentTime - startTime >= 1500)
                                {
                                    enemies[0].ped.Task.ClearAllImmediately();
                                    if (actionToTake <= 3)
                                    {
                                        enemies[0].ped.Task.ShootAt(neutralPeds[0].ped, -1, FiringPattern.FullAuto);
                                    } else
                                    {
                                        enemies[0].ped.Task.FightAgainst(Game.Player.Character);
                                    }
                                    actionStarted = true;
                                }
                            }
                        }
                    }
                    break;
                }
            case Objectives.Completed:
                {
                    GTA.UI.Screen.ShowSubtitle("Crime scene cleared.", 8000);
                    Game.Player.Money += 1000;
                    currentObjective = Objectives.None;
                    RemoveVehiclesAndNeutrals();
                    missionWorld.CompleteMission();
                    script.Tick -= MissionTick;
                    break;
                }
        }
    }

    public override void QuitMission()
    {
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
        foreach(MissionPed neutral in neutralPeds)
        {
            neutral.Delete();
        }
    }

    public override bool StartMission()
    {
        try
        {
            do
            {
                objectiveLocation = randomMissions.GetRandomLocation(RandomMissions.LocationType.Foot);
            } while (Game.Player.Character.IsInRange(objectiveLocation, 200f));

            currentObjective = Objectives.GoToLocation;
            objectiveLocationBlip = World.CreateBlip(objectiveLocation, 150f);
            objectiveLocationBlip.Color = BlipColor.Yellow;
            objectiveLocationBlip.ShowRoute = true;
            objectiveLocationBlip.Name = "Crime scene";
            GTA.UI.Screen.ShowSubtitle("Go to the ~y~crime scene~w~.", 8000);

            script.Tick += MissionTick;
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
