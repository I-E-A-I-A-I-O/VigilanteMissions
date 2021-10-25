using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;

static class RandomMissions
{
    static readonly List<Vector3> LOCATIONS_FOOT = new List<Vector3>()
    {
       new Vector3(958.3557f, -141.5549f, 75.50748f),
       new Vector3(1164.845f, -334.2931f, 69.74226f),
       new Vector3(1214.709f, -420.8012f, 68.62407f),
       new Vector3(1155.659f, -473.3027f, 67.51048f),
       new Vector3(788.7799f, -738.5971f, 28.53819f),
       new Vector3(415.7993f, -804.9828f, 30.36194f),
       new Vector3(-115.3424f, -426.6558f, 36.8196f),
       new Vector3(-318.0066f, 224.781f, 88.75639f),
       new Vector3(-235.7711f, 276.5725f, 93.04496f),
       new Vector3(439.0587f, 133.8473f, 101.1775f),
       new Vector3(432.2224f, 225.0025f, 104.1669f),
       new Vector3(-686.5708f, 273.3582f, 82.44141f),
       new Vector3(-1223.188f, -264.502f, 39.41944f),
       new Vector3(-1096.87f, -801.2212f, 19.64473f),
       new Vector3(-1227.51f, -1171.697f, 8.635307f),
       new Vector3(-841.7546f, -1204.689f, 7.395357f),
       new Vector3(157.1279f, -967.2166f, 31.09192f),
       new Vector3(237.4271f, -390.4071f, 47.30567f),
       new Vector3(66.69101f, 113.9236f, 80.11262f),
       new Vector3(-564.027f, 327.2527f, 85.40903f),
       new Vector3(-219.9077f, -1957.957f, 27.75663f),
       new Vector3(-325.6057f, -1462.222f, 30.51306f),
       new Vector3(-172.0509f, -1436.433f, 31.2634f),
       new Vector3(-105.1376f, -1094.589f, 26.06644f),
       new Vector3(-63.15568f, -1107.807f, 26.32477f),
       new Vector3(-208.8304f, -631.2042f, 48.22083f),
       new Vector3(-175.8562f, -156.3028f, 43.62122f),
       new Vector3(325.5165f, -211.5939f, 54.08629f),
       new Vector3(458.5976f, -753.1367f, 27.35779f),
       new Vector3(423.4489f, -1286.861f, 30.25341f),
       new Vector3(286.4246f, -1584.264f, 30.53215f),
       new Vector3(102.1467f, -1943f, 20.80371f),
       new Vector3(-1020.126f, -2701.373f, 13.75664f),
       new Vector3(-1641.421f, -1108.605f, 13.02085f),
       new Vector3(-1836.245f, -1219.252f, 13.01726f),
       new Vector3(-1820.584f, -862.6905f, 4.100582f),
       new Vector3(-1660.969f, -900.475f, 8.538025f),
       new Vector3(-1296.6f, -1501.365f, 4.344109f),
       new Vector3(-1165.643f, -1514.781f, 4.383816f),
       new Vector3(-331.8909f, -911.0989f, 31.08062f),
       new Vector3(-739.7289f, -66.36839f, 41.75113f),
       new Vector3(-844.1396f, -108.4216f, 28.18535f),
       new Vector3(-425.4473f, 1128.7f, 325.8547f),
       new Vector3(-105.5424f, 911.351f, 236.1674f),
       new Vector3(958.9243f, -1742.755f, 31.12148f),
       new Vector3(1208.936f, -1756.755f, 39.63063f),
       new Vector3(971.1397f, -1457.827f, 31.34114f),
       new Vector3(987.2574f, -1409.128f, 31.26411f),
       new Vector3(264.4848f, -1278.07f, 29.16894f),
       new Vector3(-231.7598f, -1165.854f, 22.90478f),
       new Vector3(-603.5284f, -877.6115f, 25.35468f),
       new Vector3(-931.6569f, -796.4459f, 15.92116f),
       new Vector3(-1378.364f, 47.46008f, 53.68186f),
       new Vector3(-1237.991f, 485.7172f, 93.16705f),
       new Vector3(-1888.959f, 2028.161f, 140.7174f),
       new Vector3(-2551.821f, 2322.683f, 33.05998f),
       new Vector3(-3156.103f, 1068.687f, 20.67572f),
       new Vector3(-2980.058f, 75.9231f, 11.6085f),
       new Vector3(-2202.769f, 4261.527f, 47.83957f),
       new Vector3(-212.3218f, 6198.221f, 31.48867f),
       new Vector3(160.8905f, 6618.127f, 31.92143f),
       new Vector3(1688.108f, 4780.192f, 41.92149f),
       new Vector3(1660.526f, 4827.077f, 42.01349f),
       new Vector3(1711.821f, 3761.432f, 34.20344f),
       new Vector3(1420.286f, 3621.922f, 34.87617f),
       new Vector3(1992.391f, 3768.693f, 32.1808f),
       new Vector3(2488.09f, 4116.335f, 38.0647f),
       new Vector3(2499.898f, 4086.932f, 38.37713f),
       new Vector3(2752.527f, 3456.097f, 55.94258f),
       new Vector3(2704.049f, 3447.708f, 55.80101f),
       new Vector3(1998.903f, 3060.598f, 47.04928f),
       new Vector3(1210.369f, 2702.171f, 38.00581f),
       new Vector3(1177.054f, 2653.476f, 37.93536f),
       new Vector3(1123.178f, 2655.729f, 37.99691f),
       new Vector3(1050.356f, 2667.707f, 39.55112f),
       new Vector3(616.5576f, 2734.069f, 42.01853f)
    };

