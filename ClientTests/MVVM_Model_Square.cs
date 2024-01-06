using Client.MVVM.Model;
using System.Windows.Media;

namespace ClientTests
{
    public class MVVM_Model_Square
    {
        [Fact]
        public void Square_ConstructedWithCoordinates_HasCorrectCoordinates()
        {
            int x = 2;
            int y = 3;

            var square = new Square(x, y);

            Assert.Equal(x, square.X);
            Assert.Equal(y, square.Y);
        }

        [Fact]
        public void Square_WhenCreated_IsNotShipAndNotCloseToShip()
        {
            var square = new Square(0, 0);

            Assert.False(square.isShip);
            Assert.False(square.isCloseToShip);
        }

        [Fact]
        public void Square_WhenColorChanged_IsCorrectColor()
        {
            var square = new Square(0, 0);

            square.ChangeColor();

            Assert.Equal(Colors.Gray, square.Color.Color);
        }

        [Fact]
        public void Square_WhenCloseToShipAndNotShip_ColorIsDimGray()
        {
            var square = new Square(0, 0);
            square.isCloseToShip = true;

            square.ChangeColor();

            Assert.Equal(Colors.DimGray, square.Color.Color);
        }

        [Fact]
        public void Square_WhenCloseToShipAndIsShip_ColorIsMidnightBlue()
        {
            var square = new Square(0, 0);
            square.isCloseToShip = true;
            square.isShip = true;

            square.ChangeColor();

            Assert.Equal(Colors.MidnightBlue, square.Color.Color);
        }
    }
}