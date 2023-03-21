namespace Sales.API.Helpers
{
    public interface IFireBaseService
    {
        Task<string> SubirStorageAsync(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo);
        Task<bool> EliminarStorageAsync(string CarpetaDestino, string NombreArchivo);
        public  StreamContent ConvertBase64ToStream(string fileBase64);

    }
}
