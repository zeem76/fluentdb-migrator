using System;
using FluentMigrator;
using Microsoft.Extensions.Logging;

namespace FluentDatabase.Migrator.MigrationEngine
{
    [Migration(202009201527)]
    public class Migration_202009201527 : Migration, IDatabaseMigrator
    {
        private readonly ILogger<Migration_202009201527> _logger;

        public Migration_202009201527(ILogger<Migration_202009201527> logger)
        {
            _logger = logger;
        }

        public override void Up()
        {
            try
            {
                //Create in dependent order , meaning tables that rely on other tables make them first
                //Based off of naming conventions here: https://martendb.io/documentation/postgres/naming/
                
                Create.Table("customers")
                    .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("first_name").AsString(50).NotNullable();
                
                Create.Table("payments")
                    .WithColumn("id").AsInt64().NotNullable().PrimaryKey().Identity()
                    .WithColumn("customer_id").AsInt64().ForeignKey("customers", "id").NotNullable()
                    .WithColumn("staff_id").AsInt64().NotNullable()
                    .WithColumn("store_id").AsInt64().NotNullable()
                    .WithColumn("status").AsByte().NotNullable();
                
            }
            catch (Exception e)
            {
                _logger.LogError($"While trying to process migration up command that following ex occurred" +
                                 $"ex=[{e}]");
            }
        }

        public override void Down()
        {
            try
            {
                Delete.Table("customers");
                Delete.Table("payments");
            }
            catch (Exception e)
            {
                _logger.LogError($"While trying to process migration down command that following ex occurred" +
                                 $"ex=[{e}]");
            }
        }
    }
}