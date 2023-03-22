namespace Sales.API.Helpers
{
    public interface IFireBaseService
    {
        Task<string> SubirStorageAsync(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo);
        Task<bool> EliminarStorageAsync(string CarpetaDestino, string NombreArchivo);
        StreamContent ConvertBase64ToStream(string fileBase64);

        async Task<string> EditFileAsync(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            if (NombreArchivo is not null)
            {
                await EliminarStorageAsync(CarpetaDestino, NombreArchivo);
            }

            return await SubirStorageAsync(StreamArchivo, CarpetaDestino, NombreArchivo!);
        }
    }
}
