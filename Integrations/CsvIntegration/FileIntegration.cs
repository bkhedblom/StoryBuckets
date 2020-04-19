using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryBuckets.Integrations.CsvIntegration
{
	public class FileIntegration : IIntegration
	{
		private readonly IFileReader _filereader;

		public FileIntegration(IFileReader filereader)
		{
			_filereader = filereader;
		}
		public async Task<IEnumerable<IStoryFromIntegration>> FetchAsync()
		{
			var mappedStories = new List<IStoryFromIntegration>();
			var parentIds = new HashSet<int>();
			await foreach (var storyFromFile in _filereader.ParseAsync())
			{
				mappedStories.Add(new StoryFromIntegration { 
					Id = storyFromFile.Id,
					Title = storyFromFile.Title
				});
				
				if(storyFromFile.ParentId.HasValue && !parentIds.Contains(storyFromFile.ParentId.Value))
					parentIds.Add(storyFromFile.ParentId.Value);
			}

			return mappedStories.Where(story => !parentIds.Contains(story.Id));
		}
	}
}
