using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.WeChatMessage.Domain;

namespace Nop.Plugin.Misc.WeChatMessage.Data;

[NopMigration("2024/08/26 12:00:00", "Misc.WeChatMessage base schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    #region Methods

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<CustomMessageTemplate>();

        Create.TableFor<QueuedMessage>();
    }

    #endregion
}