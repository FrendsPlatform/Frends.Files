using System;

namespace Frends.Files.Read.Tests
{
    public abstract class FileTestBase : IDisposable
    {
        protected DisposableFileSystem TestFileContext;

        protected FileTestBase()
        {
            TestFileContext = new DisposableFileSystem();
        }


        public void Dispose()
        {
            TestFileContext?.Dispose();
            TestFileContext = null;
        }
    }
}
