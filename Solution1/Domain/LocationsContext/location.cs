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

		private Location()
		{
			Id = null!;
			Name = null!;
			Address = null!;
			TimeZone = null!;
			LifeTime = null!;
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

		public void ChangeActivity(bool v)
		{
			throw new NotImplementedException();
		}

		public void Update(LocationName? newName, LocationAddress? newAddress, IanaTimeZone? newTimeZone)
		{
			if (LifeTime == null)
			{
				throw new InvalidOperationException("LifeTime не инициализирован");
			}

			if (!LifeTime.IsActive)
			{
				throw new InvalidOperationException("Нельзя обновить архивированную локацию");
			}

			if (newName is null && newAddress is null && newTimeZone is null)
			{
				throw new InvalidOperationException("Нет данных для обновления");
			}

			if (newName is not null)
			{
				ChangeName(newName);
			}

			if (newAddress is not null)
			{
				ChangeAddress(newAddress);
			}

			if (newTimeZone is not null)
			{
				ChangeTimeZone(newTimeZone);
			}
		}
	}
}
