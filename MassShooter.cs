using GTA;
using GTA.Native;
using GTA.Math;
using System;

class MassShooter : Mission
{
    enum Objectives
    {
        GoToLocation,
        KillEnemy,
        None
    }

    MissionPed enemy;
    Vector3 location;
    Objectives currentObjective;
    Script script;
    MissionWorld missionWorld;
    RandomMissions randomMissions;
    Blip locationBlip;
    RelationshipGroup enemyRelGroup;

    public MassShooter(Script script, MissionWorld missionWorld, RelationshipGroup enemyRelGroup)
    {
        this.script = script;
        this.missionWorld = missionWorld;
        this.enemyRelGroup = enemyRelGroup;

        randomMissions = new RandomMissions();
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
                    var ped = randomMissions.CreateCriminal(location);
                    Script.Wait(500);
                    enemy = new MissionPed(ped, enemyRelGroup, location, script);
                    enemy.ped.FiringPattern = FiringPattern.FullAuto;
                    enemy.ped.Accuracy = 80;
                    enemy.ped.GiveHelmet(false, Helmet.RegularMotorcycleHelmet, 1);
                    enemy.ped.CanSufferCriticalHits = false;
                    enemy.ped.CanWrithe = false;
                    enemy.ped.MaxHealth = 450;
                    enemy.ped.Health = 450;
                    enemy.ped.Armor = 300;
                    Function.Call(Hash.SET_PED_COMBAT_MOVEMENT, enemy.ped, 3);
                    enemy.ShowBlip();
                    enemy.ped.Task.FightAgainstHatedTargets(190);

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
                        missionWorld.CompleteMission();
                        currentObjective = Objectives.None;
                        script.Tick -= MissionTick;
                    }
                    break;
                }
        }
    }

    public override void QuitMission()
    {
        currentObjective = Objectives.None;
        script.Tick -= MissionTick;
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
            location = randomMissions.GetRandomLocation(RandomMissions.LocationType.Foot);
        } while (Game.Player.Character.IsInRange(location, 200));

        locationBlip = World.CreateBlip(location);
        locationBlip.Color = BlipColor.Yellow;
        locationBlip.Name = "Mass shooter location";
        locationBlip.ShowRoute = true;

        GTA.UI.Screen.ShowSubtitle("Go to the ~y~crime scene~w~.", 8000);

        currentObjective = Objectives.GoToLocation;
        script.Tick += MissionTick;

        return true;
    }
}
