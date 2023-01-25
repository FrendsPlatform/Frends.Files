using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using Frends.FilesRead;

namespace Frends.FilesRead.Tests
{

    public class UnitTest : FileTestBase
    {
        public UnitTest()
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
            var result = await FilesRead.Read(new ReadInput() { Path = Path.Combine(TestFileContext.RootPath, "folder/test.txt") }, new ReadOption() { });
            Assert.Equal(fileContent, result.Content);
        }

      
        
        private static string BinaryTestFilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestFiles/frends_favicon.png");
    }
}
