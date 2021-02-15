using System;
using System.Linq;
using Autofac.Features.AttributeFilters;
using Dapper;
using FluentMigrator.Runner;
using Npgsql;

namespace FluentDatabase.Migrator.MigrationEngine
{
    public class DatabaseMigrationEngine : IDatabaseMigrationEngine
    {
        private readonly long _toVersion;
        private readonly bool _rollback;
        private readonly string _connectionString;

        public DatabaseMigrationEngine(
            long version,
            bool rollback,
            string connectionString)
        {
             _toVersion = version;
            _rollback = rollback;
            _connectionString = connectionString;
        }

        public void Run(IMigrationRunner runner)
        {
            _ensureDatabase();
            if(_rollback)
                runner.RollbackToVersion(_toVersion);
            else
                runner.MigrateUp(_toVersion);
        }

        private void _ensureDatabase()
        {
            var connStr = _connectionString.Replace($"Database={Manager.DB_NAME}", "Database=postgres");
                using var conn = new NpgsqlConnection(connStr);
                var data = conn.Query($"select * from pg_catalog.pg_database where datname = @dbname",
                    new {dbname = Manager.DB_NAME});
                if (!data.Any())
                    conn.Execute($" Create Database {Manager.DB_NAME}");
        }
    } 
}