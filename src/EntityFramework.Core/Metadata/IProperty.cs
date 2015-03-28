// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Data.Entity.Metadata
{
    public interface IProperty : IPropertyBase
    {
        Type ClrType { get; }
        bool IsNullable { get; }
        bool IsReadOnly { get; }
        StoreGeneratedPattern StoreGeneratedPattern { get; }
        bool IsValueGeneratedOnAdd { get; }
        int Index { get; }
        bool IsShadowProperty { get; }
        bool IsConcurrencyToken { get; }
        object SentinelValue { get; }
    }
}
