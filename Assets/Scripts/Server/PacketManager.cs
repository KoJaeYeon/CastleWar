using System.Runtime.InteropServices;
using Unity.VisualScripting;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
struct SpawnPacket
{
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfService;
    [MarshalAs(UnmanagedType.I4)]
    public int playerId;
    [MarshalAs(UnmanagedType.I4)]
    public int payloadLength;
    [MarshalAs(UnmanagedType.I4)]
    public int unitSlot;
    [MarshalAs(UnmanagedType.I4)]
    public int x;
    [MarshalAs(UnmanagedType.I4)]
    public int z;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
struct AddSlotPacket
{
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfService;
    [MarshalAs(UnmanagedType.I4)]
    public int playerId;
    [MarshalAs(UnmanagedType.I4)]
    public int payloadLength;
    [MarshalAs(UnmanagedType.I4)]
    public int unitId;
    [MarshalAs(UnmanagedType.I4)]
    public int slotIndex;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
struct CommandPacket
{
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfService;
    [MarshalAs(UnmanagedType.I4)]
    public int playerId;
    [MarshalAs(UnmanagedType.I4)]
    public int payloadLength;
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfCommand; // Return, Cancel, TierUp
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
struct AccountPacket
{
    [MarshalAs(UnmanagedType.I4)]
    public int typeOfService;
    [MarshalAs(UnmanagedType.I4)]
    public int playerId;
    [MarshalAs(UnmanagedType.I4)]
    public int payloadLength;
    [MarshalAs(UnmanagedType.I4)]
    public int newPlayerId;
}

public class PacketManager
{
    // Packet to send
    private SpawnPacket spawnPacket = new SpawnPacket();
    private AddSlotPacket addSlotPacket = new AddSlotPacket();
    private CommandPacket commandPacket = new CommandPacket();
    private AccountPacket accountPacket = new AccountPacket();

    public PacketManager()
    {
        
    }

    public byte[] GetSpawnPacket(int unitSlot,int playerId, int x, int z)
    {
        spawnPacket.typeOfService = 0;
        spawnPacket.playerId = playerId;
        spawnPacket.payloadLength = 12;
        spawnPacket.unitSlot = unitSlot;
        spawnPacket.x = x;
        spawnPacket.z = z;

        return Serialize<SpawnPacket>(spawnPacket);
    }

    public byte[] GetAddSlotPacket(int unitId,int playerId, int slotIndex)
    {
        addSlotPacket.typeOfService = 1;
        addSlotPacket.playerId = playerId;
        addSlotPacket.payloadLength = 8;
        addSlotPacket.unitId = unitId;
        addSlotPacket.slotIndex = slotIndex;

        return Serialize<AddSlotPacket>(addSlotPacket);
    }

    public byte[] GetCommandPacket(int playerId,int typeOfCommand)
    {
        commandPacket.typeOfService = 2;
        commandPacket.playerId = playerId;
        commandPacket.payloadLength = 4;
        commandPacket.typeOfCommand = typeOfCommand;

        return Serialize<CommandPacket>(commandPacket);
    }

    public byte[] GetAccountPacket()
    {
        accountPacket.typeOfService = 3;
        accountPacket.playerId = 0;
        accountPacket.payloadLength = 4;
        accountPacket.newPlayerId = 0;

        return Serialize<AccountPacket>(accountPacket);
    }

    // Calling this method will return a byte array with the contents
    // of the struct ready to be sent via the tcp socket.
    private byte[] Serialize<T>(T packet)
    {
        // allocate a byte array for the struct data
        var buffer = new byte[Marshal.SizeOf(typeof(T))];

        // Allocate a GCHandle and get the array pointer
        var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var pBuffer = gch.AddrOfPinnedObject();

        // copy data from struct to array and unpin the gc pointer
        Marshal.StructureToPtr(packet, pBuffer, false);
        gch.Free();

        return buffer;
    }
}
