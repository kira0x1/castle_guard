namespace Kira;

using System;

[Flags]
public enum StructureFlags
{
    Repairable = 0x1,
    BlockVision = 0x2,
    KeyStructure = 0x3
}

public class Structure : Component
{
}