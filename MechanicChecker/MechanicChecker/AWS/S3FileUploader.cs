using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker.AWS
{
    public class S3FileUploader
    {
        private int filenameLength = 50;
        private string awsS3BucketUrlBase = "https://s3.amazonaws.com/mechanic.checker/";
        public string value(IFormFile file, string folder)
        {
            string awsS3BucketUrl = awsS3BucketUrlBase + folder + "/";

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[filenameLength];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars) + Path.GetExtension(file.FileName);

            AmazonS3Uploader amazonS3 = new AmazonS3Uploader();
            amazonS3.UploadFile(finalString, file, folder);
            string awsS3ImageUrl = awsS3BucketUrl + finalString;

            return awsS3ImageUrl;

        }
       
    }
}
