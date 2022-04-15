
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
        [RequestSizeLimit(100 * 1024 * 1024)] //限制http大小
        public void UploadFile(List<IFormFile> files, string subDirectory)
        {
            subDirectory = subDirectory ?? string.Empty;
            var target = Path.Combine(_hostingEnvironment.ContentRootPath, subDirectory);

            Directory.CreateDirectory(target);

            files.ForEach(async file =>
            {
                if (file.Length <= 0) return;
                var filePath = Path.Combine(target, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            });
        }


        [HttpGet("download")]
        public async Task<IActionResult> Get(string id, string fileName)
        {
            try
            {
                var addrUrl = Path.Combine(_hostingEnvironment.ContentRootPath, $@"{id + "\\" + fileName}");
                FileStream fs = new FileStream(addrUrl, FileMode.Open);
                return File(fs, "application/vnd.android.package-archive", fileName);

            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }
    }
}