    static readonly List<Vector3> LOCATIONS_VEHICLE = new List<Vector3>()
    {
        new Vector3(-1326.338f, 5136.826f, 61.98092f), 
        new Vector3(-337.7497f, 6069.629f, 31.73389f), 
        new Vector3(-180.8488f, 6465.757f, 31.20587f), 
        new Vector3(172.2699f, 6579.066f, 32.53137f),
        new Vector3(1727.338f, 6398.668f, 35.22195f),
        new Vector3(2547.849f, 5319.684f, 44.95111f),
        new Vector3(2544.552f, 2621.24f, 38.3621f),
        new Vector3(2666.361f, 1673.555f, 25.20385f),
        new Vector3(2580.545f, 441.4199f, 109.0808f),
        new Vector3(408.2307f, -2070.269f, 21.63659f), 
        new Vector3(-86.95599f, -2115.876f, 17.26918f),
        new Vector3(-556.7122f, -2136.262f, 6.531339f),
        new Vector3(-1187.478f, -1495.184f, 5.097012f),
        new Vector3(-1651.682f, -923.522f, 9.012685f),
        new Vector3(-1476.117f, -501.9565f, 33.09459f),
        new Vector3(-188.0657f, -149.6397f, 44.20177f),
        new Vector3(230.8618f, -789.9957f, 30.94275f), 
        new Vector3(-195.1992f, 282.343f, 93.2417f),
        new Vector3(-1394.672f, 58.78949f, 54.4501f),
        new Vector3(-1949.325f, -335.7829f, 46.1995f),
        new Vector3(208.6546f, 226.1538f, 105.6051f),
        new Vector3(100.5251f, -20.41864f, 68.01024f),
        new Vector3(61.86081f, -301.5036f, 47.17083f),
        new Vector3(467.2407f, -532.9135f, 35.98397f),
        new Vector3(423.3599f, -607.2281f, 28.49987f),
        new Vector3(323.2897f, -850.3389f, 29.34249f),
        new Vector3(501.5409f, -994.3605f, 27.74894f),
        new Vector3(531.5969f, -1036.542f, 37.50127f),
        new Vector3(726.5676f, -1190.626f, 44.8922f),
        new Vector3(573.3943f, -1428.351f, 29.6075f),
        new Vector3(190.251f, -1774.328f, 29.17102f),
        new Vector3(-64.03038f, -2054.813f, 21.7747f),
        new Vector3(-90.87307f, -2020.987f, 18.01695f),
        new Vector3(-405.5422f, -1809.907f, 21.50559f),
        new Vector3(-581.2361f, -1956.833f, 27.20109f),
        new Vector3(-710.875f, -2122.791f, 42.36239f),
        new Vector3(-713.8049f, -1950.05f, 27.14961f),
        new Vector3(-983.3277f, -2616.08f, 13.8295f),
        new Vector3(-1170.747f, -1460.43f, 4.957169f),
        new Vector3(-1213.78f, -1197.595f, 7.694859f),
        new Vector3(-1156.907f, -825.1862f, 14.44065f),
        new Vector3(-1394.662f, -579.5674f, 30.26363f),
        new Vector3(-1554.632f, -326.2013f, 46.85171f),
        new Vector3(-1786.169f, 90.07755f, 76.46387f),
        new Vector3(-1612.345f, 375.1869f, 89.97917f),
        new Vector3(-1882.838f, 699.646f, 128.3672f),
        new Vector3(-2010.467f, 705.9418f, 145.5211f),
        new Vector3(-2621.259f, 1127.152f, 162.2988f),
        new Vector3(-2302.596f, 449.2766f, 174.4667f),
        new Vector3(-1998.635f, -162.3358f, 29.56968f),
        new Vector3(-2022.341f, -476.3376f, 11.368f),
        new Vector3(-2182.905f, -337.427f, 13.20371f),
        new Vector3(-2526.534f, -193.4347f, 19.31018f),
        new Vector3(-3025.761f, 105.4377f, 11.62844f),
        new Vector3(-3106.848f, 1096.672f, 20.47231f),
        new Vector3(-3024.62f, 1874.819f, 29.89819f),
        new Vector3(-2545.359f, 1906.313f, 168.9719f),
        new Vector3(-1898.068f, 2016.655f, 140.9769f),
        new Vector3(-1593.878f, 2352.143f, 43.58834f),
        new Vector3(-1142.753f, 2662.476f, 18.0209f),
        new Vector3(-516.3898f, 2840.728f, 33.98786f),
        new Vector3(847.5055f, 2846.733f, 58.57842f),
        new Vector3(1211.186f, 2715.805f, 38.00459f),
        new Vector3(1230.549f, 2684.057f, 37.6313f),
        new Vector3(2052.189f, 3018.868f, 45.46199f),
        new Vector3(2254.444f, 3194.383f, 48.69269f),
        new Vector3(2642.698f, 3116.496f, 49.37523f),
        new Vector3(2623.745f, 3129.004f, 49.28777f),
        new Vector3(2757.665f, 3450.729f, 55.9211f),
        new Vector3(2429.007f, 3963.394f, 36.57501f),
        new Vector3(2486.238f, 4118.029f, 38.0647f),
        new Vector3(1800.934f, 4586.852f, 37.10669f),
        new Vector3(78.46184f, 3627.929f, 39.6853f),
        new Vector3(337.3323f, 3422.375f, 36.39695f),
        new Vector3(748.8038f, 3595.673f, 32.978f),
        new Vector3(-2201.213f, 4261.719f, 47.933f)
    };

