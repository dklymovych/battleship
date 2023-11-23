using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Client.MVVM.Model;

public class Square : INotifyPropertyChanged
{
    public int X { get; set; }
    public int Y { get; set; }

    private SolidColorBrush _color;
    public bool isShip { get ; set; }
    public bool isCloseToShip { get; set; }

   public SolidColorBrush Color
    {
        get { return _color; }
        set
        {
            _color = value;
            OnPropertyChanged();
        }
    }

    public override string ToString()
    {
        return $"x{X}y{Y}";
    }

    public Square(int x, int y)
    {
        X = x;
        Y = y;
        isShip = false;
        isCloseToShip = false;
        Color = new SolidColorBrush(Colors.Gray); // Початковий колір
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Метод для зміни кольору
    public void ChangeColor()
    {
        if (isCloseToShip)
        {
            if (isShip)
            {
                Color = new SolidColorBrush(Colors.MidnightBlue);
            }
            else
            {
                Color = new SolidColorBrush(Colors.DimGray);
            }
        }
        else
        {
            Color = new SolidColorBrush(Colors.Gray);
        }
        // Color = isShip ? new SolidColorBrush(Colors.Blue) : new SolidColorBrush(Colors.Gray);
        // Color = new SolidColorBrush(Colors.Black);
    }

}