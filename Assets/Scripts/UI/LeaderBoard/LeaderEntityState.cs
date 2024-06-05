using System;
using Unity.Collections;
using Unity.Netcode;

public struct LeaderEntityState : INetworkSerializable, IEquatable<LeaderEntityState>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Coins;
    public bool Equals(LeaderEntityState other)
    {
        return ClientId == other.ClientId && PlayerName.Equals(other.PlayerName) && Coins == other.Coins;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
        serializer.SerializeValue(ref Coins);
    }
}
