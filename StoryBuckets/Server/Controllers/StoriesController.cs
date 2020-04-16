using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoryBuckets.Server.Services;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Implementations;

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
        public async Task<IEnumerable<IStory>> Get()
            => await _service.GetAllAsync();

        //// GET: api/Stories/5
        //[HttpGet("{id}", Name = "Get")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// PUT: api/Stories/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}
    }
}
