﻿using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRESTServices.BLL.DTOs;
using MyRESTServices.BLL.Interfaces;
using MyRESTServices.Models;

namespace MyRESTServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleBLL _articleBLL;
        private readonly IUserBLL _userBLL;
        private readonly IValidator<ArticleCreateDTO> _validatorArticleCreateDto;
        private readonly IValidator<ArticleUpdateDTO> _validatorArticleUpdateDTO;

        public ArticlesController(IArticleBLL articleBLL, IUserBLL userBLL, IValidator<ArticleCreateDTO> validatorArticleCreateDto, IValidator<ArticleUpdateDTO> validatorArticleUpdateDTO)
        {
            _articleBLL = articleBLL;
            _userBLL = userBLL;
            _validatorArticleCreateDto = validatorArticleCreateDto;
            _validatorArticleUpdateDTO = validatorArticleUpdateDTO;
        }

        // GET: api/Articles
        [Authorize(Roles = "admin,contributor,reader")]
        [HttpGet]
        public async Task<IEnumerable<ArticleDTO>> Get()
        {
            return await _articleBLL.GetArticleWithCategory();
        }

        //GET: api/Articles/5
        [Authorize(Roles = "admin,contributor,reader")]
        [HttpGet("{id}")]
        public async Task<ArticleDTO> Get(int id)
        {
            return await _articleBLL.GetArticleById(id);
        }

        //POST api/Articles
        [Authorize(Roles = "admin,contributor")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ArticleCreateDTO articleCreateDTO)
        {
            var validationResult = _validatorArticleCreateDto.Validate(articleCreateDTO);
            if (!validationResult.IsValid)
            {
                Helpers.Extensions.AddToModelState(validationResult, ModelState);
                return BadRequest(ModelState);
            }

            var article = await _articleBLL.Insert(articleCreateDTO);
            return CreatedAtAction(nameof(Get), new { id = article.ArticleID }, article);
        }

        //POST with upload file
        /*[HttpPost("upload")]
        public async Task<IActionResult> Post([FromForm] IFormFile file, [FromForm] int CategoryID,
            [FromForm] string Title, [FromForm] string Details, [FromForm] bool IsApprove)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is required");
            }
            var newName = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var articleCreateDTO = new ArticleCreateDTO
            {
                CategoryId = CategoryID,
                Title = Title,
                Details = Details,
                IsApproved = IsApprove,
                Pic = newName
            };

            var article = await _articleBLL.Insert(articleCreateDTO);

            return CreatedAtAction(nameof(Get), new { id = article.ArticleID }, article);
        }*/

        [Authorize(Roles = "admin,contributor")]
        [HttpPost("upload")]
        public async Task<IActionResult> Post([FromForm] ArticleWithFile articleWithFile)
        {
            if (articleWithFile.file == null || articleWithFile.file.Length == 0)
            {
                return BadRequest("File is required");
            }
            var newName = $"{Guid.NewGuid()}_{articleWithFile.file.FileName}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", newName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await articleWithFile.file.CopyToAsync(stream);
            }

            var articleCreateDTO = new ArticleCreateDTO
            {
                CategoryId = articleWithFile.CategoryId,
                Title = articleWithFile.Title,
                Details = articleWithFile.Details,
                IsApproved = articleWithFile.IsApproved,
                Pic = newName
            };

            var article = await _articleBLL.Insert(articleCreateDTO);
            return CreatedAtAction(nameof(Get), new { id = article.ArticleID }, article);
        }

        //PUT 
        [Authorize(Roles = "admin,contributor")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] ArticleUpdateDTO articleUpdateDTO)
        {
            var validationResult = _validatorArticleUpdateDTO.Validate(articleUpdateDTO);
            if (!validationResult.IsValid)
            {
                Helpers.Extensions.AddToModelState(validationResult, ModelState);
                return BadRequest(ModelState);
            }

            var article = await _articleBLL.Update(id, articleUpdateDTO);
            return Ok(article);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _articleBLL.Delete(id);
            if (result)
            {
                return Ok();
            }
            return NotFound();
        }
    }
}

