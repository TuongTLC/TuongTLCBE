using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TuongTLCBE.Business;
using TuongTLCBE.Data.Entities;

namespace TuongTLCBE.API;

[Route("file")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly FileService _fileService;

    public FileController(FileService fileService)
    {
        _fileService = fileService;
    }
    [HttpPost("upload-files")]
    [SwaggerOperation(Summary = "Upload files")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> UploadFile([FromForm]List<IFormFile> files)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _fileService.UploadFile(files, token);
        return result.GetType() == typeof(List<string>) ? Ok(result) : BadRequest(result);
    }
    [HttpGet("get-files")]
    [SwaggerOperation(Summary = "Get files by user")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> GetFiles()
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _fileService.GetFilesByUser(token);
        return result.GetType() == typeof(List<FileUpload>) ? Ok(result) : BadRequest(result);
    }
    [HttpDelete("delete-file")]
    [SwaggerOperation(Summary = "Delete user file")]
    [Authorize(Roles = "User, Admin")]
    public async Task<IActionResult> DeleteFile(string fileUrl)
    {
        string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
        object result = await _fileService.DeleteFile(fileUrl,token);
        if (result is bool)
        {
            return (bool)result ? Ok(result) : BadRequest(result);
        }
        else
        {
            return BadRequest(result);
        }
    }
}