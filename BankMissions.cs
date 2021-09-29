using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;

static class BankMissions
{
    public static Vector3 PACIFIC_LOCATION = new Vector3(224.4161f, 208.0313f, 104.5419f);

    public static List<Ped> InitializePacificRobbers()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(1822283721), new Vector3(235.113f, 219.065f, 106.2867f), -172.8443f),
            World.CreatePed(new Model(1822283721), new Vector3(238.0808f, 214.8803f, 106.2868f), 31.00105f),
            World.CreatePed(new Model(1822283721), new Vector3(262.2846f, 208.6228f, 106.2832f), 98.99927f),
            World.CreatePed(new Model(1822283721), new Vector3(262.2047f, 216.5545f, 106.2833f), 167.7561f),
            World.CreatePed(new Model(1822283721), new Vector3(237.4129f, 224.3355f, 110.2827f), -96.24297f),
            World.CreatePed(new Model(1822283721), new Vector3(247.6644f, 211.3228f, 110.283f), -34.46667f),
            World.CreatePed(new Model(1822283721), new Vector3(265.4254f, 220.9125f, 110.2832f), 29.99986f),
            World.CreatePed(new Model(1822283721), new Vector3(255.906f, 227.0302f, 106.2868f), 104.008f),
            World.CreatePed(new Model(1822283721), new Vector3(242.0456f, 228.2701f, 106.2868f), -107.1766f),
            World.CreatePed(new Model(1822283721), new Vector3(245.9437f, 218.2182f, 106.2868f), -16.12338f),
            World.CreatePed(new Model(1822283721), new Vector3(238.5604f, 223.384f, 106.2868f), -106.8362f),
            World.CreatePed(new Model(1822283721), new Vector3(257.0249f, 216.7024f, 106.2868f), 47.99984f),
            World.CreatePed(new Model(1822283721), new Vector3(261.0446f, 220.9603f, 106.2826f), 64.99896f),
            World.CreatePed(new Model(1822283721), new Vector3(261.5011f, 223.2303f, 101.6832f), -65.00141f),
            World.CreatePed(new Model(1822283721), new Vector3(253.2453f, 228.5812f, 101.6832f), 66.993f),
            World.CreatePed(new Model(1822283721), new Vector3(258.706f, 227.2698f, 101.6832f), 121.9991f),
            World.CreatePed(new Model(1822283721), new Vector3(258.2075f, 226.0221f, 101.6832f), 42.99987f)
        };
    }

    public static List<Ped> InitializePacificHostages()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-85696186), new Vector3(242.0825f, 224.7521f, 106.2868f), 160.7987f),
            World.CreatePed(new Model(-85696186), new Vector3(242.8634f, 224.1747f, 106.2868f), 141.9988f),
            World.CreatePed(new Model(835315305), new Vector3(246.1477f, 222.7585f, 106.2868f), 157.5216f),
            World.CreatePed(new Model(373000027), new Vector3(253.916f, 219.5672f, 106.2868f), 167.2536f),
            World.CreatePed(new Model(-283816889), new Vector3(243.5955f, 223.2672f, 106.2868f), 168.0013f),
            World.CreatePed(new Model(-1022961931), new Vector3(245.6687f, 226.3047f, 106.2877f), -21.99996f),
            World.CreatePed(new Model(435429221), new Vector3(246.7914f, 225.898f, 106.2876f), -9.944104f),
            World.CreatePed(new Model(-625565461), new Vector3(248.0685f, 225.5621f, 106.2873f), -0.03393298f),
            World.CreatePed(new Model(1264851357), new Vector3(253.5519f, 223.6367f, 106.2868f), -13.01102f),
            World.CreatePed(new Model(435429221), new Vector3(252.4567f, 224.1626f, 106.2868f), 0.0002091731f),
            World.CreatePed(new Model(835315305), new Vector3(254.6326f, 223.1729f, 106.2868f), -8.628836E-06f),
            World.CreatePed(new Model(-681004504), new Vector3(247.4532f, 222.2631f, 106.2868f), 147.0184f),
            World.CreatePed(new Model(-681004504), new Vector3(251.0426f, 220.9099f, 106.2868f), 159.0132f),
            World.CreatePed(new Model(-681004504), new Vector3(252.5389f, 220.4163f, 106.2867f), 159.0182f)
        };
    }

    public static List<Ped> InitializePacificPolice()
    {
        return new List<Ped>()
        {
            World.CreatePed(new Model(-1920001264), new Vector3(210.0296f, 202.601f, 105.5653f), 125.0188f),
            World.CreatePed(new Model(-1920001264), new Vector3(209.3151f, 204.019f, 105.5574f), 121.0188f),
            World.CreatePed(new Model(-1920001264), new Vector3(207.9868f, 202.091f, 105.5695f), -51.98019f),
            World.CreatePed(new Model(-1920001264), new Vector3(213.881f, 200.4182f, 105.5532f), 109.019f),
            World.CreatePed(new Model(-1920001264), new Vector3(212.5584f, 199.4569f, 105.5726f), -17.9801f),
            World.CreatePed(new Model(-1920001264), new Vector3(251.4264f, 187.8772f, 104.9752f), 97.54115f),
            World.CreatePed(new Model(-1920001264), new Vector3(250.2563f, 186.069f, 105.0499f), 18.02082f),
            World.CreatePed(new Model(-1920001264), new Vector3(248.6306f, 185.7142f, 105.0846f), 2.020158f),
            World.CreatePed(new Model(1558115333), new Vector3(249.0456f, 188.0162f, 105.0529f), -161.0227f),
            World.CreatePed(new Model(1558115333), new Vector3(247.8299f, 187.0601f, 105.0912f), -109.9992f),
            World.CreatePed(new Model(1558115333), new Vector3(211.8093f, 200.7906f, 105.5684f), -120.6286f),
            World.CreatePed(new Model(1558115333), new Vector3(208.9471f, 200.7848f, 105.5767f), 0f),
            World.CreatePed(new Model(1558115333), new Vector3(209.4765f, 195.9827f, 105.5855f), -109.9992f),
            World.CreatePed(new Model(1558115333), new Vector3(210.2475f, 195.0438f, 105.5962f), 3.999995f)
        };
    }

    public static List<Vehicle> InitializePacificVehicles()
    {
        return new List<Vehicle>()
        {
            World.CreateVehicle(new Model(-1205689942), new Vector3(211.2739f, 206.079f, 105.1331f), 22.38695f),
            World.CreateVehicle(new Model(-1205689942), new Vector3(216.6329f, 199.5738f, 105.1084f), -172.4375f),
            World.CreateVehicle(new Model(-1205689942), new Vector3(252.5587f, 189.4736f, 104.492f), -141.7779f),
            World.CreateVehicle(new Model(1127131465), new Vector3(211.0282f, 196.9797f, 105.2428f), 48.82729f),
            World.CreateVehicle(new Model(-1647941228), new Vector3(245.9201f, 191.9574f, 104.6282f), 67.79448f)
        };
    }
}
