using System.Text.Json;
using Backups.Core.Domain;

namespace Backups.Domain.Tests;

public class EnumSerializacionTests
{
    [Theory]
    [InlineData(EstadoRespaldo.OMITIDO_SIN_CAMBIOS, "OMITIDO_SIN_CAMBIOS")]
    [InlineData(EstadoRespaldo.PROCESANDO, "PROCESANDO")]
    [InlineData(EstadoRespaldo.COMPRIMIENDO_VOLUMENES, "COMPRIMIENDO_VOLUMENES")]
    [InlineData(EstadoRespaldo.ENVIANDO, "ENVIANDO")]
    [InlineData(EstadoRespaldo.COMPLETADO, "COMPLETADO")]
    [InlineData(EstadoRespaldo.FALLIDO, "FALLIDO")]
    public void EstadoRespaldo_Serializa_AlLiteralExactoDelContrato(EstadoRespaldo estado, string literalEsperado)
    {
        Assert.Equal($"\"{literalEsperado}\"", JsonSerializer.Serialize(estado));
    }

    [Theory]
    [InlineData(TipoDisparo.TIMER, "TIMER")]
    [InlineData(TipoDisparo.BOTON_MANUAL, "BOTON_MANUAL")]
    public void TipoDisparo_Serializa_AlLiteralExactoDelContrato(TipoDisparo tipo, string literalEsperado)
    {
        Assert.Equal($"\"{literalEsperado}\"", JsonSerializer.Serialize(tipo));
    }

    [Theory]
    [InlineData(AlgoritmoCompresion.ZIP, "ZIP")]
    [InlineData(AlgoritmoCompresion.RAR, "RAR")]
    [InlineData(AlgoritmoCompresion.LZMA, "LZMA")]
    public void AlgoritmoCompresion_Serializa_AlLiteralExactoDelContrato(AlgoritmoCompresion algoritmo, string literalEsperado)
    {
        Assert.Equal($"\"{literalEsperado}\"", JsonSerializer.Serialize(algoritmo));
    }

    [Theory]
    [InlineData(Protocolo.FTP, "FTP")]
    [InlineData(Protocolo.SFTP, "SFTP")]
    [InlineData(Protocolo.SSH, "SSH")]
    public void Protocolo_Serializa_AlLiteralExactoDelContrato(Protocolo protocolo, string literalEsperado)
    {
        Assert.Equal($"\"{literalEsperado}\"", JsonSerializer.Serialize(protocolo));
    }
}
