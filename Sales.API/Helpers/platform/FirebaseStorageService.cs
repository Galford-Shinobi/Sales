using Firebase.Auth;
using Firebase.Storage;
using Sales.Shared.ViewsModels;

namespace Sales.API.Helpers.platform
{
    public static class FirebaseStorageService
    {
        private static readonly string _apiKey = Environment.GetEnvironmentVariable("API_KEY_STORAGE");
        private static readonly string _bucket = Environment.GetEnvironmentVariable("BUCKET_STORAGE");
        private static readonly string _email = Environment.GetEnvironmentVariable("AUTH_EMAIL");
        private static readonly string _Password = Environment.GetEnvironmentVariable("AUTH_PASWORD");

        public static StreamContent ConvertBase64ToStream(string fileBase64)
        {
            byte[] stringToBase64 = Convert.FromBase64String(fileBase64);
            StreamContent streamContent = new(new MemoryStream(stringToBase64));
            return streamContent;
        }

        public static async Task<string> UploadFile(Stream stream, FileModel file)
        {
            string fileFromFirebaseStorage = string.Empty;
            FirebaseAuthProvider firebaseConfiguration = new(new FirebaseConfig(_apiKey));

            FirebaseAuthLink authConfiguration = await firebaseConfiguration
                .SignInWithEmailAndPasswordAsync(_email, _Password);

            CancellationTokenSource cancellationToken = new();

            FirebaseStorageTask storageManager = new FirebaseStorage(_bucket,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(authConfiguration.FirebaseToken),
                    ThrowOnCancel = true
                })
                .Child(file.FolderName)
                .Child(file.FileName)
                .PutAsync(stream, cancellationToken.Token);

            try
            {
                fileFromFirebaseStorage = await storageManager;
            }
            catch (Exception e)
            {
            }
            return fileFromFirebaseStorage;
        }
    }
}
