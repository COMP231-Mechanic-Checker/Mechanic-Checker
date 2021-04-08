using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MechanicChecker
{
    public class S3FileUploader
    {
        public string value(IFormFile files)
        {
            //var size = files.Sum(f => f.Length);
            var filePaths = new List<string>();
            string filepath;
            string v;
            Random rnd = new Random();
            int number = rnd.Next(1, 10);
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            string filename = string.Format("{0},{1}", finalString, files.FileName);
            //string encodedStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(filename));
            filepath = Path.Combine(Directory.GetCurrentDirectory(), files.FileName);
            string path = Path.GetFileName(files.FileName);
            Console.WriteLine(filepath);
            filePaths.Add(filepath);
            //amazonS3.UploadFile(filepath, formFile.FileName);
            //using (var stream = new FileStream(filepath, FileMode.Create))
            //{
            //    await formFile.CopyToAsync(stream);
            //}
            AmazonS3Uploader amazonS3 = new AmazonS3Uploader();
            amazonS3.UploadFile(filepath, finalString);
            v = "https://s3.amazonaws.com/mechanic.checker/seller/" + finalString;
            //sql code

            Console.Write(v);
            return v;

        }
       
    }
}
