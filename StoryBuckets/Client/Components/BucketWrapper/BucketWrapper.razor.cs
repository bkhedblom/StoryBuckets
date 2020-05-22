using Microsoft.AspNetCore.Components;
using StoryBuckets.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Client.Components.BucketWrapper
{
    public partial class BucketWrapper
    {
        [Parameter]
        public IBucketModel Bucket { get; set; }

        [Parameter]
        public EventCallback<IBucketModel> OnChosen { get; set; }

        [Parameter]
        public bool DisableChoosing { get; set; }

        public async Task OnClickChoose()
        {
            await OnChosen.InvokeAsync(Bucket);
        }
    }
}
