using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public enum PlayerOption
{
    NONE, //0
    X, // 1
    O // 2
}

public class TTT : MonoBehaviour
{
    public int Rows;
    public int Columns;
    [SerializeField] BoardView board;

    PlayerOption currentPlayer = PlayerOption.X;
    Cell[,] cells;

    //My variables
    [SerializeField] int sequence = 0;
    bool adjacent = false;
    int adjacentRow;
    int adjacentCol;
    bool noBlock;
    [SerializeField] bool hiddenThreat;

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[Columns, Rows];

        board.InitializeBoard(Columns, Rows);

        for(int i = 0; i < Rows; i++)
        {
            for(int j = 0; j < Columns; j++)
            {
                cells[j, i] = new Cell();
                cells[j, i].current = PlayerOption.NONE;
            }
        }
    }

    public void MakeOptimalMove()
    {
        switch (sequence)
        {
            case 0:
                InitialMove();
                sequence++;
                break;
            case 1:
                SecondMove();
                sequence++;
                break;
            case 2:
                Adjacent();
                sequence++;
                break;
            case 3:
                BlockWin();
                break;
        }
    }

    public void InitialMove()
    {
        int emptyAmount = 0;
        int[] corners = { 0, 2 };
        int randomIndex = Random.Range(0, corners.Length);
        int randomIndex2 = Random.Range(0, corners.Length);

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (cells[j, i].current == PlayerOption.NONE)
                    emptyAmount++;
            }
        }

        // Take corner if board is empty
        if (emptyAmount == 9)
        {
            ChooseSpace(corners[randomIndex], corners[randomIndex2]);
            adjacent = true;
            adjacentRow = corners[randomIndex];
            adjacentCol = corners[randomIndex2];
        }
        else
        {
            SecondMove();
        }
    }

    public void SecondMove()
    {
        Debug.Log("SecondMove called");
        int corners = 2;
        int emptyAmount = 0;

        for (int i = 0; i < corners + 1; i++)
        {
            if (i == 1) continue;
            for (int j = 0; j < corners + 1; j++)
            {
                if (j == 1) continue;
                if (cells[j, i].current == PlayerOption.NONE)
                    emptyAmount++;
            }
        }

        //Take center
        if ((emptyAmount < 4) && (cells[1, 1].current == PlayerOption.NONE))
        {
            ChooseSpace(1, 1);
        }
        //Take corner or safe
        else //(merge into 1?)
        {
            BlockWin();
            if (hiddenThreat == true)
            {
                Safe();
            }
            if (noBlock == true)
            {
                Debug.Log("noBlock");
                int[] corner = { 0, 2 };
                int randomIndex = Random.Range(0, corner.Length);
                int randomIndex2 = Random.Range(0, corner.Length);

                ChooseSpace(corner[randomIndex], corner[randomIndex2]);
            }
        }
    }

    public void Adjacent()
    {
        if (!hiddenThreat)
        {
            BlockWin();
        }
        //BlockWin();
        Debug.Log("Adjacent called");
        if (adjacent)
        {
            //If top left corner is owned
            if (adjacentRow == 0 && adjacentCol == 0)
            {
                if ((cells[0, 1].current == PlayerOption.NONE) && (cells[0, 1].current == PlayerOption.NONE))
                {
                    int[] possibilities = { -1, 1 };
                    int randomIndex = Random.Range(0, possibilities.Length);
                    
                    if (possibilities[randomIndex] == 1)
                    {
                        ChooseSpace(0, 1);
                    }
                    else
                    {
                        ChooseSpace(1, 0);
                    }
                }
                else if (cells[0, 1].current == PlayerOption.NONE)
                {
                    ChooseSpace(0, 1);
                }
                else
                {
                    ChooseSpace(1, 0);
                }
            }

            //If top right corner is owned
            if (adjacentRow == 0 && adjacentCol == 2)
            {
                if ((cells[0, 1].current == PlayerOption.NONE) && (cells[1, 2].current == PlayerOption.NONE))
                {
                    int[] possibilities = { -1, 1 };
                    int randomIndex = Random.Range(0, possibilities.Length);

                    if (possibilities[randomIndex] == 1)
                    {
                        ChooseSpace(0, 1);
                    }
                    else
                    {
                        ChooseSpace(1, 2);
                    }
                }
                else if (cells[0, 1].current == PlayerOption.NONE)
                {
                    ChooseSpace(0, 1);
                }
                else
                {
                    ChooseSpace(1, 2);
                }
            }

            //If bottom left corner is owned
            if (adjacentRow == 2 && adjacentCol == 0)
            {
                if ((cells[1, 0].current == PlayerOption.NONE) && (cells[2, 1].current == PlayerOption.NONE))
                {
                    int[] possibilities = { -1, 1 };
                    int randomIndex = Random.Range(0, possibilities.Length);

                    if (possibilities[randomIndex] == 1)
                    {
                        ChooseSpace(1, 0);
                    }
                    else
                    {
                        ChooseSpace(2, 1);
                    }
                }
                else if (cells[1, 0].current == PlayerOption.NONE)
                {
                    ChooseSpace(1, 0);
                }
                else
                {
                    ChooseSpace(2, 1);
                }
            }

            //If bottom right corner is owned
            if (adjacentRow == 2 && adjacentCol == 2)
            {
                if ((cells[2, 1].current == PlayerOption.NONE) && (cells[1, 2].current == PlayerOption.NONE))
                {
                    int[] possibilities = { -1, 1 };
                    int randomIndex = Random.Range(0, possibilities.Length);

                    if (possibilities[randomIndex] == 1)
                    {
                        ChooseSpace(2, 1);
                    }
                    else
                    {
                        ChooseSpace(1, 2);
                    }
                }
                else if (cells[2, 1].current == PlayerOption.NONE)
                {
                    ChooseSpace(2, 1);
                }
                else
                {
                    ChooseSpace(1, 2);
                }
            }
        }
    }

    public void BlockWin()
    {
        Debug.Log("BlockWin called");
        int sum = 0;
        int rowNone;
        int colNone;
        int none;
        bool checkColumns = true;
        bool checkFirstDiagonal = true;
        bool checkSecondDiagonal = true;
        noBlock = false;

        hiddenThreat = false;
        int cornerTotal;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            rowNone = 0;
            colNone = 0;

            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[i, j].current == PlayerOption.X)
                    value = 1;
                else if (cells[i, j].current == PlayerOption.O)
                    value = -1;
                else if (cells[i, j].current == PlayerOption.NONE)
                {
                    value = 0;
                    rowNone = i; 
                    colNone = j;
                }

                sum += value;
            }

            if (sum == 2 || sum == -2)
            {
                ChooseSpace(rowNone, colNone);
                checkColumns = false;
                checkFirstDiagonal = false;
                checkSecondDiagonal = false;
                adjacent = false;
                Debug.Log(sequence);
                break;
            }
        }

        // check columns
        if (checkColumns)
        {
            for (int j = 0; j < Columns; j++)
            {
                sum = 0;
                rowNone = 0;
                colNone = 0;

                for (int i = 0; i < Rows; i++)
                {
                    var value = 0;
                    if (cells[i, j].current == PlayerOption.X)
                        value = 1;
                    else if (cells[i, j].current == PlayerOption.O)
                        value = -1;
                    else if (cells[i, j].current == PlayerOption.NONE)
                    {
                        value = 0;
                        rowNone = i;
                        colNone = j;
                    }

                    sum += value;
                }

                if (sum == 2 || sum == -2)
                {
                    ChooseSpace(rowNone, colNone);
                    checkFirstDiagonal = false;
                    checkSecondDiagonal = false;
                    adjacent = false;
                    Debug.Log(sequence);
                    break;
                }
            }
        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        none = 0;
        cornerTotal = 0;

        if (checkFirstDiagonal)
        {
            for (int i = 0; i < Rows; i++)
            {
                int value = 0;
                cornerTotal = 0;
                if (cells[i, i].current == PlayerOption.X)
                {
                    value = 1;
                    cornerTotal++;
                }
                else if (cells[i, i].current == PlayerOption.O)
                {
                    value = -1;
                    cornerTotal++;
                }
                else if (cells[i, i].current == PlayerOption.NONE)
                {
                    value = 0;
                    none = i;
                }

                sum += value;
            }

            if (cornerTotal == 3)
            {
                hiddenThreat = true;
                //Adjacent();
            }
            else if (sum == 2 || sum == -2)
            {
                ChooseSpace(none, none);
                adjacent = false;
                Debug.Log(sequence);
                checkSecondDiagonal = false;
            }
        }
        

        // top right to bottom left
        if (checkSecondDiagonal)
        {
            sum = 0;
            none = 0;
            cornerTotal = 0;

            for (int i = 0; i < Rows; i++)
            {
                int value = 0;

                if (cells[i, Columns - 1 - i].current == PlayerOption.X)
                {
                    value = 1;
                    cornerTotal++;
                }
                else if (cells[i, Columns - 1 - i].current == PlayerOption.O)
                {
                    value = -1;
                    cornerTotal++;
                }
                else if (cells[i, Columns - 1 - i].current == PlayerOption.NONE)
                {
                    value = 0;
                    none = i;
                }

                sum += value;
            }

            if (cornerTotal == 3)
            {
                Debug.Log(cornerTotal);
                hiddenThreat = true;
                //Adjacent();
            }
            else if (sum == 2 || sum == -2)
            {
                ChooseSpace(none, Columns - 1 - none);
                adjacent = false;
                Debug.Log(sequence);
                Debug.Log("Second Diagonal Checked");
            }
            else
            {
                noBlock = true;
            }
        }
    }

    public void Safe() //check why this is not being called
    {
        bool valid = false;

        int randomIndex = Random.Range(0, 2);
        int randomIndex2 = Random.Range(0, 2);

        Debug.Log("[" + randomIndex + "," +  randomIndex2 + "]");
        if (cells[randomIndex, randomIndex2].current == PlayerOption.NONE)
        {
            valid = true;
            ChooseSpace(randomIndex, randomIndex2);
        }
        else
        {
            Safe();
        }
    }

    public void ChooseSpace(int column, int row)
    {
        // can't choose space if game is over
        if (GetWinner() != PlayerOption.NONE)
            return;

        // can't choose a space that's already taken
        if (cells[column, row].current != PlayerOption.NONE)
            return;

        // set the cell to the player's mark
        cells[column, row].current = currentPlayer;

        // update the visual to display X or O
        board.UpdateCellVisual(column, row, currentPlayer);

        // if there's no winner, keep playing, otherwise end the game
        if(GetWinner() == PlayerOption.NONE)
            EndTurn();
        else
        {
            Debug.Log("GAME OVER!");
        }
    }

    public void EndTurn()
    {
        // increment player, if it goes over player 2, loop back to player 1
        currentPlayer += 1;
        if ((int)currentPlayer > 2)
            currentPlayer = PlayerOption.X;
    }

    public PlayerOption GetWinner()
    {
        // sum each row/column based on what's in each cell X = 1, O = -1, blank = 0
        // we have a winner if the sum = 3 (X) or -3 (O)
        int sum = 0;

        // check rows
        for (int i = 0; i < Rows; i++)
        {
            sum = 0;
            for (int j = 0; j < Columns; j++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check columns
        for (int j = 0; j < Columns; j++)
        {
            sum = 0;
            for (int i = 0; i < Rows; i++)
            {
                var value = 0;
                if (cells[j, i].current == PlayerOption.X)
                    value = 1;
                else if (cells[j, i].current == PlayerOption.O)
                    value = -1;

                sum += value;
            }

            if (sum == 3)
                return PlayerOption.X;
            else if (sum == -3)
                return PlayerOption.O;

        }

        // check diagonals
        // top left to bottom right
        sum = 0;
        for(int i = 0; i < Rows; i++)
        {
            int value = 0;
            if (cells[i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        // top right to bottom left
        sum = 0;
        for (int i = 0; i < Rows; i++)
        {
            int value = 0;

            if (cells[Columns - 1 - i, i].current == PlayerOption.X)
                value = 1;
            else if (cells[Columns - 1 - i, i].current == PlayerOption.O)
                value = -1;

            sum += value;
        }

        if (sum == 3)
            return PlayerOption.X;
        else if (sum == -3)
            return PlayerOption.O;

        return PlayerOption.NONE;
    }
}
