﻿using GTA;
using GTA.Native;
using System.Windows.Forms;
using System;

public class VigilanteMissions: Script
{
    bool isPlayerInPoliceVehicle = false;
    Vehicle currentPoliceVehicle;
    MissionWorld missionWorld;
    Menu menu;
    Keys accessComputerKey;
    Keys cancelMissionKey;
    public string accessKey;
    public string cancelKey;
    ScriptSettings iniFile;
    int startTime;
    int currentTime;

    public VigilanteMissions()
    {
        iniFile = ScriptSettings.Load("scripts\\VigilanteMissionsConfig.ini");
        accessKey = iniFile.GetValue("Controls", "AccessComputer", "E");
        cancelKey = iniFile.GetValue("Controls", "CancelMission", "F1");


        if (!Enum.TryParse(accessKey, out accessComputerKey))
        {
            accessKey = "E";
            accessComputerKey = Keys.E;
        }
        if (!Enum.TryParse(cancelKey, out cancelMissionKey))
        {
            cancelKey = "F1";
            cancelMissionKey = Keys.F1;
        }

        missionWorld = new MissionWorld(this);
        menu = new Menu(missionWorld);

        Tick += (o, e) =>
        {
            menu.menuPool.Process();
            CheckIfPlayerIsInPoliceCar();
            CheckIfPlayerDied();
        };

        Tick += (o, e) =>
        {
            if (Game.LastInputMethod == InputMethod.MouseAndKeyboard)
            {
                return;
            }

            if (Game.IsControlJustPressed(GTA.Control.ScriptPadLeft) && missionWorld.isMissionActive)
            {
                startTime = Game.GameTime;
            }

            if (Game.IsControlPressed(GTA.Control.ScriptPadLeft) && missionWorld.isMissionActive)
            {
                currentTime = Game.GameTime - startTime;
                if (currentTime >= 3000)
                {
                    GTA.UI.Screen.ShowSubtitle("~r~Vigilante mission canceled.");
                    missionWorld.QuitMission();
                }
            }

            if (Game.IsControlJustReleased(GTA.Control.ScriptPadRight) && !menu.menuPool.AreAnyVisible && isPlayerInPoliceVehicle && currentPoliceVehicle.IsStopped)
            {
                menu.mainMenu.Visible = true;
            }
        };

        KeyUp += (o, e) =>
        {
            if (e.KeyCode == accessComputerKey && !menu.menuPool.AreAnyVisible && isPlayerInPoliceVehicle && currentPoliceVehicle.IsStopped)
            {
                menu.mainMenu.Visible = true;
            }
            if (e.KeyCode == cancelMissionKey && missionWorld.isMissionActive)
            {
                GTA.UI.Screen.ShowSubtitle("~r~Vigilante mission canceled.");
                missionWorld.QuitMission();
            }
        };
    }

    void CheckIfPlayerIsInPoliceCar()
    {
        isPlayerInPoliceVehicle = Function.Call<bool>(Hash.IS_PED_IN_ANY_POLICE_VEHICLE, Game.Player.Character);
        if (isPlayerInPoliceVehicle)
        {
            currentPoliceVehicle = Function.Call<Vehicle>(Hash.GET_VEHICLE_PED_IS_USING, Game.Player.Character);
            if (currentPoliceVehicle.IsStopped)
            {
                switch(Game.LastInputMethod)
                {
                    case InputMethod.MouseAndKeyboard:
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press {accessKey.ToUpper()} to access the police computer");
                            break;
                        }
                    case InputMethod.GamePad:
                        {
                            GTA.UI.Screen.ShowHelpTextThisFrame($"Press DPad Right to access the police computer");
                            break;
                        }
                }
            }
        } else
        {
            if (menu.menuPool.AreAnyVisible)
            {
                menu.menuPool.HideAll();
                menu.mainMenu.Visible = false;
            }
        }
    }

    void CheckIfPlayerDied()
    {
        if (Game.Player.IsDead && missionWorld.isMissionActive)
        {
            missionWorld.QuitMission();
        }
    }
}