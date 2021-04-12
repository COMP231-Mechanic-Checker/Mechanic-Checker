using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace MechanicChecker.AWS
{
    public class AmazonS3Uploader
    {
        public static string folder = "seller";
        //private string bucketName = "mechanic.checker";

        public void UploadFile(string filename, IFormFile readStream,string bucketName)
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
        //This is the method you will call in the account controller
        public void AWSdelete( string filename,string bucketName)
        {
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            deleteFile(filename,bucketName).Wait();
        }


        private static async Task deleteFile(string filename,string bucketName)
        {
            string keyName = string.Format("{0}/{1}", folder, filename);
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                };

                Console.WriteLine("Deleting an object");
                await client.DeleteObjectAsync(deleteObjectRequest);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when deleting an object", e.Message);
            }
        }

    }
}
