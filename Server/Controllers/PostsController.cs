using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class PostsController : ControllerBase
    {
        private readonly AppDBContext _appDBContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;

        public PostsController(AppDBContext appDBContext, IWebHostEnvironment webHostEnvironment, IMapper mapper)
        {
            _appDBContext = appDBContext;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
        }

        #region CRUD operations

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            List<Post> posts = await _appDBContext.Posts
                .Include(post => post.Category)
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("dto/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDTO(int id)
        {
            Post post = await GetPostByPostId(id);
            PostDTO postDTO = _mapper.Map<PostDTO>(post);

            return Ok(postDTO);
        }

        // website.com/api/posts/2
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            Post post = await GetPostByPostId(id);

            return Ok(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PostDTO postToCreateDTO)
        {
            try
            {
                if (postToCreateDTO == null)
                {
                    return BadRequest(ModelState);
                }

                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                Post postToCreate = _mapper.Map<Post>(postToCreateDTO);

                if (postToCreate.Published == true)
                {
                    // European DateTime
                    postToCreate.PublishDate = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm");
                }

                await _appDBContext.Posts.AddAsync(postToCreate);

                bool changesPersistedToDatabase = await PersistChangesToDatabase();

                if (changesPersistedToDatabase == false)
                {
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator.");
                }
                else
                {
                    return Created("Create", postToCreate);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message}.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PostDTO updatedPostDTO)
        {
            try
            {
                if (id < 1 || updatedPostDTO == null || id != updatedPostDTO.PostId)
                {
                    return BadRequest(ModelState);
                }

                Post oldPost = await _appDBContext.Posts.FindAsync(id);

                if (oldPost == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                Post updatedPost = _mapper.Map<Post>(updatedPostDTO);

                if (updatedPost.Published == true)
                {
                    if (oldPost.Published == false)
                    {
                        updatedPost.PublishDate = DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm");
                    }
                    else
                    {
                        updatedPost.PublishDate = oldPost.PublishDate;
                    }                    
                }
                else
                {
                    updatedPost.PublishDate = string.Empty;
                }

                // Detach oldPost from EF, else it can't be updated.
                _appDBContext.Entry(oldPost).State = EntityState.Detached;

                _appDBContext.Posts.Update(updatedPost);

                bool changesPersistedToDatabase = await PersistChangesToDatabase();

                if (changesPersistedToDatabase == false)
                {
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator.");
                }
                else
                {
                    return Created("Create", updatedPost);
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message}.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1)
                {
                    return BadRequest(ModelState);
                }

                bool exists = await _appDBContext.Posts.AnyAsync(post => post.PostId == id);

                if (exists == false)
                {
                    return NotFound();
                }

                if (ModelState.IsValid == false)
                {
                    return BadRequest(ModelState);
                }

                Post postToDelete = await GetPostByPostId(id);

                if (postToDelete.ThumbnailImagePath != "uploads/placeholder.jpg")
                {
                    string fileName = postToDelete.ThumbnailImagePath.Split('/').Last();

                    System.IO.File.Delete($"{_webHostEnvironment.ContentRootPath}\\wwwroot\\uploads\\{fileName}");
                }

                _appDBContext.Posts.Remove(postToDelete);

                bool changesPersistedToDatabase = await PersistChangesToDatabase();

                if (changesPersistedToDatabase == false)
                {
                    return StatusCode(500, "Something went wrong on our side. Please contact the administrator.");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Something went wrong on our side. Please contact the administrator. Error message: {e.Message}.");
            }
        }

        #endregion

        #region Utility methods

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<bool> PersistChangesToDatabase()
        {
            int amountOfChanges = await _appDBContext.SaveChangesAsync();

            return amountOfChanges > 0;
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        private async Task<Post> GetPostByPostId(int postId)
        {
            Post postToGet = await _appDBContext.Posts
                    .Include(post => post.Category)
                    .FirstAsync(post => post.PostId == postId);

            return postToGet;
        }

        #endregion
    }
}
