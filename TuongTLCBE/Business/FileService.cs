using TuongTLCBE.Data.Entities;
using TuongTLCBE.Data.Repositories;

namespace TuongTLCBE.Business;

public class FileService
{
    private readonly FileRepo _fileRepo;
    private readonly DecodeToken _decodeToken;
    private static readonly string Endpoint =
        Environment.GetEnvironmentVariable("EndPoint", EnvironmentVariableTarget.Process)
        ?? throw new ArgumentException("Environment variable not found.");
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

    public async Task<object> UploadFile(List<IFormFile> files, string token)
    {
        try
        {
            List<string> urls = new();
            string userId = _decodeToken.Decode(token, "userid");
            foreach (var file in files)
            {
                string urlPath = Endpoint + ":8080/FileStorage/Pictures/";
                Guid id = Guid.NewGuid();
                string filename = id + Path.GetExtension(file.FileName);
                urls.Add( urlPath + userId +"/"+ filename);
                bool checkFolder = Directory.Exists(@"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures\"+userId);
                if (!checkFolder)
                {
                    Directory.CreateDirectory(@"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures\"+userId);
                }
                string desPath = Path.Combine(@"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures", userId, filename);
                await file.CopyToAsync(new FileStream(desPath, FileMode.Create));
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
                int deleteInDb = await _fileRepo.Delete(fileUploadGet);
                if (deleteInDb > 0)
                {
                    return DeleteImgInStorage(url, token);
                }
                else
                {
                    return "Failed to delete file in Database!";
                }
            }
            else
            {
                return "File not found!";
            }
        }
        catch (Exception e)
        {
            return e;
        }
    }

    private bool DeleteImgInStorage(string imgUrl, string token)
    {

        try
        {
            string userid = _decodeToken.Decode(token, "userid");
            string deletePath = "https://tuongtlc.ddns.net:8080/FileStorage/Pictures/"+userid+"/";
            int startIndex = imgUrl.IndexOf(deletePath, StringComparison.Ordinal);

            if (startIndex >= 0)
            {
                int length = deletePath.Length;
                string output =  imgUrl.Remove(startIndex, length) ;
                string filePath = @"N:\TuongTLCWebsite_Data\Webdata\FileStorage\Pictures\" + userid + "\\" + output;
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

        }
        catch
        {
            return false;

        }
    }
    
}