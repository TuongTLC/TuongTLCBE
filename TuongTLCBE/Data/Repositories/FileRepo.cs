using Microsoft.EntityFrameworkCore;
using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.Data.Repositories;

public class FileRepo: Repository<FileUpload>
{
    public FileRepo(TuongTlcdbContext context) : base(context)
    {
    }

    public async Task<List<FileUpload>> GetFilesByUser(Guid userId)
    {
        return await context.FileUploads.Where(x => x.UploadedBy.Equals(userId)).OrderByDescending(y => y.UploadDate).ToListAsync();
    }

    public async Task<FileUpload?> GetFileByPath(string path)
    {
        return await context.FileUploads.Where(x => x.Path.Equals(path)).FirstOrDefaultAsync();
    }
}