namespace Solstice
{
    public interface IVehicleComponent
    {
        abstract public void Construct(MessageBus messageBus, VehicleState vehicleState);
    }
}