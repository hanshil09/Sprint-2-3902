using System;
using System.Linq;
using TransformersGame.Core;

internal static class RoomChecks
{
    private const int TilesPerPickup = 170;

    public static void Validate(LevelData data, string room, Action<string, bool> check)
    {
        bool isHub = data.Objects.Count(o => o.Kind == ObjectKind.Goal) == 0
            && data.Doors.Count(d => d.Kind.IsTargeted) >= 2;

        check(room + ": pickup density", PickupCountIsReasonable(data));

        if (isHub)
        {
            ValidateHub(data, room, check);
        }
        else
        {
            ValidateSpoke(data, room, check);
        }
    }

    private static void ValidateHub(LevelData data, string room, Action<string, bool> check)
    {
        check(room + ": hub doors all target a room", data.Doors.All(d => d.Kind.IsTargeted));
        check(room + ": hub has a locked door", data.Doors.Any(d => d.Kind.IsLocked));
    }

    private static void ValidateSpoke(LevelData data, string room, Action<string, bool> check)
    {
        check(room + ": at least two enemy kinds",
            data.Enemies.Count >= 2 && data.Enemies.Select(e => e.Kind).Distinct().Count() >= 2);
        bool returnsToHub = data.Doors.Any(d => d.Kind.TargetRoomIndex == 0 || d.Kind.Direction == -1);
        check(room + ": returns to the hub", returnsToHub);
        if (HasElevatedExit(data))
        {
            check(room + ": exit above starting row", ParkourCheck.ExitRow(data) < data.PlayerRow);
            check(room + ": exit reachable", ParkourCheck.CanReachExit(data));
        }
    }

    private static bool HasElevatedExit(LevelData data) =>
        data.Objects.Any(o => o.Kind == ObjectKind.Goal)
        || data.Doors.Any(d => d.Kind.Direction == 1);

    private static bool PickupCountIsReasonable(LevelData data)
    {
        int maxPickups = Math.Max(3, data.Columns * data.Rows / TilesPerPickup);
        int consumables = data.Items.Count(i => i.Kind != ItemKind.Key);
        return consumables <= maxPickups;
    }
}
