using Client.Services;
using System.Globalization;
using System.Windows.Media;

namespace ClientTests
{
    public class Services_BackgroundColorConverter
    {
        [Fact]
        public void ConvertBack_AlwaysThrowsNotSupportedException()
        {
            var converter = new BackgroundColorConverter();

            Assert.Throws<NotSupportedException>(() =>
                converter.ConvertBack(null, null, null, CultureInfo.InvariantCulture));
        }
    }
}