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
        var ran = new Random();
        var backupSize = ran.Next(1, 3);

        var outPos = new OutputArgument();
        var posAroundPlayer = Game.Player.Character.Position.Around(100);
        var result = Function.Call<bool>(Hash.GET_CLOSEST_VEHICLE_NODE, posAroundPlayer.X, posAroundPlayer.Y, posAroundPlayer.Z, outPos, 0, 3, 0);
        var vehiclePos = outPos.GetResult<Vector3>();

        if (backupSize == 1)
        {
            var models = new List<Model>() { new Model(VehicleHash.Policeb), new Model(PedHash.Hwaycop01SMY) };
            Loading.LoadModels(models);
            vehicle = World.CreateVehicle(models[0], vehiclePos);
            var ped = vehicle.CreatePedOnSeat(VehicleSeat.Driver, models[1]);
            Loading.LoadModels(models);
            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, ped.Handle, Game.GenerateHash("COP"));
            Function.Call(Hash.SET_PED_AS_COP, ped.Handle, true);
            ped.Weapons.Give(WeaponHash.Pistol, 100, true, true);
            police.Add(ped);
        } else
        {
            var vehicleModel = new Model(vehicleHashList[ran.Next(0, vehicleHashList.Count)]);
            var models = new List<Model>() { vehicleModel, new Model(PedHash.Cop01SMY), new Model(PedHash.Cop01SFY) };
            Loading.LoadModels(models);
            vehicle = World.CreateVehicle(models[0], vehiclePos);
            var pedDriver = vehicle.CreatePedOnSeat(VehicleSeat.Driver, models[1]);
            var pedCopilot = vehicle.CreatePedOnSeat(VehicleSeat.RightFront, models[2]);
            Loading.UnloadModels(models);
            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, pedDriver.Handle, Game.GenerateHash("COP"));
            Function.Call(Hash.SET_PED_RELATIONSHIP_GROUP_HASH, pedCopilot.Handle, Game.GenerateHash("COP"));
            pedDriver.Weapons.Give(WeaponHash.Pistol, 100, true, true);
            pedCopilot.Weapons.Give(WeaponHash.PumpShotgun, 100, true, true);
            Function.Call(Hash.SET_PED_AS_COP, pedDriver.Handle, true);
            Function.Call(Hash.SET_PED_AS_COP, pedCopilot.Handle, true);
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
