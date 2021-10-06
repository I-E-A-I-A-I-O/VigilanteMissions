using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

public class MissionPed
{
    Ped ped;
    Vector3 originalPosition;
    float originalHeading;
    bool isFollowing = false;
    bool isInScenario = true;
    bool drawedWeapon = false;
    bool wasWeaponDrawed = false;
    bool timerStarted = false;
    bool stolenVehicleMission = false;
    float timeBeforeAttack = 5000f;
    int startTime;
    int currentTime;

    readonly List<string> scenarios = new List<string>()
    {
        "WOLRD_HUMAN_SMOKING",
        "WORLD_HUMAN_GUARD_STAND",
        "WORLD_HUMAN_DRINKING",
        "WORLD_HUMAN_AA_SMOKE",
        "WORLD_HUMAN_STAND_IMPATIENT",
        "WORLD_HUMAN_DRUG_DEALER",
        "WORLD_HUMAN_STAND_MOBILE",
        "WORLD_HUMAN_STAND_MOBILE_UPRIGHT",
        "PATROL"
    };

    public MissionPed(Ped ped, RelationshipGroup relationshipGroup, bool civilian = false, bool stolenVehicleMission = false)
    {
        this.ped = ped;
        this.ped.RelationshipGroup = relationshipGroup;
        originalPosition = ped.Position;
        originalHeading = ped.Heading;
        if (!civilian)
        {
            this.ped.Weapons.Give(WeaponHash.MicroSMG, 500, false, true);
            this.ped.Weapons.Give(RandomWeapon(), 500, true, true);
            if (stolenVehicleMission)
            {
                return;
            } else if (ped.GetRelationshipWithPed(Game.Player.Character) == Relationship.Neutral)
            {
               MissionWorld.script.Tick += PedTick;
            }
        }
    }

    public MissionPed(Ped ped, RelationshipGroup relationshipGroup, string RelGroupName)
    {
        this.ped = ped;
        Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, this.ped.Handle, Game.GenerateHash(RelGroupName));
        this.ped.Weapons.Give(WeaponHash.PumpShotgun, 500, false, true);
        this.ped.Weapons.Give(WeaponHash.Pistol, 500, true, true);
    }

    void PedTick(object o, EventArgs args)
    {
        if (!ped.IsAlive || ped.IsInCombatAgainst(Game.Player.Character))
        {
            MissionWorld.script.Tick -= PedTick;
            return;
        }
        if (Game.Player.Character.Weapons.Current.Hash == WeaponHash.Unarmed)
        {
            timeBeforeAttack = 6500f;
            drawedWeapon = false;
        } else
        {
            timeBeforeAttack = 3000f;
            drawedWeapon = true;
        }
        if (Game.Player.Character.IsInRange(ped.Position, 8.5f) && !isFollowing)
        {
            ped.PlayAmbientSpeech("GENERIC_INSULT_MED", SpeechModifier.Standard);
            ped.Task.ClearAllImmediately();
            ped.Task.FollowToOffsetFromEntity(Game.Player.Character, Vector3.Zero, 0.65f, -1, 5);
            isFollowing = true;
            isInScenario = false;
            timerStarted = true;
            wasWeaponDrawed = drawedWeapon;
            startTime = Game.GameTime;
        } else if (!Game.Player.Character.IsInRange(ped.Position, 12) && isFollowing)
        {
            ped.Task.ClearAllImmediately();
            ped.Task.GoStraightTo(originalPosition);
            isFollowing = false;
            timerStarted = false;
        } 
        if (isFollowing && timerStarted)
        {
            currentTime = Game.GameTime;
            if ((!wasWeaponDrawed && drawedWeapon) || (currentTime - startTime > timeBeforeAttack))
            {
                ped.Task.FightAgainst(Game.Player.Character);
                MissionWorld.script.Tick -= PedTick;
                return;
            }
        }
        if (!isFollowing && !isInScenario && ped.IsInRange(originalPosition, 1))
        {
            GiveRandomScenario();
            isInScenario = true;
        }
    }

    public void GiveRandomScenario()
    {
        var ran = new Random();
        var scenario = scenarios[ran.Next(0, scenarios.Count)];
        if (scenario == "PATROL")
        {
            ped.Task.GuardCurrentPosition();
        }
        else
        {
            ped.Task.StartScenario(scenario, originalHeading);
        }
    }

    WeaponHash RandomWeapon()
    {
        var random = new Random();
        switch(random.Next(1, 11))
        {
            case 1:
                return WeaponHash.APPistol;
            case 2:
                return WeaponHash.SpecialCarbine;
            case 3:
                return WeaponHash.MicroSMG;
            case 5:
                return WeaponHash.AssaultRifle;
            case 6:
                return WeaponHash.PumpShotgun;
            case 7:
                return WeaponHash.MG;
            case 8:
                return WeaponHash.MiniSMG;
            case 9:
                return WeaponHash.SawnOffShotgun;
            case 10:
                return WeaponHash.CompactRifle;
            default:
                return WeaponHash.Pistol;
        }
    }

    public bool IsDead()
    {
        return ped.IsDead;
    }

    public void Delete(bool force = false)
    {
        if (!force)
        {
            if (ped.AttachedBlip != null)
                RemoveBlip();
            ped.MarkAsNoLongerNeeded();
        } else
        {
            ped.Delete();
        }
    }

    public void RemoveBlip()
    {
        ped.AttachedBlip.Delete();
    }
    public void ShowBlip()
    {
        ped.AddBlip();
        ped.AttachedBlip.Scale = 0.8f;
        ped.AttachedBlip.Color = BlipColor.Red;
        ped.AttachedBlip.Name = "Wanted Criminal";
    }
    public TaskInvoker GetTask()
    {
        return ped.Task;
    }
    public Ped GetPed()
    {
        return ped;
    }
    public Vector3 GetPosition()
    {
        return ped.Position;
    }
    public RelationshipGroup GetRelGroup()
    {
        return ped.RelationshipGroup;
    }
    public Blip GetBlip()
    {
        return ped.AttachedBlip;
    }
}

