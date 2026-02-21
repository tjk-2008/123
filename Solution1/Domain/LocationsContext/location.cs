using DirectoryService.Domain.LocationsContext.ValueObjects;
using DirectoryService.Domain.Shared;
using System.Net;

namespace DirectoryService.Domain.LocationsContext
{
    public class Location
    {
        public LocationId Id { get;  }
        public LocationName Name { get; set; }
        public LocationAddress Address { get; set; }
        public IanaTimeZone TimeZone { get; set; }
        public EntityLifeTime LifeTime { get; set; }

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

        public void ChangeIanaTimeZone(IanaTimeZone newname)
        {
            if (!LifeTime.IsActive)
            {
                throw new InvalidOperationException("Сущность удалена");
            }

            TimeZone = newname;
            LifeTime = LifeTime.Update();
        }
        public void ChrgeAddress(LocationAddress newname)
        {
            if (!LifeTime.IsActive)
            {
                throw new InvalidOperationException("Локация не активна");
            }

            Address = newname;
            LifeTime = LifeTime.Update();
        }
        public void ChrgeName(LocationName newname)
        {
            if (!LifeTime.IsActive)
            {
                throw new InvalidOperationException(" Название не активна ");
            }

            Name = newname;
            LifeTime = LifeTime.Update();
        }
    }
}
