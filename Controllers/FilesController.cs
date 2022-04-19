
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace UpDownFiles.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FilesController : Controller
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private IWebHostEnvironment _hostingEnvironment;
        public FilesController(ILogger<WeatherForecastController> logger, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="files"></param>
        /// <param name="subDirectory"></param>
        [HttpPost("upload")]
        //[DisableRequestSizeLimit] //禁用http限制大小
        [RequestSizeLimit(1000 * 1024 * 1024)] //限制http大小
        public async Task<string> UploadFile(List<IFormFile> files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath + "\\附件", subDirectory);

            Directory.CreateDirectory(target);

            files.ForEach(file =>
            {
                if (file.Length <= 0) return;
                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            });
            
            return "上传成功";
        }


        [HttpGet("download")]
        public async Task<IActionResult> Get(string id, string fileName)
        {
            try
            {
                var addrUrl = Path.Combine(_hostingEnvironment.ContentRootPath + "\\附件", $@"{id + "\\" + fileName}");
                FileStream fs = new FileStream(addrUrl, FileMode.Open);
                return File(fs, "application/vnd.android.package-archive", fileName);

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
        [HttpGet("delete")]
        public async Task<int> delete(string id, string fileName)
        {
            try
            {
                var addrUrl = Path.Combine(_hostingEnvironment.ContentRootPath + "\\附件", $@"{id + "\\" + fileName}");
                System.IO.File.Delete(addrUrl);
                return 0;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }
    }
}
