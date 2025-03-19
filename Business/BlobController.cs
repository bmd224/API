using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MVC.Business
{
    public class BlobController
    {
        private const string BlobConnectionString = "UseDevelopmentStorage=true";
        private const string ContainerName = "unvalidated-container";

        public async Task<string> PushImageToBlob(IFormFile formFile, Guid imageGuid)
        {
            try
            {
                Console.WriteLine($"Début de l'upload de l'image : {formFile.FileName}");

                using (MemoryStream ms = new MemoryStream())
                {
                    // Vérification de la taille du fichier (max 40MB)
                    if (formFile.Length > 40971520)
                    {
                        throw new ExceptionFilesize();
                    }

                    await formFile.CopyToAsync(ms);

                    Console.WriteLine("Création du service client Azure Blob...");
                    // Création du service client Azure Blob avec une version supportée
                    BlobServiceClient serviceClient = new BlobServiceClient(BlobConnectionString, new BlobClientOptions(BlobClientOptions.ServiceVersion.V2021_06_08));



                    Console.WriteLine($"Récupération du conteneur {ContainerName}...");
                    BlobContainerClient blobContainer = serviceClient.GetBlobContainerClient(ContainerName);

                    // Vérifier si le conteneur existe, sinon le créer
                    Console.WriteLine("Vérification de l'existence du conteneur...");
                    try
                    {
                        await blobContainer.CreateIfNotExistsAsync(PublicAccessType.None);
                        Console.WriteLine("Conteneur prêt.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Erreur lors de la création du conteneur : {ex.Message}");
                        throw;
                    }

                    // Réinitialisation du flux avant l’upload
                    ms.Position = 0;

                    // Upload de l’image avec le GUID comme nom de fichier
                    Console.WriteLine($"Envoi de l'image sur Azure Blob avec GUID : {imageGuid}");
                    string fileName = $"{imageGuid}{Path.GetExtension(formFile.FileName)}";
                    BlobClient blobClient = blobContainer.GetBlobClient(fileName);


                    try
                    {
                        await blobClient.UploadAsync(ms, true);
                        Console.WriteLine($"✅ Upload terminé : {blobClient.Uri.AbsoluteUri}");
                        return blobClient.Uri.AbsoluteUri;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Erreur lors de l'upload de l'image : {ex.Message}");
                        throw;
                    }
                }
            }
            catch (ExceptionFilesize)
            {
                Console.WriteLine("❌ Le fichier est trop volumineux (max 40MB).");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur inattendue : {ex.Message}");
                throw;
            }
        }
    }

    // Exception pour fichiers trop volumineux
    public class ExceptionFilesize : Exception
    {
        public ExceptionFilesize() : base("Le fichier est trop volumineux (max 40MB).") { }
    }
}