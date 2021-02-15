using FluentMigrator.Runner;

namespace FluentDatabase.Migrator.MigrationEngine
{
    public interface IDatabaseMigrationEngine
    {
        void Run(IMigrationRunner runner);
    }
}