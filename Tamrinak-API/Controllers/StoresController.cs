using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tamrinak_API.DataAccess;
using Tamrinak_API.DataAccess.Models;
using Tamrinak_API.DTO.StoreDtos;
using Tamrinak_API.Services.ImageService;

namespace Tamrinak_API.Controllers
{
    [ApiController]
    [Route("api/store")]
    public class StoreController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly IImageService _imageService;

        public StoreController(DatabaseContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // =========================================================
        // GET /store/items
        // RETURNS: List<ItemListDto>
        // =========================================================
        [HttpGet("items")]
        public async Task<ActionResult<List<ItemListDto>>> GetAllItems()
        {
            var items = await _context.Items
                .Where(i => i.IsActive)
                .Select(i => new ItemListDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Price = i.Price,
                    InStock = i.StockQuantity > 0,
                    Image = _context.Images
                        .Where(img => img.ItemId == i.Id)
                        .Select(img => img.Base64Data)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(items);
        }

        // =========================================================
        // GET /store/items/{id}
        // RETURNS: ItemDetailsDto
        // =========================================================
        [HttpGet("items/{id}")]
        public async Task<ActionResult<ItemDetailsDto>> GetItemDetails(int id)
        {
            var item = await _context.Items
                .Where(i => i.Id == id && i.IsActive)
                .Select(i => new ItemDetailsDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Price = i.Price,
                    StockQuantity = i.StockQuantity,
                    Images = _context.Images
                        .Where(img => img.ItemId == i.Id)
                        .Select(img => img.Base64Data)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        // =========================================================
        // POST /store/items
        // TAKES: CreateItemDto (Form)
        // RETURNS: { id }
        // =========================================================
        [Authorize(Roles = "Admin,VenueManager")]
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromForm] CreateItemDto dto)
        {
            var item = new Items
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                IsActive = true
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            if (dto.Images != null && dto.Images.Any())
            {
                var canAdd = await _imageService
                    .CanAddEntityImagesAsync<Items>(item.Id, 5);

                if (!canAdd)
                    return BadRequest("Maximum number of images reached");

                var base64Images =
                    await _imageService.UploadImagesAsync(dto.Images, "items");

                foreach (var base64 in base64Images)
                {
                    await _imageService.AddImageAsync(new Image
                    {
                        Base64Data = base64,
                        ItemId = item.Id
                    });
                }
            }

            return Ok(new { item.Id });
        }

        // =========================================================
        // DELETE /store/items/{id}
        // RETURNS: 200 OK
        // =========================================================
        [Authorize(Roles = "Admin,VenueManager")]
        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
                return NotFound();

            item.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
