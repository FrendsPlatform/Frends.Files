using System.IO;
using System.Threading.Tasks;
using Xunit;


namespace Frends.Files.Read.Tests
{

    public class UnitTest_TaskReadFileContent_IsResultEqualToFileConten : FileTestBase
    {
        public UnitTest_TaskReadFileContent_IsResultEqualToFileConten()
        {
            TestFileContext.CreateFiles(
                    "folder/foo/sub/test.xml",
                    "folder/bar/sub/example.xml");
        }

        [Fact]
        public async Task ReadFileContent()
        {
            var fileContent = "Well this is content with some extra nice ümlauts: ÄÖåå 你好!";

            TestFileContext.CreateFile("Folder/test.txt", fileContent);
            var result = await FilesRead.Read(new ReadInput() { Path = Path.Combine(TestFileContext.RootPath, "folder/test.txt") }, new ReadOptions() { });

            Assert.Equal(fileContent, result.Content);
        }

    }
}

