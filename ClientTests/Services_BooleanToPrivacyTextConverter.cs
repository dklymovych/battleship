using Client.Services;
using System.Globalization;
using System.Windows.Media;

namespace ClientTests
{
    public class Services_BooleanToPrivacyTextConverter
    {
        [Theory]
        [InlineData(true, "Private")]
        [InlineData(false, "Public")]
        public void Convert_ReturnsExpectedResult(bool value, string expectedText)
        {
            // Arrange
            var converter = new BooleanToPrivacyTextConverter();

            // Act
            var result = converter.Convert(value, null, null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(expectedText, result);
        }

        [Fact]
        public void ConvertBack_AlwaysThrowsNotImplementedException()
        {
            // Arrange
            var converter = new BooleanToPrivacyTextConverter();

            // Act & Assert
            Assert.Throws<NotImplementedException>(() =>
                converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture));
        }
    }
}