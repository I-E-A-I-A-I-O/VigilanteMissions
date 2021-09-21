using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;

class RandomMissions
{
    readonly List<Vector3> LOCATIONS_FOOT = new List<Vector3>()
    {
       new Vector3(958.3557f, -141.5549f, 74.50748f),
       new Vector3(1164.845f, -334.2931f, 68.74226f),
       new Vector3(1214.709f, -420.8012f, 67.62407f),
       new Vector3(1155.659f, -473.3027f, 66.51048f),
       new Vector3(788.7799f, -738.5971f, 27.53819f),
       new Vector3(415.7993f, -804.9828f, 29.36194f),
       new Vector3(-115.3424f, -426.6558f, 35.8196f),
       new Vector3(-318.0066f, 224.781f, 87.75639f),
       new Vector3(-235.7711f, 276.5725f, 92.04496f),
       new Vector3(439.0587f, 133.8473f, 100.1775f),
       new Vector3(432.2224f, 225.0025f, 103.1669f),
       new Vector3(-686.5708f, 273.3582f, 81.44141f),
       new Vector3(-1223.188f, -264.502f, 38.41944f),
       new Vector3(-1096.87f, -801.2212f, 18.64473f),
       new Vector3(-1227.51f, -1171.697f, 7.635307f),
       new Vector3(-841.7546f, -1204.689f, 6.395357f),
       new Vector3(157.1279f, -967.2166f, 30.09192f),
       new Vector3(237.4271f, -390.4071f, 46.30567f),
       new Vector3(66.69101f, 113.9236f, 79.11262f),
       new Vector3(-564.027f, 327.2527f, 84.40903f),
       new Vector3(-219.9077f, -1957.957f, 26.75663f),
       new Vector3(-325.6057f, -1462.222f, 29.51306f),
       new Vector3(-172.0509f, -1436.433f, 30.2634f),
       new Vector3(-105.1376f, -1094.589f, 25.06644f),
       new Vector3(-63.15568f, -1107.807f, 25.32477f),
       new Vector3(-208.8304f, -631.2042f, 47.22083f),
       new Vector3(-175.8562f, -156.3028f, 42.62122f),
       new Vector3(325.5165f, -211.5939f, 53.08629f),
       new Vector3(458.5976f, -753.1367f, 26.35779f),
       new Vector3(423.4489f, -1286.861f, 29.25341f),
       new Vector3(286.4246f, -1584.264f, 29.53215f),
       new Vector3(102.1467f, -1943f, 19.80371f),
       new Vector3(-1020.126f, -2701.373f, 12.75664f),
       new Vector3(-1641.421f, -1108.605f, 12.02085f),
       new Vector3(-1836.245f, -1219.252f, 12.01726f),
       new Vector3(-1820.584f, -862.6905f, 3.100582f),
       new Vector3(-1660.969f, -900.475f, 7.538025f),
       new Vector3(-1296.6f, -1501.365f, 3.344109f),
       new Vector3(-1165.643f, -1514.781f, 3.383816f),
       new Vector3(-331.8909f, -911.0989f, 30.08062f),
       new Vector3(-739.7289f, -66.36839f, 40.75113f),
       new Vector3(-844.1396f, -108.4216f, 27.18535f),
       new Vector3(-425.4473f, 1128.7f, 324.8547f),
       new Vector3(-105.5424f, 911.351f, 235.1674f),
       new Vector3(958.9243f, -1742.755f, 30.12148f),
       new Vector3(1208.936f, -1756.755f, 38.63063f),
       new Vector3(971.1397f, -1457.827f, 30.34114f),
       new Vector3(987.2574f, -1409.128f, 30.26411f),
       new Vector3(264.4848f, -1278.07f, 28.16894f),
       new Vector3(-231.7598f, -1165.854f, 21.90478f),
       new Vector3(-603.5284f, -877.6115f, 24.35468f),
       new Vector3(-931.6569f, -796.4459f, 14.92116f),
       new Vector3(-1378.364f, 47.46008f, 52.68186f),
       new Vector3(-1237.991f, 485.7172f, 92.16705f),
       new Vector3(-1888.959f, 2028.161f, 139.7174f),
       new Vector3(-2551.821f, 2322.683f, 32.05998f),
       new Vector3(-3156.103f, 1068.687f, 19.67572f),
       new Vector3(-2980.058f, 75.9231f, 10.6085f),
       new Vector3(-2202.769f, 4261.527f, 46.83957f),
       new Vector3(-212.3218f, 6198.221f, 30.48867f),
       new Vector3(160.8905f, 6618.127f, 30.92143f),
       new Vector3(1688.108f, 4780.192f, 40.92149f),
       new Vector3(1660.526f, 4827.077f, 41.01349f),
       new Vector3(1711.821f, 3761.432f, 33.20344f),
       new Vector3(1420.286f, 3621.922f, 33.87617f),
       new Vector3(1992.391f, 3768.693f, 31.1808f),
       new Vector3(2488.09f, 4116.335f, 37.0647f),
       new Vector3(2499.898f, 4086.932f, 37.37713f),
       new Vector3(2752.527f, 3456.097f, 54.94258f),
       new Vector3(2704.049f, 3447.708f, 54.80101f),
       new Vector3(1998.903f, 3060.598f, 46.04928f),
       new Vector3(1210.369f, 2702.171f, 37.00581f),
       new Vector3(1177.054f, 2653.476f, 36.93536f),
       new Vector3(1123.178f, 2655.729f, 36.99691f),
       new Vector3(1050.356f, 2667.707f, 38.55112f),
       new Vector3(616.5576f, 2734.069f, 41.01853f)
    };
    readonly List<Vector3> LOCATIONS_VEHICLE = new List<Vector3>()
    {
        new Vector3(-1326.338f, 5136.826f, 60.98092f), 
        new Vector3(-337.7497f, 6069.629f, 30.73389f), 
        new Vector3(-180.8488f, 6465.757f, 30.20587f), 
        new Vector3(172.2699f, 6579.066f, 31.53137f),
        new Vector3(1727.338f, 6398.668f, 34.22195f),
        new Vector3(2547.849f, 5319.684f, 43.95111f),
        new Vector3(2544.552f, 2621.24f, 37.3621f),
        new Vector3(2666.361f, 1673.555f, 24.20385f),
        new Vector3(2580.545f, 441.4199f, 108.0808f),
        new Vector3(408.2307f, -2070.269f, 20.63659f), 
        new Vector3(-86.95599f, -2115.876f, 16.26918f),
        new Vector3(-556.7122f, -2136.262f, 5.531339f),
        new Vector3(-1187.478f, -1495.184f, 4.097012f),
        new Vector3(-1651.682f, -923.522f, 8.012685f),
        new Vector3(-1476.117f, -501.9565f, 32.09459f),
        new Vector3(-188.0657f, -149.6397f, 43.20177f),
        new Vector3(230.8618f, -789.9957f, 29.94275f), 
        new Vector3(-195.1992f, 282.343f, 92.2417f),
        new Vector3(-1394.672f, 58.78949f, 53.4501f),
        new Vector3(-1949.325f, -335.7829f, 45.1995f),
        new Vector3(208.6546f, 226.1538f, 104.6051f),
        new Vector3(100.5251f, -20.41864f, 67.01024f),
        new Vector3(61.86081f, -301.5036f, 46.17083f),
        new Vector3(467.2407f, -532.9135f, 34.98397f),
        new Vector3(423.3599f, -607.2281f, 27.49987f),
        new Vector3(323.2897f, -850.3389f, 28.34249f),
        new Vector3(501.5409f, -994.3605f, 26.74894f),
        new Vector3(531.5969f, -1036.542f, 36.50127f),
        new Vector3(726.5676f, -1190.626f, 43.8922f),
        new Vector3(573.3943f, -1428.351f, 28.6075f),
        new Vector3(190.251f, -1774.328f, 28.17102f),
        new Vector3(-64.03038f, -2054.813f, 20.7747f),
        new Vector3(-90.87307f, -2020.987f, 17.01695f),
        new Vector3(-405.5422f, -1809.907f, 20.50559f),
        new Vector3(-581.2361f, -1956.833f, 26.20109f),
        new Vector3(-710.875f, -2122.791f, 41.36239f),
        new Vector3(-713.8049f, -1950.05f, 26.14961f),
        new Vector3(-983.3277f, -2616.08f, 12.8295f),
        new Vector3(-1170.747f, -1460.43f, 3.957169f),
        new Vector3(-1213.78f, -1197.595f, 6.694859f),
        new Vector3(-1156.907f, -825.1862f, 13.44065f),
        new Vector3(-1394.662f, -579.5674f, 29.26363f),
        new Vector3(-1554.632f, -326.2013f, 45.85171f),
        new Vector3(-1786.169f, 90.07755f, 75.46387f),
        new Vector3(-1612.345f, 375.1869f, 88.97917f),
        new Vector3(-1882.838f, 699.646f, 127.3672f),
        new Vector3(-2010.467f, 705.9418f, 144.5211f),
        new Vector3(-2621.259f, 1127.152f, 161.2988f),
        new Vector3(-2302.596f, 449.2766f, 173.4667f),
        new Vector3(-1998.635f, -162.3358f, 28.56968f),
        new Vector3(-2022.341f, -476.3376f, 10.368f),
        new Vector3(-2182.905f, -337.427f, 12.20371f),
        new Vector3(-2526.534f, -193.4347f, 18.31018f),
        new Vector3(-3025.761f, 105.4377f, 10.62844f),
        new Vector3(-3106.848f, 1096.672f, 19.47231f),
        new Vector3(-3024.62f, 1874.819f, 28.89819f),
        new Vector3(-2545.359f, 1906.313f, 167.9719f),
        new Vector3(-1898.068f, 2016.655f, 139.9769f),
        new Vector3(-1593.878f, 2352.143f, 42.58834f),
        new Vector3(-1142.753f, 2662.476f, 17.0209f),
        new Vector3(-516.3898f, 2840.728f, 32.98786f),
        new Vector3(847.5055f, 2846.733f, 57.57842f),
        new Vector3(1211.186f, 2715.805f, 37.00459f),
        new Vector3(1230.549f, 2684.057f, 36.6313f),
        new Vector3(2052.189f, 3018.868f, 44.46199f),
        new Vector3(2254.444f, 3194.383f, 47.69269f),
        new Vector3(2642.698f, 3116.496f, 48.37523f),
        new Vector3(2623.745f, 3129.004f, 48.28777f),
        new Vector3(2757.665f, 3450.729f, 54.9211f),
        new Vector3(2429.007f, 3963.394f, 35.57501f),
        new Vector3(2486.238f, 4118.029f, 37.0647f),
        new Vector3(1800.934f, 4586.852f, 36.10669f),
        new Vector3(78.46184f, 3627.929f, 38.6853f),
        new Vector3(337.3323f, 3422.375f, 35.39695f),
        new Vector3(748.8038f, 3595.673f, 31.978f),
        new Vector3(-2201.213f, 4261.719f, 46.933f)
    };
    readonly List<VehicleHash> VEHICLES = new List<VehicleHash>()
    {
        VehicleHash.Zentorno,
        VehicleHash.SultanRS,
        VehicleHash.Sultan,
        VehicleHash.Sentinel,
        VehicleHash.SabreGT,
        VehicleHash.Phoenix,
        VehicleHash.Pfister811,
        VehicleHash.Penumbra,
        VehicleHash.Cyclone,
        VehicleHash.Ruiner,
        VehicleHash.JB700,
        VehicleHash.Cheetah,
        VehicleHash.Comet2,
        VehicleHash.Banshee,
        VehicleHash.Bullet
    };
    readonly List<PedHash> VICTIMS = new List<PedHash>()
    {
        PedHash.Vinewood01AFY,
        PedHash.Vinewood01AMY,
        PedHash.Vinewood04AFY,
        PedHash.Tourist01AFM,
        PedHash.Tourist01AFY,
        PedHash.Hooker01SFY,
        PedHash.Hipster01AFY,
        PedHash.Hipster03AFY,
        PedHash.Hipster02AMY,
        PedHash.Hiker01AMY,
        PedHash.Runner01AMY,
        PedHash.FatWhite01AFM
    };
    readonly List<PedHash> CRIMINALS = new List<PedHash>()
    {
        PedHash.ArmGoon01GMM,
        PedHash.MexGang01GMY,
        PedHash.MexGoon01GMY,
        PedHash.PoloGoon01GMY,
        PedHash.SalvaGoon01GMY,
        PedHash.Methhead01AMY,
        PedHash.Hillbilly01AMM,
        PedHash.Lost01GFY,
        PedHash.Lost01GMY,
        PedHash.Robber01SMY,
        PedHash.ChiGoon01GMM
    };

