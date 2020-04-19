using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StoryBuckets.Integrations.CsvIntegration
{
	public class FileIntegration : IIntegration
	{
		private readonly IFileReader _filereader;

		public FileIntegration(IFileReader fileReader)
		{
			_filereader = fileReader;
		}
		public async Task<IEnumerable<IStoryFromIntegration>> FetchAsync()
		{
			throw new NotImplementedException();
		}
	}
}
