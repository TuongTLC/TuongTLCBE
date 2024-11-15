using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class FileService
{
    private static readonly string Endpoint =
        Environment.GetEnvironmentVariable("EndPoint", EnvironmentVariableTarget.Process)
        ?? throw new ArgumentException("Environment variable not found.");

    private readonly DecodeToken _decodeToken;
    private readonly FileRepo _fileRepo;

    public FileService(FileRepo fileRepo, DecodeToken decodeToken)
    {
        _fileRepo = fileRepo;
        _decodeToken = decodeToken;
    }

    public async Task<object> GetFilesByUser(string token)
    {
        try
        {
            string userId = _decodeToken.Decode(token, "userid");
            List<FileUpload> fileList = await _fileRepo.GetFilesByUser(Guid.Parse(userId));
            return fileList;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /*public async Task<object> UploadFile(List<IFormFile> files, string token)
    {
        try
        {
            List<string> urls = new();
            var userId = _decodeToken.Decode(token, "userid");
            foreach (var file in files)
            {
                var urlPath = Endpoint + "/FileStorage/Pictures/";
                var id = Guid.NewGuid();
                var filename = id + Path.GetExtension(file.FileName);
                urls.Add(urlPath + userId + "/" + filename);
                var checkFolder = Directory.Exists(@"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures\" + userId);
                if (!checkFolder)
                    Directory.CreateDirectory(@"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures\" + userId);
                var desPath = Path.Combine(@"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures", userId, filename);
                /*
                await file.CopyToAsync(new FileStream(desPath, FileMode.Create));
                #1#
                using (var sourceStream = file.OpenReadStream())
                using (var destinationStream = new FileStream(desPath, FileMode.Create))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                FileUpload fileIn = new()
                {
                    Id = id,
                    Path = urlPath + userId + "/" + filename,
                    UploadedBy = Guid.Parse(userId),
                    UploadDate = DateTime.Now
                };
                _ = await _fileRepo.Insert(fileIn);
            }

            return urls;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }*/
    public async Task<object> UploadFile(List<IFormFile> files, string token)
    {
        try
        {
            List<string> urls = new();
            string userId = _decodeToken.Decode(token, "userid");

            foreach (IFormFile file in files)
            {
                string urlPath = "https://statics.tuongtlc.site/FileStorage/Pictures/"; // Update with your actual domain
                Guid id = Guid.NewGuid();
                string filename = id + Path.GetExtension(file.FileName);
                urls.Add(urlPath + userId + "/" + filename);

                string userFolder = Path.Combine("/var/www/statics/WebStatics/FileStorage/Pictures/", userId);
                string desPath = Path.Combine(userFolder, filename);

                // Check if the user's folder exists, and create it if it does not
                if (!Directory.Exists(userFolder))
                {
                    _ = Directory.CreateDirectory(userFolder);
                }

                // Save the file to the specified destination
                using (Stream sourceStream = file.OpenReadStream())
                using (FileStream destinationStream = new(desPath, FileMode.Create))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                // Create a new FileUpload object to store metadata
                FileUpload fileIn = new()
                {
                    Id = id,
                    Path = urlPath + userId + "/" + filename,
                    UploadedBy = Guid.Parse(userId),
                    UploadDate = DateTime.Now
                };
                _ = await _fileRepo.Insert(fileIn);
            }

            return urls;
        }
        catch (Exception e)
        {
            return e.ToString();
        }
    }

    public async Task<object> DeleteFile(string url, string token)
    {
        try
        {
            string userid = _decodeToken.Decode(token, "userid");
            FileUpload? fileUploadGet = await _fileRepo.GetFileByPath(url);
            if (fileUploadGet != null)
            {
                if (!fileUploadGet.UploadedBy.Equals(Guid.Parse(userid)))
                {
                    return "This file isn't your to delete!";
                }

                bool deleteInDb = DeleteImgInStorage(url, token);
                if (deleteInDb)
                {
                    return (await _fileRepo.Delete(fileUploadGet)) > 0;
                }
                //return await _fileRepo.Delete(fileUploadGet) > 0;
            }

            return "File not found!";
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /*private bool DeleteImgInStorage(string imgUrl, string token)
    {
        try
        {
            var userid = _decodeToken.Decode(token, "userid");
            var deletePath = "https://statics.tuongtlc.site/FileStorage/Pictures/" + userid.ToLower() + "/";
            var startIndex = imgUrl.IndexOf(deletePath, StringComparison.Ordinal);

            if (startIndex >= 0)
            {
                var length = deletePath.Length;
                var output = imgUrl.Remove(startIndex, length);
                var filePath = @"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures\" + userid.ToLower() + "\\" +
                               output.ToLower();
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }*/
    private bool DeleteImgInStorage(string imgUrl, string token)
    {
        try
        {
            string userid = _decodeToken.Decode(token, "userid");
            string deletePath = "https://statics.tuongtlc.site/FileStorage/Pictures/" + userid.ToLower() + "/";
            int startIndex = imgUrl.IndexOf(deletePath, StringComparison.Ordinal);

            if (startIndex >= 0)
            {
                int length = deletePath.Length;
                string output = imgUrl.Remove(startIndex, length);
                // Change the file path to a Linux-compatible path
                string filePath = $"/var/www/statics/WebStatics/FileStorage/Pictures/{userid.ToLower()}/{output.ToLower()}";

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }

                return false;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

}