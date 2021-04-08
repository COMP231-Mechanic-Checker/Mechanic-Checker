using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace MechanicChecker
{
    public class AmazonS3Uploader
    {
        public static string folder = "seller";
        private string bucketName = "mechanic.checker";

        public void UploadFile(string filename, IFormFile readStream)
        {
            string keyName = string.Format("{0}/{1}", folder, filename);
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            var transferUtility = new TransferUtility(client);

            try
            {
                var transferUtilityUploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    InputStream = readStream.OpenReadStream()
                };
                transferUtility.Upload(transferUtilityUploadRequest);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                    ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    throw new Exception("Check the provided AWS Credentials.");
                }
                else
                {
                    throw new Exception("Error occurred: " + amazonS3Exception.Message);
                }
            }
        }
    }
}
