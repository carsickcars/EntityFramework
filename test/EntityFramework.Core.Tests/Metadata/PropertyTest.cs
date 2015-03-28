// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Metadata;
using Xunit;

namespace Microsoft.Data.Entity.Tests.Metadata
{
    public class PropertyTest
    {
        [Fact]
        public void Can_create_property_from_property_info()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)));

            Assert.Equal("Name", property.Name);
            Assert.Same(typeof(string), property.ClrType);
        }

        [Fact]
        public void Default_nullability_of_property_is_based_on_nullability_of_CLR_type()
        {
            var stringProperty = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)));
            var nullableIntProperty = new Property("Name", typeof(int?), new Model().AddEntityType(typeof(object)));
            var intProperty = new Property("Name", typeof(int), new Model().AddEntityType(typeof(object)));

            Assert.Null(stringProperty.IsNullable);
            Assert.True(((IProperty)stringProperty).IsNullable);
            Assert.Null(stringProperty.IsNullable);
            Assert.True(((IProperty)nullableIntProperty).IsNullable);
            Assert.Null(intProperty.IsNullable);
            Assert.False(((IProperty)intProperty).IsNullable);
        }

        [Fact]
        public void Property_nullability_can_be_mutated()
        {
            var stringProperty = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)));
            var intProperty = new Property("Name", typeof(int), new Model().AddEntityType(typeof(object)));

            stringProperty.IsNullable = false;
            Assert.False(stringProperty.IsNullable.Value);
            Assert.Null(intProperty.IsNullable);

            stringProperty.IsNullable = true;
            intProperty.IsNullable = false;
            Assert.True(stringProperty.IsNullable.Value);
            Assert.False(intProperty.IsNullable.Value);

            stringProperty.IsNullable = null;
            intProperty.IsNullable = null;
            Assert.Null(stringProperty.IsNullable);
            Assert.Null(intProperty.IsNullable);
        }

        [Fact]
        public void Properties_with_non_nullable_types_cannot_be_made_nullable()
        {
            var intProperty = new Property("Name", typeof(int), new Model().AddEntityType(typeof(object)));

            Assert.Equal(
                Strings.CannotBeNullable("Name", "Object", "Int32"),
                Assert.Throws<InvalidOperationException>(() => intProperty.IsNullable = true).Message);
        }

        [Fact]
        public void UnderlyingType_returns_correct_underlying_type()
        {
            Assert.Equal(typeof(int), new Property("Name", typeof(int?), new Model().AddEntityType(typeof(object))).ClrType.UnwrapNullableType());
            Assert.Equal(typeof(int), new Property("Name", typeof(int), new Model().AddEntityType(typeof(object))).ClrType.UnwrapNullableType());
        }

        [Fact]
        public void HasClrProperty_is_set_appropriately()
        {
            Assert.False(new Property("Kake", typeof(int), new Model().AddEntityType(typeof(object))).IsShadowProperty);
            Assert.False(new Property("Kake", typeof(int), new Model().AddEntityType(typeof(object))).IsShadowProperty);
            Assert.True(new Property("Kake", typeof(int), new Model().AddEntityType(typeof(object)), shadowProperty: true).IsShadowProperty);
        }

        [Fact]
        public void Property_does_not_use_StoreGeneratedPattern_by_default()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)));

            Assert.Null(property.StoreGeneratedPattern);
            Assert.Equal(StoreGeneratedPattern.None, ((IProperty)property).StoreGeneratedPattern);
        }

        [Fact]
        public void Can_mark_property_as_using_StoreGeneratedPattern()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(Entity)));

            property.StoreGeneratedPattern = StoreGeneratedPattern.Computed;
            Assert.Equal(StoreGeneratedPattern.Computed, property.StoreGeneratedPattern.Value);

            property.StoreGeneratedPattern = StoreGeneratedPattern.None;
            Assert.Equal(StoreGeneratedPattern.None, property.StoreGeneratedPattern.Value);

            property.StoreGeneratedPattern = null;
            Assert.Null(property.StoreGeneratedPattern);
        }

        [Fact]
        public void Property_is_not_concurrency_token_by_default()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)));

            Assert.Null(property.IsConcurrencyToken);
            Assert.False(((IProperty)property).IsConcurrencyToken);
        }

        [Fact]
        public void Can_mark_property_as_concurrency_token()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(Entity)));

            property.IsConcurrencyToken = true;
            Assert.True(property.IsConcurrencyToken.Value);

            property.IsConcurrencyToken = false;
            Assert.False(property.IsConcurrencyToken.Value);

            property.IsConcurrencyToken = null;
            Assert.Null(property.IsConcurrencyToken);
        }

        [Fact]
        public void Property_is_read_write_by_default()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)));

            Assert.Null(property.IsReadOnly);
            Assert.False(((IProperty)property).IsReadOnly);
        }

        [Fact]
        public void Property_can_be_marked_as_read_only()
        {
            var property 
                = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)))
                {
                    IsReadOnly = true
                };

            Assert.True(property.IsReadOnly.Value);

            property.IsReadOnly = false;
            Assert.False(property.IsReadOnly.Value);

            property.IsReadOnly = null;
            Assert.Null(property.IsReadOnly);
        }

        [Fact]
        public void Can_get_and_set_property_index_for_normal_property()
        {
            var property = new Property("Kake", typeof(int), new Model().AddEntityType(typeof(object)));

            Assert.Equal(0, property.Index);
            Assert.Equal(-1, property.GetShadowIndex());

            property.Index = 1;

            Assert.Equal(1, property.Index);
            Assert.Equal(-1, property.GetShadowIndex());

            Assert.Equal(
                "value",
                Assert.Throws<ArgumentOutOfRangeException>(() => property.Index = -1).ParamName);

            Assert.Equal(
                "index",
                Assert.Throws<ArgumentOutOfRangeException>(() => property.SetShadowIndex(-1)).ParamName);

            Assert.Equal(
                "index",
                Assert.Throws<ArgumentOutOfRangeException>(() => property.SetShadowIndex(1)).ParamName);
        }

        [Fact]
        public void Can_get_and_set_property_and_shadow_index_for_shadow_property()
        {
            var property = new Property("Kake", typeof(int), new Model().AddEntityType(typeof(object)), shadowProperty: true);

            Assert.Equal(0, property.Index);
            Assert.Equal(0, property.GetShadowIndex());

            property.Index = 1;
            property.SetShadowIndex(2);

            Assert.Equal(1, property.Index);
            Assert.Equal(2, property.GetShadowIndex());

            Assert.Equal(
                "value",
                Assert.Throws<ArgumentOutOfRangeException>(() => property.Index = -1).ParamName);

            Assert.Equal(
                "index",
                Assert.Throws<ArgumentOutOfRangeException>(() => property.SetShadowIndex(-1)).ParamName);
        }

        [Fact]
        public void Nullable_property_has_null_sentinel_by_default()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object)));

            Assert.Null(property.SentinelValue);
            Assert.Null(((IProperty)property).SentinelValue);
        }

        [Fact]
        public void Non_nullable_property_has_CLR_default_sentinel_by_default()
        {
            var property = new Property("Name", typeof(int), new Model().AddEntityType(typeof(object)));

            Assert.Null(property.SentinelValue);
            Assert.Equal(0, ((IProperty)property).SentinelValue);
        }

        [Fact]
        public void Can_set_sentinel_for_nullable_property()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object))) { SentinelValue = "Void" };

            Assert.Equal("Void", property.SentinelValue);
            Assert.Equal("Void", ((IProperty)property).SentinelValue);
        }

        [Fact]
        public void Can_set_sentinel_for_non_nullable_property()
        {
            var property = new Property("Name", typeof(int), new Model().AddEntityType(typeof(object))) { SentinelValue = -1 };

            Assert.Equal(-1, property.SentinelValue);
            Assert.Equal(-1, ((IProperty)property).SentinelValue);
        }

        [Fact]
        public void IsSentinelValue_on_nullable_propertyalways_returns_true_for_null_or_sentinel()
        {
            var property = new Property("Name", typeof(string), new Model().AddEntityType(typeof(object))) { SentinelValue = "Void" };

            Assert.True(property.IsSentinelValue(null));
            Assert.True(property.IsSentinelValue("Void"));
            Assert.False(property.IsSentinelValue("Ether"));
        }

        [Fact]
        public void IsSentinelValue_on_non_nullable_propertyalways_returns_true_for_null_or_sentinel()
        {
            var property = new Property("Name", typeof(int), new Model().AddEntityType(typeof(object))) { SentinelValue = -1 };

            Assert.True(property.IsSentinelValue(null));
            Assert.True(property.IsSentinelValue(-1));
            Assert.False(property.IsSentinelValue(0));
        }

        private class Entity
        {
            public string Name { get; set; }
        }
    }
}
