using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoryBuckets.Shared;
using StoryBuckets.Shared.Interfaces;

namespace StoryBuckets.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BucketsController : ControllerBase
    {
        // GET: api/buckets
        [HttpGet]
        public async Task<IEnumerable<IBucket>> Get() => await Task.FromResult(Enumerable.Empty<IBucket>());
    }
}