    static readonly List<VehicleHash> VEHICLES = new List<VehicleHash>()
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

    static readonly List<PedHash> VICTIMS = new List<PedHash>()
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

    static readonly List<PedHash> CRIMINALS = new List<PedHash>()
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

    static public Vector3 GetRandomLocation(LocationType type)
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

    static public Ped CreateVictim(Vector3 location)
    {
        var ran = new Random();
        var model = new Model(VICTIMS[ran.Next(0, VICTIMS.Count)]);
        Loading.LoadModel(model);
        var ped = World.CreatePed(new Model(VICTIMS[ran.Next(0, VICTIMS.Count)]), location.Around(2));
        Loading.UnloadModel(model);
        return ped;
    }

    static public Ped CreateCriminal(Vector3 location)
    {
        var ran = new Random();
        var model = new Model(CRIMINALS[ran.Next(0, CRIMINALS.Count)]);
        Loading.LoadModel(model);
        var ped = World.CreatePed(model, location.Around(2));
        Loading.UnloadModel(model);
        return ped;
    }

    static public Vehicle CreateVehicle(Vector3 location)
    {
        var ran = new Random();
        var model = new Model(VEHICLES[ran.Next(0, VEHICLES.Count)]);
        Loading.LoadModel(model);
        var vehicle = World.CreateVehicle(model, location.Around(2));
        Loading.UnloadModel(model);
        return vehicle;
    }

    static public List<Ped> CreateGroupOfCriminals(Vector3 location)
    {
        var ran = new Random();
        var groupSize = ran.Next(4, 13);
        var peds = new List<Ped>();
        var models = new List<Model>();
        for (var i = 0; i < groupSize; i++)
        {
            models.Add(new Model(CRIMINALS[ran.Next(0, CRIMINALS.Count)]));
        }
        Loading.LoadModels(models.Distinct().ToList());
        foreach (Model model in models)
        {
            peds.Add(World.CreatePed(model, location.Around(5), ran.Next(0, 351)));
        }
        Loading.UnloadModels(models.Distinct().ToList());
        return peds;
    }
}

