namespace FluentDatabase.DummyService.Models
{
    public class MigrationConfig
    {
        public string ConnectionString { get; set; }
        public bool ToRollback { get; set; }
        public long MigrationVersion { get; set; }
    }
}