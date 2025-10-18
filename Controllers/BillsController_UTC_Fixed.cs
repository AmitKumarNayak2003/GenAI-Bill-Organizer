using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BillOrganizerAPI.Data;
using BillOrganizerAPI.Models;
using BillOrganizerAPI.Services;
using Newtonsoft.Json;

namespace BillOrganizerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly GeminiService _geminiService;
        private readonly IWebHostEnvironment _environment;

        public BillsController(AppDbContext context, GeminiService geminiService, IWebHostEnvironment environment)
        {
            _context = context;
            _geminiService = geminiService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBills()
        {
            return await _context.Bills.Include(b => b.Items).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bill>> GetBill(int id)
        {
            var bill = await _context.Bills.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == id);
            if (bill == null)
            {
                return NotFound();
            }
            return bill;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Bill>> UploadBill(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            try
            {
                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                string geminiResponse = await _geminiService.ExtractBillDataAsync(base64Image);
                string extractedJson = _geminiService.ParseGeminiResponse(geminiResponse);

                dynamic? billData = JsonConvert.DeserializeObject(extractedJson);

                DateTime billDate = DateTime.UtcNow;
                if (DateTime.TryParse(billData?.date?.ToString(), out DateTime parsedDate))
                {
                    billDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                }

                var bill = new Bill
                {
                    FileName = file.FileName,
                    MerchantName = billData?.merchantName ?? "Unknown",
                    BillDate = billDate,
                    TotalAmount = billData?.totalAmount ?? 0,
                    Category = billData?.category ?? "Uncategorized",
                    ExtractedText = extractedJson,
                    ImagePath = filePath,
                    UploadedAt = DateTime.UtcNow
                };

                if (billData?.items != null)
                {
                    foreach (var item in billData.items)
                    {
                        bill.Items.Add(new BillItem
                        {
                            ItemName = item.name ?? "Unknown Item",
                            Price = item.price ?? 0,
                            Quantity = item.quantity ?? 1
                        });
                    }
                }

                _context.Bills.Add(bill);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBill), new { id = bill.Id }, bill);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill(int id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }

            if (System.IO.File.Exists(bill.ImagePath))
            {
                System.IO.File.Delete(bill.ImagePath);
            }

            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}