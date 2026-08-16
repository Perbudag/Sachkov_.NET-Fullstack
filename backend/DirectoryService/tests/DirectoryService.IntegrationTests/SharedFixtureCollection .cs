using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.IntegrationTests;

[CollectionDefinition("Shared Fixture Collection")]
#pragma warning disable CA1711 // Идентификаторы не должны иметь неправильных суффиксов
public class SharedFixtureCollection : ICollectionFixture<DirectoryTestWebFactory> { }
#pragma warning restore CA1711 // Идентификаторы не должны иметь неправильных суффиксов
