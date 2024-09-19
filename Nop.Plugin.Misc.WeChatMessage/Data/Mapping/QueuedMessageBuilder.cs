using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Messages;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.WeChatMessage.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Data.Mapping;
public partial class QueuedMessageBuilder : NopEntityBuilder<QueuedMessage>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(QueuedMessage.Recipient)).AsString(255).NotNullable()
            .WithColumn(nameof(QueuedMessage.ToName)).AsString(255).Nullable()
            .WithColumn(nameof(QueuedMessage.Subject)).AsString(255).Nullable()
            .WithColumn(nameof(QueuedMessage.TemplateCode)).AsString(255).Nullable()
            .WithColumn(nameof(QueuedMessage.TemplateParamJson)).AsString(255).Nullable()
            .WithColumn(nameof(QueuedMessage.Body)).AsString(255).Nullable();
             
            //don't create an ForeignKey for the EmailAccount table, because this field may by zero
            // .WithColumn(nameof(QueuedMessage.EmailAccountId)).AsInt32();
    }

    #endregion
}


