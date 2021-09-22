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
    Script script;
    MissionWorld mission;

    readonly List<string> randomEvents = new List<string>()
    {
        "Stolen vehicle",
        "Assault",
        "Gang activity",
        "Suspect on the run",
    };

    public Menu(MissionWorld mission, Script script)
    {
        this.mission = mission;
        this.script = script;

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
            new NativeItem("Harry \"Taco\" Bowman - Racketeering")
        };
        for (var i = 0; i < listOfItems.Count; i++)
        {
            mostWantedMenu.Add(listOfItems[i]);
            var index = i;
            listOfItems[i].Activated += (o, e) =>
            {
                if (mission.isMissionActive)
                {
                    GTA.UI.Notification.Show("A mission is already in progress!");
                    return;
                }
                if (!Game.Player.CanStartMission)
                {
                    GTA.UI.Notification.Show("Mission not available.");
                    return;
                }
                mission.StartMission((MissionWorld.Missions)index);
            };
        }
        currentCrimesMenu = new NativeMenu("Current crimes", "Current crimes");
        menuPool.Add(currentCrimesMenu);
        mainMenu.AddSubMenu(currentCrimesMenu);
        var callBackupOption = new NativeItem("Call police backup");
        mainMenu.Add(callBackupOption);
        callBackupOption.Activated += BackUpCalled;
        currentCrimesMenu.Shown += CurrentCrimesMenu_Shown;
    }

    void BackUpCalled(object o, EventArgs e)
    {
        new PoliceBackup(script);
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
                if (mission.isMissionActive)
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
                            mission.StartMission(MissionWorld.Missions.SuspectOnTheRun);
                            break;
                        }
                    case "Assault":
                        {
                            mission.StartMission(MissionWorld.Missions.Assault);
                            break;
                        }
                    case "Gang activity":
                        {
                            mission.StartMission(MissionWorld.Missions.GangActivity);
                            break;
                        }
                    case "Stolen vehicle":
                        {
                            mission.StartMission(MissionWorld.Missions.StolenVehicle);
                            break;
                        }
                }
            };
        }
    }
}
