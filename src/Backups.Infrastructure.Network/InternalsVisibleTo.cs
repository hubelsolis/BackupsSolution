using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Backups.Network.Tests")]

// Requerido por NSubstitute (Castle DynamicProxy) para poder generar mocks de las
// interfaces internas ITransmisor / IClienteTransmision desde los tests.
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
