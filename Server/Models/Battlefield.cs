namespace Server.Models;

using ShipsPosition = Dictionary<string, List<List<Coordinate>>>;

class Battlefield
{
    private const int FIELD_SIZE = 10;
    private enum FieldCellType { Empty, Miss, Hit, Ship }
    private ShipsPosition _shipsPosition;
    private int[] _field;

    public Battlefield(ShipsPosition shipsPosition)
    {
        _shipsPosition = shipsPosition;
        _field = Build(shipsPosition);
    }

    public int[] GetField() => _field;

    public (bool, bool) MakeMove(Coordinate coordinate)
    {
        bool isHit = false;
        bool isDestroy = false;

        switch (GetCell(coordinate.X, coordinate.Y))
        {
        case FieldCellType.Empty:
            SetCell(coordinate.X, coordinate.Y, FieldCellType.Miss);
            break; 
        case FieldCellType.Ship:
            SetCell(coordinate.X, coordinate.Y, FieldCellType.Hit);
            isHit = true;
            break;
        default:
            break;    
        }

        if (isHit)
        {
            var ship = FindShip(coordinate)!;
            isDestroy = DestroyShip(ship);
        }

        return (isHit, isDestroy);
    }

    private List<Coordinate>? FindShip(Coordinate coordinate)
    {
        foreach (var (key, ships) in _shipsPosition)
        {
            foreach (var ship in ships)
            {
                foreach (var cell in ship)
                {
                    if (cell == coordinate)
                        return ship;
                }
            }
        }

        return null;
    } 

    private bool DestroyShip(List<Coordinate> ship)
    {
        foreach (var cell in ship)
        {
            if (GetCell(cell.X, cell.Y) == FieldCellType.Ship)
                return false;
        }

        void changeEmptyCell(int x, int y)
        {
            if (GetCell(x, y) == FieldCellType.Empty)
                SetCell(x, y, FieldCellType.Miss);
        }

        foreach (var cell in ship)
        {
            changeEmptyCell(cell.X - 1, cell.Y);
            changeEmptyCell(cell.X + 1, cell.Y);
            changeEmptyCell(cell.X, cell.Y - 1);
            changeEmptyCell(cell.X, cell.Y + 1);

            changeEmptyCell(cell.X - 1, cell.Y - 1);
            changeEmptyCell(cell.X - 1, cell.Y + 1);
            changeEmptyCell(cell.X + 1, cell.Y - 1);
            changeEmptyCell(cell.X + 1, cell.Y + 1);
        }

        return true;
    }

    private FieldCellType GetCell(int x, int y)
    {
        if (OutOfRange(x, y))
            return FieldCellType.Empty;

        return (FieldCellType)_field[y * FIELD_SIZE + x];
    }

    private void SetCell(int x, int y, FieldCellType fieldCellType)
    {
        if (!OutOfRange(x, y))
            _field[y * FIELD_SIZE + x] = (int)fieldCellType;
    }

    private static bool OutOfRange(int x, int y) => (
        x < 0 || y < 0 || x >= FIELD_SIZE || y >= FIELD_SIZE
    );

    public static int[] Build(ShipsPosition shipsPosition)
    {
        int[] battlefield = new int[FIELD_SIZE * FIELD_SIZE];

        foreach (var (key, ships) in shipsPosition)
        {
            foreach (var ship in ships)
            {
                foreach (var cell in ship)
                {
                    battlefield[cell.Y * FIELD_SIZE + cell.X] = (int)FieldCellType.Ship;
                }
            }
        }

        return battlefield;
    }
}
