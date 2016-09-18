using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var maze = new Maze();
            maze.Initialize();

            FindCheese(maze);

            Console.ReadKey();
        }

        public static void FindCheese(IMaze maze)
        {
            // Implement this method
            if (CheckCheese(maze, null))
                Console.WriteLine("success!");

            Console.WriteLine("nothing found!");
        }

        private static bool CheckCheese(IMaze maze, Direction? back)
        {
            if (maze.Success()) return true;

            if (back.HasValue)
            {
                if (back != Direction.left && maze.Move(Direction.left))
                    return CheckCheese(maze, Direction.right);
                else if (back != Direction.down && maze.Move(Direction.down))
                    return CheckCheese(maze, Direction.up);
                else if (back != Direction.right && maze.Move(Direction.right))
                    return CheckCheese(maze, Direction.left);
                else if (back != Direction.up && maze.Move(Direction.up))
                    return CheckCheese(maze, Direction.down);
                else return CheckCheese(maze, back);
            }

            return false;
        }
    }

    public enum Direction
    {
        left = 1, // move left (x-1)
        down = 2, // move down (y-1)
        right = 3, // move right (x+1)
        up = 4  // move up (y+1)
    }

    public interface IMaze
    {
        // Will create a session and a maze.  The maze will be no larger than 100 by 100 units in size.  The mouse and a piece of cheese will be positioned at random locations within the maze.
        void Initialize();

        // Will attempt to move the mouse in one direction.  If the move was successful, returns true.  If there was a wall and the move failed, returns false.
        bool Move(Direction tryMovingMouseInThisDirection);

        // Will return true if the mouse and cheese are at the same location, false otherwise.
        bool Success();
    }

    public class Maze : IMaze
    {
        private enum CellType
        {
            Passage = 0,
            Wall = 1,
            Start = 2,
            Cheese = 3,
        }

        private CellType[,] _maze;

        private int currentX = -1;
        private int currentY = -1;

        public void Initialize()
        {
            int[,] maze = new int[,]
                {
                        {1, 1, 1, 1, 1, 1, 1, 1},
                        {1, 0, 0, 0, 0, 0, 0, 1},
                        {1, 0, 1, 1, 1, 1, 0, 1},
                        {1, 0, 0, 0, 3, 1, 0, 1},
                        {1, 1, 1, 1, 1, 1, 0, 1},
                        {1, 0, 0, 0, 0, 1, 0, 1},
                        {1, 0, 1, 1, 1, 1, 0, 1},
                        {1, 0, 0, 0, 0, 0, 0, 1},
                        {1, 1, 1, 1, 1, 1, 2, 1},
                };
            int yLength = maze.GetLength(0);
            int xLength = maze.GetLength(1);

            _maze = new CellType[yLength, xLength];

            for (int y = 0; y < yLength; y++)
            {
                for (int x = 0; x < xLength; x++)
                {
                    Console.Write(maze[y, x] + " ");
                    _maze[y, x] = (CellType)maze[y, x];
                    if (_maze[y, x] == CellType.Start)
                    {
                        currentY = y;
                        currentX = x;
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine(string.Format("Start position ({0},{1})", currentX + 1, currentY + 1));
        }

        public bool Move(Direction tryMovingMouseInThisDirection)
        {
            int newY = currentY;
            int newX = currentX;

            Console.Write("validate move to ");
            switch (tryMovingMouseInThisDirection)
            {
                case Direction.left:
                    newX--;
                    Console.Write("left ");
                    break;
                case Direction.down:
                    newY++;
                    Console.Write("down ");
                    break;
                case Direction.right:
                    newX++;
                    Console.Write("right ");
                    break;
                case Direction.up:
                    newY--;
                    Console.Write("up ");
                    break;
                default:
                    break;
            }

            Console.Write(string.Format("({0},{1}) : ", newX + 1, newY + 1));
            if (ValidateMove(newY, newX))
            {
                currentY = newY;
                currentX = newX;

                Console.WriteLine("OK");
                return true;
            }

            Console.WriteLine("X");

            return false;
        }

        private bool ValidateMove(int newY, int newX)
        {
            if (newY < 0 || newY >= _maze.GetLength(0)) return false;
            if (newX < 0 || newX >= _maze.GetLength(1)) return false;

            if (_maze[newY, newX] != CellType.Wall) return true;

            return false;
        }

        public bool Success()
        {
            return _maze[currentY, currentX] == CellType.Cheese;
        }
    }
}

/*
Your task is to write a function FindCheese that takes an IMaze interface (see below) as parameter and makes the mouse automatically find the cheese in the maze (labyrinth).  Once your algorithm has found the cheese, simply display the message “Success!”.

public void FindCheese(IMaze maze)
{
    // Implement this method
}

(You don’t need to implement the maze interface below.)
public interface IMaze
{
    // Will create a session and a maze.  The maze will be no larger than 100 by 100 units in size.  The mouse and a piece of cheese will be positioned at random locations within the maze.
    public void Initialize();
 
    // Will attempt to move the mouse in one direction.  If the move was successful, returns true.  If there was a wall and the move failed, returns false.
    public bool Move(Direction tryMovingMouseInThisDirection);
 
    // Will return true if the mouse and cheese are at the same location, false otherwise.
    public bool Success();
}
 
public enum Direction
{
    left = 1, // move left (x-1)
    down = 2, // move down (y-1)
    right = 3, // move right (x+1)
    up = 4  // move up (y+1)
}

You may use C#, Java, or C++ according to your personal preference.  The code should be as syntactically correct as possible but there is no requirement that it compiles (notepad is OK.)  You have 1 hour to answer this email with your proposed solution.  Enjoy!

*/
