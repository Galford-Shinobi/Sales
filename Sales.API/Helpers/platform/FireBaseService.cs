using Firebase.Auth;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;

namespace Sales.API.Helpers.platform
{
    public class FireBaseService : IFireBaseService
    {
        private readonly IConfiguration _configuration;

        public FireBaseService(IConfiguration configuration)
        {
           _configuration = configuration;
        }

        public async Task<bool> EliminarStorageAsync(string CarpetaDestino, string NombreArchivo)
        {
            try
            {
                var api_key = _configuration["Configuracion:FireBase_StorageApi_key"];
                var email = _configuration["Configuracion:FireBase_StorageEmail"]!;
                var clave = _configuration["Configuracion:FireBase_StorageClave"]!;
                var ruta = _configuration["Configuracion:FireBase_StorageRuta"];


                var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
                var a = await auth.SignInWithEmailAndPasswordAsync(email, clave);

                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    ruta,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(CarpetaDestino)
                    .Child(NombreArchivo)
                    .DeleteAsync();

                await task;

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> SubirStorageAsync(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo)
        {
            string UrlImagen = "";
            try
            {
                var api_key = _configuration["Configuracion:FireBase_StorageApi_key"];
                string email = _configuration["Configuracion:FireBase_StorageEmail"]!;
                string clave = _configuration["Configuracion:FireBase_StorageClave"]!;
                var ruta = _configuration["Configuracion:FireBase_StorageRuta"];

                var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));

                var a = await auth.SignInWithEmailAndPasswordAsync(email, clave);
               
                
                var cancellation = new CancellationTokenSource();

                var task = new FirebaseStorage(
                    ruta,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                        ThrowOnCancel = true
                    })
                    .Child(CarpetaDestino)
                    .Child(NombreArchivo)
                    .PutAsync(StreamArchivo, cancellation.Token);

                UrlImagen = await task;
            }
            catch
            {
                UrlImagen = "";
            }

            return UrlImagen;
        }

        public StreamContent ConvertBase64ToStream(string fileBase64)
        {
            byte[] stringToBase64 = Convert.FromBase64String(fileBase64);
            StreamContent streamContent = new(new MemoryStream(stringToBase64));
            return streamContent;
        }
    }
}
