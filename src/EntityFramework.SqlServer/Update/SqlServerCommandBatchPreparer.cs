// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.SqlServer.Update
{
    public class SqlServerCommandBatchPreparer : CommandBatchPreparer
    {
        public SqlServerCommandBatchPreparer(
            [NotNull] SqlServerModificationCommandBatchFactory modificationCommandBatchFactory,
            [NotNull] ParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] ModificationCommandComparer modificationCommandComparer,
            [NotNull] IBoxedValueReaderSource boxedValueReaderSource)
            : base(modificationCommandBatchFactory, parameterNameGeneratorFactory, modificationCommandComparer, boxedValueReaderSource)
        {
        }

        public override IRelationalPropertyExtensions GetPropertyExtensions(IProperty property)
        {
            Check.NotNull(property, nameof(property));

            return property.SqlServer();
        }

        public override IRelationalEntityTypeExtensions GetEntityTypeExtensions(IEntityType entityType)
        {
            Check.NotNull(entityType, nameof(entityType));

            return entityType.SqlServer();
        }
    }
}
