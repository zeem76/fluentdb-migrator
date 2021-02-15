namespace FluentDatabase.Migrator.MigrationEngine
{
    public interface IDatabaseMigrator
    {
        void Up();
        void Down();
    }
}