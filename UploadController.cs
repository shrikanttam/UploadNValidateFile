using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace UploadNvalidateFiles.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost("upload", Name = "upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationToken)
        {
            if (CheckIfTextFile(file))
            {
                string fileName;
                try
                {
                    var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
                    fileName = DateTime.Now.Ticks + extension; //Create a new Name for the file due to security reasons.

                    var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files");

                    if (!Directory.Exists(pathBuilt))
                    {
                        Directory.CreateDirectory(pathBuilt);
                    }

                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Upload\\files",
                       fileName);

                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    string[] lines = System.IO.File.ReadAllLines(path);
                    StringBuilder messages = new StringBuilder();
                    foreach (string line in lines)
                    {
                        StringBuilder accountName = new StringBuilder();
                        StringBuilder accountNumber = new StringBuilder();

                        char[] b = new char[line.Length];
                        b = line.ToCharArray();
                        int count = 0;
                        for (int i = 0; i < line.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(line[i].ToString()) && count == 0)
                            {
                                accountName.Append(line[i].ToString());
                            }
                            else
                            {
                                if (count != 0)
                                {
                                    accountNumber.Append(line[i].ToString());
                                }
                                count++;
                            }
                        }
                        if (accountNumber.Length > 8)
                        {
                            messages.Append(accountNumber.ToString() + " ");
                        }
                    }

                    if (messages.Length > 0)
                    {
                        return BadRequest("Invalid Accounts : " + messages);
                    }
                }
                catch (Exception e)
                {
                    //log error
                }
            }
            else
            {
                return BadRequest(new { message = "Invalid file extension" });
            }

            return Ok(new { message = "File uploaded successfully" });
        }

        /// <summary>
        /// Method to check if file is text file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool CheckIfTextFile(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];
            return (extension == ".txt"); // Change the extension based on your need
        }
    }
}
