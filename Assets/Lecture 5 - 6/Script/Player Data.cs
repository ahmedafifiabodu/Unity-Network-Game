using Unity.Collections;
using Unity.Netcode;

namespace NGO_ToonTanks
{
    internal enum Team
    {
        Team1 = 0,
        Team2 = 1
    }

    internal enum PlayerType
    {
        Tank = 0,
        DPS = 1
    }

    public struct PlayerData : INetworkSerializable
    {
        internal FixedString64Bytes PlayerName;
        internal Team PlayerTeam;
        internal PlayerType PlayerType;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
            {
                FastBufferWriter writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(PlayerName);
                writer.WriteValueSafe(PlayerTeam);
                writer.WriteValueSafe(PlayerType);
            }
            else if (serializer.IsReader)
            {
                FastBufferReader reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out PlayerName);
                reader.ReadValueSafe(out PlayerTeam);
                reader.ReadValueSafe(out PlayerType);
            }
        }

        public override readonly string ToString() => $"Player Name: {PlayerName}, Team: {PlayerTeam}, Type: {PlayerType}";
    }
}