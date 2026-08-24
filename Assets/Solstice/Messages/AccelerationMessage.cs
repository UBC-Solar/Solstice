namespace Solstice.Messages
{
    // AccelerationValue is the accelerator pedal position as a 0-100 percent, not a true acceleration.
    public record AccelerationMessage(float AccelerationValue) : IMessage;
}

