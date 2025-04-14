using System.IO.Abstractions.TestingHelpers;
using System.Text;
using API.Adapters;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace UnitTests.API.Adapters
{
    [TestFixture]
    public class FormFileAdapterTests
    {
        private Mock<IFormFile> _formFileMock;
        private MockFileSystem _fileSystemMock;
        private string _testFileName;
        private byte[] _testFileContent;

        [SetUp]
        public void SetUp()
        {
            _formFileMock = new Mock<IFormFile>();
            _fileSystemMock = new MockFileSystem();
            _testFileName = "test.txt";
            _testFileContent = Encoding.UTF8.GetBytes("Hello, world!");
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_WhenFileIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FormFileAdapter(null));
        }

        [Test]
        public void Constructor_ShouldInitializeFileNameAndBytes_WhenFileIsValid()
        {
            // Arrange
            SetupFormFileMock(_testFileName, _testFileContent);

            // Act
            var adapter = new FormFileAdapter(_formFileMock.Object);

            // Assert
            adapter.FileName.Should().Be(_testFileName);
            adapter.ContentLength.Should().Be(_testFileContent.Length);
        }

        [Test]
        public void FileName_ShouldReturnCorrectFileName()
        {
            // Arrange
            SetupFormFileMock(_testFileName, _testFileContent);
            var adapter = new FormFileAdapter(_formFileMock.Object);

            // Act
            var fileName = adapter.FileName;

            // Assert
            fileName.Should().Be(_testFileName);
        }

        [Test]
        public void ContentLength_ShouldReturnCorrectLength()
        {
            // Arrange
            SetupFormFileMock(_testFileName, _testFileContent);
            var adapter = new FormFileAdapter(_formFileMock.Object);

            // Act
            var length = adapter.ContentLength;

            // Assert
            length.Should().Be(_testFileContent.Length);
        }

        [Test]
        public void InputStream_ShouldReturnStreamWithCorrectContent()
        {
            // Arrange
            SetupFormFileMock(_testFileName, _testFileContent);
            var adapter = new FormFileAdapter(_formFileMock.Object);

            // Act
            using var stream = adapter.InputStream;
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEnd();

            // Assert
            content.Should().Be(Encoding.UTF8.GetString(_testFileContent));
        }

        [Test]
        public void SaveAs_ShouldWriteFileToPath()
        {
            // Arrange
            SetupFormFileMock(_testFileName, _testFileContent);
            var adapter = new FormFileAdapter(_formFileMock.Object);
            var path = "output.txt";

            // Act
            adapter.SaveAs(path);

            // Assert
            var savedContent = File.ReadAllBytes(path);
            savedContent.Should().BeEquivalentTo(_testFileContent);
        }

        [Test]
        public void SaveAs_ShouldThrowArgumentNullException_WhenPathIsNull()
        {
            // Arrange
            SetupFormFileMock(_testFileName, _testFileContent);
            var adapter = new FormFileAdapter(_formFileMock.Object);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => adapter.SaveAs(null));
        }

        private void SetupFormFileMock(string fileName, byte[] content)
        {
            _formFileMock.Setup(f => f.FileName).Returns(fileName);
            _formFileMock.Setup(f => f.Length).Returns(content.Length);
            _formFileMock.Setup(f => f.CopyTo(It.IsAny<Stream>()))
                         .Callback<Stream>(stream => stream.Write(content, 0, content.Length));
        }
    }
}