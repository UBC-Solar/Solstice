namespace Solstice.Messages
{
    // MechBrakeValue is the mech brake pedal position from 0-100 percent.
    public record MechBrakeMessage(float MechBrakeValue) : IMessage;
}

