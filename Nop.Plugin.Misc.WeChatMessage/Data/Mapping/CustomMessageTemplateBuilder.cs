using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.WeChatMessage.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.WeChatMessage.Data.Mapping;

public partial class CustomMessageTemplateBuilder : NopEntityBuilder<CustomMessageTemplate>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table) => table
        .WithColumn(nameof(CustomMessageTemplate.Name)).AsString(255).NotNullable()
        .WithColumn(nameof(CustomMessageTemplate.SystemName)).AsString(127).Nullable()
        .WithColumn(nameof(CustomMessageTemplate.Subject)).AsString(255).Nullable();

    #endregion
}
