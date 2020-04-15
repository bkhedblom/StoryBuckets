using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Implementations;

namespace StoryBuckets.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        // GET: api/Stories
        [HttpGet]
        public IReadOnlyCollection<IStory> Get() 
            => new[] 
            {
                new Story
                {
                    Id = 42,
                    Title = "A Planet-sized computer simulation",
                },
                new Story
                {
                    Id = 31415,
                    Title = "Squaring the circle"
                }
            };

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