    public enum LocationType
    {
        Foot,
        Vehicle
    }

    public Vector3 GetRandomLocation(LocationType type)
    {
        var ran = new Random();
        if (type == LocationType.Foot)
        {
            return LOCATIONS_FOOT[ran.Next(0, LOCATIONS_FOOT.Count)];
        } else
        {
            return LOCATIONS_FOOT[ran.Next(0, LOCATIONS_VEHICLE.Count)];
        }
    }

    public Ped CreateVictim(Vector3 location)
    {
        var ran = new Random();
        return World.CreatePed(new Model(VICTIMS[ran.Next(0, VICTIMS.Count)]), location.Around(2));
    }

    public Ped CreateCriminal(Vector3 location)
    {
        var ran = new Random();
        return World.CreatePed(new Model(CRIMINALS[ran.Next(0, CRIMINALS.Count)]), location.Around(2));
    }

    public Vehicle CreateVehicle(Vector3 location)
    {
        var ran = new Random();
        return World.CreateVehicle(new Model(VEHICLES[ran.Next(0, VEHICLES.Count)]), location.Around(2));
    }

    public List<Ped> CreateGroupOfCriminals(Vector3 location)
    {
        var ran = new Random();
        var groupSize = ran.Next(4, 13);
        var peds = new List<Ped>();
        for (var i = 0; i < groupSize; i++)
        {
            peds.Add(World.CreatePed(new Model(CRIMINALS[ran.Next(0, CRIMINALS.Count)]), location.Around(5), ran.Next(0, 351)));
        }
        return peds;
    }
}

