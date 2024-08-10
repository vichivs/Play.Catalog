using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepository<Item> itemsRepository;

        public ItemsController(IRepository<Item> _itemsRepository)
        {
            itemsRepository = _itemsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAllAsync()
        {
            var items = (await itemsRepository.GetAllAsync())
                        .Select(item => item.AsDto());
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetItemByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetItemByIdAsync(id);

            if(item is null)
            {
                return NotFound();
            }
            return item.AsDto();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateAsync(CreatedItemDto createdItemDto)
        {
            var item = new Item
            {
                Name = createdItemDto.Name,
                Description = createdItemDto.Description,
                Price = createdItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow,
            }; 
            await itemsRepository.CreateAsync(item);

            return CreatedAtAction(nameof(GetItemByIdAsync), new {id = item.Id}, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdatedItemDto updatedItemDto)
        {
            var existingItem = await itemsRepository.GetItemByIdAsync(id);
            if(existingItem is null)
            {
                return NotFound();
            }
            existingItem.Name = updatedItemDto.Name;
            existingItem.Description = updatedItemDto.Description;
            existingItem.Price = updatedItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemsRepository.GetItemByIdAsync(id);

            if(item is null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);
            return NoContent();
        }
    }
}