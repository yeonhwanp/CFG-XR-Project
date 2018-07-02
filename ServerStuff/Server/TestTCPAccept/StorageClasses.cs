using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

/// <summary>
/// Classes to store the values to be sent over.
/// Server version: Doesn't contain the methods.
/// </summary>

[ProtoInclude(500, typeof(PositionStorage))]
[ProtoContract]
public class PositionList
{
    [ProtoMember(1)]
    public List<PositionStorage> PList = new List<PositionStorage>();
}

[ProtoContract]
public class PositionStorage : PositionList
{
    [ProtoMember(1)]
    public float Rotation;
    [ProtoMember(2)]
    public float Velocity;
}
