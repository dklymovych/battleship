using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace Client.MVVM.Model;



public class BattlefieldSquare : INotifyPropertyChanged
{
    public int X { get; set; }
    public int Y { get; set; }
    private int _value;
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
            Color = ValueToColor(_value);
            OnPropertyChanged("Value");
        }
    }

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

    public BattlefieldSquare(int x, int y, int value)
    {
        X = x;
        Y = y;
        Value = value;  // Set the initial value
        isShip = false;
        isCloseToShip = false;
        // Initialize Color based on the initial value
        Color = ValueToColor(value); 
    }
    private SolidColorBrush ValueToColor(int value)
    {
        switch (value)
        {
            case 0: return new SolidColorBrush(ColorFromHex("#696969"));
            case 1: return new SolidColorBrush(ColorFromHex("#727272"));
            case 2: return new SolidColorBrush(ColorFromHex("#F95454"));
            case 3: return new SolidColorBrush(ColorFromHex("#2C3F66"));
            default: return new SolidColorBrush(Colors.Black);
        }
    }
    private Color ColorFromHex(string hex)
    {
        // Convert hex to a Color
        return (Color)ColorConverter.ConvertFromString(hex);
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