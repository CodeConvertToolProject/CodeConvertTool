
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeConverterTool.Models;
using Microsoft.AspNetCore.Authorization;
using Amazon.S3.Transfer;
using Amazon.S3;

namespace CodeConverterTool.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScriptsController : ControllerBase
    {
        private readonly ConvertToolDbContext _context;


        public ScriptsController(ConvertToolDbContext context)
        {
            _context = context;
        }

        [HttpPost("UploadScript")]
        public async Task<IActionResult> UploadFileToS3(IFormFile file)
        {
            using var s3Client = new AmazonS3Client("", "", Amazon.RegionEndpoint.EUWest1);
            using var fileTransferUtility = new TransferUtility(s3Client);

            try
            {
                await fileTransferUtility.UploadAsync(file.OpenReadStream(), "codeconvertbucket", file.FileName);
                Console.WriteLine("Upload completed successfully.");
                return Ok("File Uploaded");
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }

            return Ok("Error Occurd");
        }



        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Script>>> GetScripts()
        {
            return await _context.Scripts.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Script>> GetScript(char id)
        {
            if (!Char.IsDigit(id))
            {
                return BadRequest("Invalid ID format");
            }

            int idValue = int.Parse("" + id);

            if (idValue <= 0)
            {
                return BadRequest("Invalid ID format. Must be a Positive Integer.");
            }

            var script = await _context.Scripts.FindAsync(idValue);

            if (script == null)
            {
                return NotFound();
            }

            return script;
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScript(int id, Script script)
        {
            if (id != script.ScriptId)
            {
                return BadRequest();
            }

            _context.Entry(script).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScriptExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Script>> PostScript(Script script)
        {
            _context.Scripts.Add(script);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScript", new { id = script.ScriptId }, script);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScript(char id)
        {
            if (!Char.IsDigit(id))
            {
                return BadRequest("Invalid ID format");
            }

            int idValue = int.Parse("" + id);

            if (idValue <= 0)
            {
                return BadRequest("Invalid ID format. Must be a Positive Integer.");
            }

            var script = await _context.Scripts.FindAsync(idValue);
            if (script == null)
            {
                return NotFound();
            }

            _context.Scripts.Remove(script);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ScriptExists(int id)
        {
            return _context.Scripts.Any(e => e.ScriptId == id);
        }
    }
}