using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;

namespace DirectoryService.Domain.LocationsContext
{
    public class Location
    {
        public LocationId Id { get; }
        public LocationName Name { get; private set; }
        public LocationAddress Address { get; private set; }
        public IanaTimeZone TimeZone { get; private set; }
        public EntityLifeTime LifeTime { get; private set; }

        public Location(
            LocationId id,
            LocationAddress address,
            LocationName name,
            IanaTimeZone timeZone,
            EntityLifeTime lifeTime
        )
        {
            Id = id;
            Address = address;
            Name = name;
            TimeZone = timeZone;
            LifeTime = lifeTime;
        }

        public void ChangeTimeZone(IanaTimeZone newTimeZone) // переименовал
        {
            if (LifeTime == null)
            {
                throw new InvalidOperationException("LifeTime не инициализирован");
            }

            if (!LifeTime.IsActive)
            {
                throw new InvalidOperationException("Локация не активна");
            }

            TimeZone = newTimeZone;
            LifeTime = LifeTime.Update();
        }

        public void ChangeAddress(LocationAddress newAddress) // переименовал с ChrgeAddress
        {
            if (LifeTime == null)
            {
                throw new InvalidOperationException("LifeTime не инициализирован");
            }

            if (!LifeTime.IsActive)
            {
                throw new InvalidOperationException("Локация не активна");
            }

            Address = newAddress;
            LifeTime = LifeTime.Update();
        }

        public void ChangeName(LocationName newName) // заменил ChrgeName и ChangeName
        {
            if (LifeTime == null)
            {
                throw new InvalidOperationException("LifeTime не инициализирован");
            }

            if (!LifeTime.IsActive)
            {
                throw new InvalidOperationException("Локация не активна");
            }

            Name = newName;
            LifeTime = LifeTime.Update();
        }
    }
}