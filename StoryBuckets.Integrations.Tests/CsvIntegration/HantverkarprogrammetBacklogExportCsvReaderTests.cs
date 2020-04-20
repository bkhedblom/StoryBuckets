using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using StoryBuckets.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace StoryBuckets.Integrations.CsvIntegration.Tests
{
    [TestClass()]
    public class HantverkarprogrammetBacklogExportCsvReaderTests
    {
        private const string correctFileHeader = HantverkarprogrammetBacklogExportCsvReader.CorrectFileHeader;

        [TestMethod()]
        public void Gets_path_from_the_path_provider()
        {
            //Arrange
            var pathProvider = GetPathProvider();
            var filesystem = GetFileSystemWithFakedOpenText(correctFileHeader);
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, pathProvider.Object);

            //Act
            _ = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            pathProvider.Verify(mock => mock.GetPathToIntegrationFile(), Times.Once);
        }

        [TestMethod()]
        public void Opens_the_File_as_text()
        {
            //Arrange
            const string path = @"c:\foo\bar.csv";
            var filesystem = GetFileSystemWithFakedOpenText(correctFileHeader);
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider(path).Object);

            //Act
            _ = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            filesystem.Verify(mock => mock.OpenText(path), Times.Once);
        }


        [TestMethod()]
        public void Checks_the_header_and_throws_if_unexpected_columns()
        {
            //Arrange
            var filesystem = GetFileSystemWithFakedOpenText("foobar,this,is,all,wrong");

            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            //Assert
             Assert.ThrowsExceptionAsync<UnexpectedCsvHeaderException>(() => Enumerate(filereader.ParseAsync()));
        }

        [TestMethod()]
        public void Parses_remaining_lines_as_Stories()
        {
            //Arrange
            var filesystem = GetFileSystemWithFakedOpenText(new[] { 
                correctFileHeader,
                "\"Product Backlog Item\",\"Approved\",\"16432\",\"16425\",\"Inloggning med konto specifikt för denna tjänst\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
                "\"Product Backlog Item\",\"Approved\",\"16325\",\"16411\",\"Krav på inloggning när oninloggad går till någon sida i systemet (oavsett om sidan finns eller inte)\",\"Story 1:Som kund vill jag att länkar in i systemet bara ska visa data om användaren loggar in så att länkar som råkar spridas felaktigt inte låter obehöriga få tag på min dataData kan t.ex. vara registrerad tid, registrerade uppdrag etc. All information som finns i systemet.Teoretiskt sett skulle man kunna visa all struktur etc på sidan, så länge det inte innehåller användares data, men det blir nog inte så snyggtStory 2:Som säkerhetsmedveten vill jag att länkar som pekar på icke-existerande data ska behandlas som om användaren inte är behörig så att det inte går att gissa vilken data som finnsStory 4:Som användare vill jag komma till inloggningen om jag oinloggad har gått in på en länk direkt in i systemet så att jag inte behöver manuellt navigera till inloggningenDet skulle kunna vara att man skickas vidare till inloggning, att en inloggningsmodal visas, etc.I framtiden vill vi troligen att man ska skickas tillbaka till den sida man gick till när man fick inloggningen (se Story 3 men det är inte nödvändigt i det här skedet)\"",
                "\"Product Backlog Item\",\"Approved\",\"16377\",\"16410\",\"Koppla användare till företag vid inloggning\",\"Story 3:Som användare vill jag kopplas till mitt företag när jag loggar in så att jag kan komma åt den data jag ska jobba med\"",
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            Assert.AreEqual(3, result.Count);
            result.ForEach((item => Assert.IsNotNull(item)));
        }

        [TestMethod()]
        public void Parses_Id()
        {
            //Arrange
            var id = 16432;
            var filesystem = GetFileSystemWithFakedOpenText(new[] {
                correctFileHeader,
                $"\"Product Backlog Item\",\"Approved\",\"{id}\",\"16425\",\"Inloggning med konto specifikt för denna tjänst\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            Assert.AreEqual(id, result.Single().Id);
        }

        [TestMethod()]
        public void Parses_Title()
        {
            //Arrange
            var title = "Inloggning med konto specifikt för denna tjänst";
            var filesystem = GetFileSystemWithFakedOpenText(new[] {
                correctFileHeader,
                $"\"Product Backlog Item\",\"Approved\",\"16432\",\"16425\",\"{title}\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            Assert.AreEqual(title, result.Single().Title);
        }

        [TestMethod()]
        public void Handles_commas_in_column_values()
        {
            //Arrange
            var title = "Inloggning, med konto specifikt för denna tjänst (inte Google, Microsoft, Twitter, etc)";
            var filesystem = GetFileSystemWithFakedOpenText(new[] {
                correctFileHeader,
                $"\"Product Backlog Item\",\"Approved\",\"16432\",\"16425\",\"{title}\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            Assert.AreEqual(title, result.Single().Title);
        }

        [TestMethod()]
        public void Handles_quotes_in_column_values_if_they_are_escaped_by_being_doubled_and_removes_the_escaping()
        {
            //Arrange
            var filesystem = GetFileSystemWithFakedOpenText(new[] {
                correctFileHeader,
                $"\"Product Backlog Item\",\"Approved\",\"16432\",\"16425\",\"Inloggning med \"\"lokalt konto\"\" (specifikt för denna tjänst, inte Google, Microsoft, Twitter, etc)\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            Assert.AreEqual("Inloggning med \"lokalt konto\" (specifikt för denna tjänst, inte Google, Microsoft, Twitter, etc)", result.Single().Title);
        }

        [TestMethod()]
        public void Parses_ParentId()
        {
            //Arrange
            var parentId = 16425;
            var filesystem = GetFileSystemWithFakedOpenText(new[] {
                correctFileHeader,
                $"\"Product Backlog Item\",\"Approved\",\"16432\",\"{parentId}\",\"Inloggning med konto specifikt för denna tjänst\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            Assert.AreEqual(parentId, result.Single().ParentId);
        }

        [TestMethod()]
        public void Sets_non_existing_parentId_to_null()
        {
            //Arrange
            var filesystem = GetFileSystemWithFakedOpenText(new[] {
                correctFileHeader,
                $"\"Product Backlog Item\",\"Approved\",\"16432\",,\"Inloggning med konto specifikt för denna tjänst\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            Assert.IsNull(result.Single().ParentId);
        }

        [TestMethod()]
        public void Handles_reading_other_properties_with_non_existing_parentId()
        {
            //Arrange
            var id1 = 16432;
            var title1 = "Inloggning med konto specifikt för denna tjänst";
            var id2 = 16603;
            var title2 = "Kundregister";
            
            var filesystem = GetFileSystemWithFakedOpenText(new[] {
                correctFileHeader,
                $"\"Product Backlog Item\",\"Approved\",\"{id1}\",,\"{title1}\",\"Story 0:Som utvecklare vill jag inte behöva hantera användares inloggningsuppgifter för att slippa det otroligt svåra jobbet att göra detta korrektStory 1:Om jag saknar konto hos någon annan kontogivare så vill jag kunna skapa ett konto specifikt för denna tjänstStory 2:Som användare vill jag kunna logga in med ett konto specifikt för denna tjänst så att jag kan hålla mina inloggningar separerade\"",
                $"\"Feature\",\"New\",\"{id2}\",,\"{title2}\","           
            });
            var filereader = new HantverkarprogrammetBacklogExportCsvReader(filesystem.Object, GetPathProvider().Object);

            //Act
            var result = Enumerate(filereader.ParseAsync()).Result;

            //Assert
            var firstItem = result.First();
            var secondItem = result.Skip(1).First();

            Assert.AreEqual(id1, firstItem.Id);
            Assert.AreEqual(title1, firstItem.Title);
            Assert.AreEqual(id2, secondItem.Id);
            Assert.AreEqual(title2, secondItem.Title);
        }

        private static async Task<List<T>> Enumerate<T>(IAsyncEnumerable<T> asyncEnumerable)
        {
            var list = new List<T>();
            await foreach (var item in asyncEnumerable)
            {
                list.Add(item);
            }
            return list;
        }

        private static Mock<IFilesystemIo> GetFileSystemWithFakedOpenText(string fileheader)
             => GetFileSystemWithFakedOpenText(new[] { fileheader });

        private static Mock<IFilesystemIo> GetFileSystemWithFakedOpenText(string[] content)
        {
            var filesystem = new Mock<IFilesystemIo>();
            var contentString = string.Join(Environment.NewLine, content);
            var fileBytes = Encoding.UTF8.GetBytes(contentString);
            var stream = new MemoryStream(fileBytes);
            filesystem
                .Setup(fake => fake.OpenText(It.IsAny<string>()))
                .Returns(new StreamReader(stream));
            return filesystem;
        }

        private static Mock<IIntegrationPathProvider> GetPathProvider(string path = @"c:\foo\bar.csv")
        {
            var provider = new Mock<IIntegrationPathProvider>();
            provider
                .Setup(fake => fake.GetPathToIntegrationFile())
                .Returns(path);
            return provider;
        }

    }
}