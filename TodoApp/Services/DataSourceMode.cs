using Npgsql;

namespace TodoApp.Services;

/// <summary>Database = PostgreSQL. Mock = in-memory database seeded with demo data (nothing is saved; for trying the app without PostgreSQL).</summary>
public enum DataSourceMode { Mock, Database }

public static class DataSourceResolver
{
    /// <summary>
    /// Config "DataSource": Database | Mock | Auto ("Json" is accepted as an old alias of Mock).
    /// Production is always Database. Auto uses Database if PostgreSQL is reachable, otherwise Mock.
    /// </summary>
    public static async Task<DataSourceMode> ResolveAsync(
        IConfiguration config, IHostEnvironment env, string connectionString, ILogger logger)
    {
        var requested = config["DataSource"] ?? "Auto";

        if (env.IsProduction())
        {
            if (!requested.Equals("Database", StringComparison.OrdinalIgnoreCase))
                logger.LogWarning("DataSource '{Requested}' ignored in Production; using Database.", requested);
            return DataSourceMode.Database;
        }

        if (requested is "Mock" or "Json" or "mock" or "json") return DataSourceMode.Mock;
        if (requested.Equals("Database", StringComparison.OrdinalIgnoreCase)) return DataSourceMode.Database;

        if (await CanConnectAsync(connectionString, logger))
        {
            logger.LogInformation("DataSource=Auto: database reachable, using Database.");
            return DataSourceMode.Database;
        }

        logger.LogWarning("DataSource=Auto: database unreachable, using the in-memory mock database.");
        return DataSourceMode.Mock;
    }

    private static async Task<bool> CanConnectAsync(string connectionString, ILogger logger)
    {
        try
        {
            var csb = new NpgsqlConnectionStringBuilder(connectionString) { Timeout = 3, Pooling = false };
            await using var conn = new NpgsqlConnection(csb.ConnectionString);
            await conn.OpenAsync();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Database connection check failed.");
            return false;
        }
    }
}
