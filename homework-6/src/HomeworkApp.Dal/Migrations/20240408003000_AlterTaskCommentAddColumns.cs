using FluentMigrator;

namespace Route256.Week5.Workshop.PriceCalculator.Dal.Migrations;

[Migration(20240408003000, TransactionBehavior.None)]
public class AlterTaskCommentAddColumns : Migration
{
    private const string table = "task_comments";
    
    public override void Up()
    {
        if (!Schema.Table(table).Column("modified_at").Exists())
        {
            Alter.Table(table)
                .AddColumn("modified_at")
                .AsDateTimeOffset()
                .Nullable();
        }

        if (!Schema.Table(table).Column("deleted_at").Exists())
        {
            Alter.Table(table)
                .AddColumn("deleted_at")
                .AsDateTimeOffset()
                .Nullable();
        }
    }

    public override void Down()
    {
        if (Schema.Table(table).Column("modified_at").Exists())
        {
            Delete.Column("modified_at").FromTable(table);
        }

        if (Schema.Table(table).Column("deleted_at").Exists())
        {
            Delete.Column("deleted_at").FromTable(table);
        }
    }
}