﻿using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class MassShooter : Mission
{
    public override bool IsMostWanted => false;

    enum Objectives
    {
        GoToLocation,
        KillEnemy,
        None
    }

    MissionPed enemy;
    Vector3 location;
    Objectives currentObjective;
    Blip locationBlip;
    RelationshipGroup enemyRelGroup;

    public MassShooter()
    {
        enemyRelGroup = MissionWorld.RELATIONSHIP_MISSION_MASS_SHOOTER;
    }

    public override void MissionTick(object o, EventArgs e)
    {
        switch(currentObjective)
        {
            case Objectives.GoToLocation:
                {
                    if (!Game.Player.Character.IsInRange(location, 200))
                    {
                        return;
                    }
                    locationBlip.Delete();
                    var ped = RandomMissions.CreateCriminal(location);
                    ped = (Ped)MissionWorld.EntityLoadLoop(ped, RandomMissions.CreateCriminal, location);
                    enemy = new MissionPed(ped, enemyRelGroup);
                    enemy.GetPed().FiringPattern = FiringPattern.FullAuto;
                    enemy.GetPed().Accuracy = 80;
                    enemy.GetPed().GiveHelmet(false, Helmet.RegularMotorcycleHelmet, 1);
                    enemy.GetPed().CanSufferCriticalHits = false;
                    enemy.GetPed().CanWrithe = false;
                    enemy.GetPed().MaxHealth = 450;
                    enemy.GetPed().Health = 450;
                    enemy.GetPed().Armor = 300;
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy.GetPed(), 3);
                    enemy.ShowBlip();
                    enemy.GetTask().FightAgainstHatedTargets(190);

                    GTA.UI.Screen.ShowSubtitle("Kill the ~r~shooter~w~.", 8000);
                    currentObjective = Objectives.KillEnemy;
                    break;
                }
            case Objectives.KillEnemy:
                {
                    if (enemy.IsDead())
                    {
                        RemoveDeadEnemies();
                        GTA.UI.Screen.ShowSubtitle("Crime scene cleared.", 8000);
                        Game.Player.Money += 1200;
                        MissionWorld.CompleteMission();
                        currentObjective = Objectives.None;
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
        if (locationBlip != null && locationBlip.Exists())
        {
            locationBlip.Delete();
        }
        if (enemy != null)
        {
            enemy.Delete();
        }
    }

    public override void RemoveDeadEnemies()
    {
        enemy.Delete();
    }

    public override void RemoveVehiclesAndNeutrals()
    {
        //Not needed
    }

    public override bool StartMission()
    {
        do
        {
            location = RandomMissions.GetRandomLocation(RandomMissions.LocationType.Foot);
        } while (Game.Player.Character.IsInRange(location, 200));

        locationBlip = World.CreateBlip(location);
        locationBlip.Color = BlipColor.Yellow;
        locationBlip.Name = "Mass shooter location";
        locationBlip.ShowRoute = true;

        GTA.UI.Screen.ShowSubtitle("Go to the ~y~crime scene~w~.", 8000);

        currentObjective = Objectives.GoToLocation;
        MissionWorld.script.Tick += MissionTick;

        return true;
    }
}
