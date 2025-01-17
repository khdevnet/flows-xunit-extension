﻿using System;

namespace Xunit.Scenario.Examples.Collections
{
    [CollectionDefinition(nameof(BeverlyHillsShop))]
    public class BeverlyHillsShopCollection : ICollectionFixture<BeverlyHillsShop>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class BeverlyHillsShop : IDisposable
    {
        public string ShopId { get; set; } = "MyShop";
        public BeverlyHillsShop()
        {
            // create test shop
            // ... initialize data in the test database ...
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }

    }
}
