using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class PoliceBackup
{
    readonly List<VehicleHash> vehicleHashList = new List<VehicleHash>()
    {
        VehicleHash.Police,
        VehicleHash.Police2,
        VehicleHash.Police3,
    };
    Vehicle vehicle;
    List<Ped> police = new List<Ped>();
    bool dialogDone = false;
    bool gettingInAgain = false;
    bool leftVehicle = false;

    public PoliceBackup()
    {
        var copRelGroupHash = Function.Call<int>(Hash.GET_HASH_KEY, "COP");
        var copRelGroup = new RelationshipGroup(copRelGroupHash);

        var ran = new Random();
        var backupSize = ran.Next(1, 3);

        var outPos = new OutputArgument();
        var posAroundPlayer = Game.Player.Character.Position.Around(100);
        var result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE, posAroundPlayer.X, posAroundPlayer.Y, posAroundPlayer.Z, outPos, 0, 3, 0);
        var vehiclePos = outPos.GetResult<Vector3>();

        if (backupSize == 1)
        {
            vehicle = World.CreateVehicle(new Model(VehicleHash.Policeb), vehiclePos);
            while(!MissionWorld.IsEntityLoaded(vehicle))
            {
                Script.Wait(1);
            }
            var ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.Hwaycop01SMY));
            while(!MissionWorld.IsEntityLoaded(ped))
            {
                Script.Wait(1);
            }
            ped.RelationshipGroup = copRelGroup;
            ped.Weapons.Give(WeaponHash.Pistol, 100, true, true);
            police.Add(ped);
        } else
        {
            vehicle = World.CreateVehicle(new Model(vehicleHashList[ran.Next(0, vehicleHashList.Count)]), vehiclePos);
            while(!MissionWorld.IsEntityLoaded(vehicle))
            {
                Script.Wait(1);
            }
            var pedDriver = vehicle.CreatePedOnSeat(VehicleSeat.Driver, new Model(PedHash.Cop01SMY));
            var pedCopilot = vehicle.CreatePedOnSeat(VehicleSeat.RightFront, new Model(PedHash.Cop01SFY));
            while(!MissionWorld.IsEntityLoaded(pedDriver) || !MissionWorld.IsEntityLoaded(pedCopilot))
            {
                Script.Wait(1);
            }
            pedDriver.RelationshipGroup = copRelGroup;
            pedCopilot.RelationshipGroup = copRelGroup;
            pedDriver.Weapons.Give(WeaponHash.Pistol, 100, true, true);
            pedCopilot.Weapons.Give(WeaponHash.PumpShotgun, 100, true, true);
            police.Add(pedDriver);
            police.Add(pedCopilot);
        }

        vehicle.IsSirenActive = true;
        vehicle.IsSirenSilent = false;

        var driverTask = new TaskSequence();
        driverTask.AddTask.DriveTo(vehicle, Game.Player.Character.Position, 20, 50, DrivingStyle.Rushed);
        driverTask.AddTask.LeaveVehicle();
        driverTask.AddTask.FightAgainstHatedTargets(50);
        driverTask.AddTask.Wait(5000);
        driverTask.AddTask.EnterVehicle(vehicle);
        driverTask.AddTask.Wait(1000);
        driverTask.AddTask.CruiseWithVehicle(vehicle, 50);

        police[0].Task.PerformSequence(driverTask);
        MissionWorld.script.Tick += PoliceTick;
    }

    void PoliceTick(object o, EventArgs e)
    {
        if (vehicle.Exists() && (!Game.Player.Character.IsInRange(vehicle.Position, 320) || vehicle.IsDead))
        {
            vehicle.MarkAsNoLongerNeeded();
        }
        for (var i = 0; i < police.Count; i++)
        {
            if (police[i].Exists() && (!Game.Player.Character.IsInRange(police[i].Position, 320) || police[i].IsDead))
            {
                police[i].MarkAsNoLongerNeeded();
                police.RemoveAt(i);
            }
        }
        if (!vehicle.Exists() && police.Count == 0)
        {
            MissionWorld.script.Tick -= PoliceTick;
            return;
        }
        if (police.Count > 0)
        {
            if (police[0].Exists() && police[0].IsAlive && !leftVehicle && police[0].TaskSequenceProgress == 1)
            {
                if (police.Count < 2)
                {
                    leftVehicle = true;
                    return;
                }
                if (police[1].Exists() && police[1].IsAlive)
                {
                    police[1].Task.LeaveVehicle();
                    leftVehicle = true;
                }
            }
            if (police[0].Exists() && police[0].IsAlive && !dialogDone && police[0].TaskSequenceProgress == 4)
            {
                police[0].PlayAmbientSpeech("GENERIC_CURSE_HIGH");
                dialogDone = true;
            }
            if (police[0].Exists() && police[0].IsAlive && police[0].TaskSequenceProgress == 4 && !gettingInAgain)
            {
                if (police.Count < 2)
                {
                    gettingInAgain = true;
                    return;
                }
                if (police[1].Exists() && police[1].IsAlive && vehicle.Exists() && vehicle.IsAlive)
                {
                    police[1].Task.EnterVehicle(vehicle, VehicleSeat.Passenger);
                    gettingInAgain = true;
                }
            }
        }
    }
}
