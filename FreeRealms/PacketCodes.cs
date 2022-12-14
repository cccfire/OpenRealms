namespace FreeRealms
{
    public static class BasePackets
    {
        public const ushort PacketSendSelfToClient = 12;

        public const ushort BasePlayerUpdatePacket = 35;
        public const ushort PacketClientBeginZoning = 31;

        public static class BasePlayerUpdatePackets
        {
            public const ushort PlayerUpdatePacketAddPc = 1;
            public const ushort PlayerUpdatePacketItemDefinitions = 37;
        }

        public const ushort BaseEncounterPacket = 41;
        public const ushort PacketSendZoneDetails = 43;

        public static class BaseEncounterPackets
        {
            public const ushort EncounterOverworldCombatPacket = 132;
        }

        public const ushort BaseAchievementPacket = 150;

        public static class BaseAchievementPackets
        {
            public const ushort AchievementObjectiveActivatedPacket = 6;
        }

        public const ushort PacketClientGameSettings = 144;

        public const ushort PacketInitializationParameters = 166;

        public const ushort AnnouncementBasePacket = 193;

        public static class AnnouncementBasePackets
        {
            public const byte AnnouncementDataSendPacket = 2;
        }
    }

    public static class ClientGatewayBasePackets
    {
        public const ushort PacketLogin = 1;
        public const ushort PacketLoginReply = 2;
        public const ushort PacketLogout = 3;
        public const ushort PacketServerForcedLogout = 4;
        public const ushort PacketTunneledClientPacket = 5;
        public const ushort PacketTunneledClientWorldPacket = 6;
        public const ushort PacketClientIsHosted = 7;
        public const ushort PacketOnlineStatusRequest = 8;
        public const ushort PacketOnlineStatusReply = 9;
        public const ushort PacketTunneledClientGatewayPacket = 10;
    }

    public static class ClientServerCoreBasePackets
    {
    }

    public static class RemoteAssetDeliveryBasePackets
    {
    }
}