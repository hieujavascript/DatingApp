using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace API.Services
{

    public class PhotoService : IPhotoService
    {
        private readonly  Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            Account account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret);
                 this._cloudinary = new Cloudinary(account);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file.Length > 0 ){
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams{
                    // địnhnghĩa file của chúng ta
                    File = new FileDescription(file.FileName , stream) , 
                    Transformation = new Transformation()
                                    .Height(500)
                                    .Width(500)
                                    .Crop("fill")
                                    .Gravity("face") 

                };
                uploadResult = await this._cloudinary.UploadAsync(uploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await this._cloudinary.DestroyAsync(deleteParams);
            return result;
            //throw new System.NotImplementedException();
        }
    }
}
