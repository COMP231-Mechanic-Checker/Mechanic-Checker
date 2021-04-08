using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker
{
    public class AmazonS3Uploader
    {
        public static string folder = "seller";
        private string bucketName = "mechanic.checker";
        // private string keyName = string.Format("{0}/{1}", folder, filename);
        //private string filePath = "C:\\Users\\EMMAL\\Documents\\Laptop Stuff\\Pictures\\b.jpg";

        public void UploadFile(string filepath, string filename)
        {
            string keyName = string.Format("{0}/{1}", folder, filename);
            var client = new AmazonS3Client(Amazon.RegionEndpoint.USEast1);
            var transferUtility = new TransferUtility(client);

            try
            {
                TransferUtilityUploadRequest transferUtilityUploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = keyName,
                    FilePath = filepath
                };
                transferUtility.Upload(transferUtilityUploadRequest);
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
                request.BucketName = bucketName;
                request.Key = keyName;
                request.Expires = DateTime.Now.AddHours(11);
                request.Protocol = Protocol.HTTP;
                string url = client.GetPreSignedURL(request);
                Console.WriteLine(url);
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
