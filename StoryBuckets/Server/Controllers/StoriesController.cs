using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoryBuckets.Services;
using StoryBuckets.Shared;

namespace StoryBuckets.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryService _service;

        public StoriesController(IStoryService service)
        {
            _service = service;
        }

        // GET: api/Stories
        [HttpGet]
        public async Task<IEnumerable<Story>> Get() => await _service.GetAllAsync();

        //// GET: api/Stories/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // PUT: api/Stories/5
        [HttpPut("{id}")]
        public async Task<Story> Put(int id, [FromBody] Story story)
        {
            await _service.UpdateAsync(id, story);
            return story;
        }
    }
}
