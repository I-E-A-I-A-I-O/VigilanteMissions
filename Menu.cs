using LemonUI;
using LemonUI.Menus;
using GTA;
using System;
using System.Collections.Generic;

class Menu
{
    public ObjectPool menuPool;
    public NativeMenu mainMenu;
    public NativeMenu mostWantedMenu;
    public NativeMenu currentCrimesMenu;
    public NativeItem callBackupOption;
    bool timerStarted = false;
    int startTime;
    int currentTime;

    readonly List<string> randomEvents = new List<string>()
    {
        "Stolen vehicle",
        "Assault",
        "Gang activity",
        "Suspect on the run",
        "Mass shooter"
    };

    public Menu()
    {
        menuPool = new ObjectPool();
        mainMenu = new NativeMenu("Police computer", "Los Santos county database");
        menuPool.Add(mainMenu);
        mostWantedMenu = new NativeMenu("Most Wanted", "Most wanted");
        menuPool.Add(mostWantedMenu);
        mainMenu.AddSubMenu(mostWantedMenu);
        var listOfItems = new List<NativeItem>()
        {
            new NativeItem("Wang Fang - Drug trafficking"),
            new NativeItem("Tony Mussolini - Racketeering"),
            new NativeItem("Martin Madrazo - Tax Evasion"),
            new NativeItem("Frank Abagnale - Check fraud"),
            new NativeItem("Ted Bundy - Murder"),
            new NativeItem("Song Jiang - Human Trafficking"),
            new NativeItem("Catherine Kerkow - Terrorism"),
            new NativeItem("Harry \"Taco\" Bowman - Racketeering"),
            new NativeItem("Heisenberg - Drug trafficking"),
            new NativeItem("Billy \"The Beaut\" Russo - Murder")
        };
        for (var i = 0; i < listOfItems.Count; i++)
        {
            mostWantedMenu.Add(listOfItems[i]);
            var index = i;
            listOfItems[i].Activated += (o, e) =>
            {
                if (MissionWorld.isMissionActive)
                {
                    GTA.UI.Notification.Show("A mission is already in progress!");
                    return;
                }
                if (!Game.Player.CanStartMission)
                {
                    GTA.UI.Notification.Show("Mission not available.");
                    return;
                }
                MissionWorld.StartMission((MissionWorld.Missions)index);
            };
        }
        currentCrimesMenu = new NativeMenu("Current crimes", "Current crimes");
        menuPool.Add(currentCrimesMenu);
        mainMenu.AddSubMenu(currentCrimesMenu);
        callBackupOption = new NativeItem("Call police backup");
        mainMenu.Add(callBackupOption);
        callBackupOption.Activated += BackUpCalled;
        currentCrimesMenu.Shown += CurrentCrimesMenu_Shown;
    }

    void BackUpCalled(object o, EventArgs e)
    {
        if (!timerStarted)
        {
            new PoliceBackup(MissionWorld.script);
            timerStarted = true;
            startTime = Game.GameTime;
        } else
        {
            currentTime = Game.GameTime;
            if (currentTime - startTime >= 60000)
            {
                timerStarted = false;
                new PoliceBackup(MissionWorld.script);
            } else
            {
                GTA.UI.Notification.Show($"Next police backup available in {60 - ((currentTime - startTime) / 1000)} seconds");
            }
        }
    }

    private void CurrentCrimesMenu_Shown(object sender, EventArgs e)
    {
        var random = new Random();
        currentCrimesMenu.Clear();
        for (var i = 0; i < 3; i++)
        {
            var text = randomEvents[random.Next(0, randomEvents.Count)];
            var item = new NativeItem(text);
            currentCrimesMenu.Add(item);
            item.Activated += (o, ev) =>
            {
                if (MissionWorld.isMissionActive)
                {
                    GTA.UI.Notification.Show("A mission is already in progress!");
                    return;
                }
                if (!Game.Player.CanStartMission)
                {
                    GTA.UI.Notification.Show("Mission not available.");
                    return;
                }
                switch (text)
                {
                    case "Suspect on the run":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.SuspectOnTheRun);
                            break;
                        }
                    case "Assault":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.Assault);
                            break;
                        }
                    case "Gang activity":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.GangActivity);
                            break;
                        }
                    case "Stolen vehicle":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.StolenVehicle);
                            break;
                        }
                    case "Mass shooter":
                        {
                            MissionWorld.StartMission(MissionWorld.Missions.MassShooter);
                            break;
                        }
                }
            };
        }
    }
}
