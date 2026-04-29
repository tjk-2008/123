namespace DirectoryService.Infrastructure.Options
{
	public class DatabaseOptions
	{
		public required string Host { get; set; }
		public required int Port { get; set; }
		public required string Database { get; set; }
		public required string Username { get; set; }
		public required string Password { get; set; }

		public string GetConnectionString()
		{
			if (string.IsNullOrWhiteSpace(Host))
			{
				throw new InvalidOperationException("DatabaseOptions: Host не может быть пустым");
			}

			if (Port <= 0 || Port > 65535)
			{
				throw new InvalidOperationException(
					$"DatabaseOptions: Port {Port} вне допустимого диапазона (1-65535)"
				);
			}

			if (string.IsNullOrWhiteSpace(Database))
			{
				throw new InvalidOperationException("DatabaseOptions: Database не может быть пустым");
			}

			if (string.IsNullOrWhiteSpace(Username))
			{
				throw new InvalidOperationException("DatabaseOptions: Username не может быть пустым");
			}

			if (string.IsNullOrWhiteSpace(Password))
			{
				throw new InvalidOperationException("DatabaseOptions: Password не может быть пустым");
			}

			return $"Host={Host};Port={Port};Database={Database};Username={Username};Password={Password}";
		}
	}
}
