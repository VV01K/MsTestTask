using NLog;
using System;

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
            //if (CheckCheese(maze, null, 0))
            //if (CheckCheese2(maze))
            if (CheckCheese3(maze, Direction.down, 0))
                Console.WriteLine("success!");
            else
                Console.WriteLine("nothing found!");
        }

        private const int MAX_DEPTH = 100 * 100 - 1;

        private static int[,] map = new int[198, 198];
        private static bool CheckCheese3(IMaze maze, Direction back, int depth)
        {
            if (maze.Success()) return true;

            if (depth > MAX_DEPTH / 100) return false;

            Direction forward = (Direction)(((int)back + 1) % 4 + 1);// Turnaround
            Direction left = (Direction)((int)forward % 4 + 1);
            Direction right = (Direction)((int)back % 4 + 1);

            if (maze.Move(forward))
                if (CheckCheese3(maze, back, depth++)) return true;
                else maze.Move(back);

            if (maze.Move(right))
                if (CheckCheese3(maze, left, depth++)) return true;
                else maze.Move(left);

            if (maze.Move(left))
                if (CheckCheese3(maze, right, depth++)) return true;
                else maze.Move(right);          

            return false;
        }

        // Always right (cleaner) method
        // add rooms detection
        private static bool CheckCheese2(IMaze maze)
        {
            if (maze.Success()) return true;

            Direction current = Direction.right;

            int depth = 0;

            while (true)
            {
                // 4 max Direction
                if (!maze.Move(current))
                    //current = (Direction)(((int)current + 1 > 4) ? 1 : (int)current + 1);  // Turn left
                    current = (Direction)((int)current % 4 + 1);
                else
                {
                    if (maze.Success()) return true;

                    if (++depth > MAX_DEPTH)
                    {
                        //current = (Direction)(((int)current + 2 > 4) ? 1 : (int)current + 2);  
                        current = (Direction)(((int)current + 1) % 4 + 1);// Turnaround
                        depth = 0;  // reset limiter
                    }
                    else
                    {
                        current = (Direction)(((int)current - 1 < 1) ? 4 : (int)current - 1);  // Turn right
                        //current = (Direction)(((int)current+3) % 4 + 1);
                    }
                }
            }
        }


        // Recursive one back
        // Not working for circles - depth workaround, and rooms (large free spaces)
        private static bool CheckCheese(IMaze maze, Direction? back, int depth)
        {
            if (maze.Success()) return true;

            if (depth > MAX_DEPTH) return false;

            if (!back.HasValue || back.Value != Direction.left)
                if (maze.Move(Direction.left))
                    if (CheckCheese(maze, Direction.right, depth++)) return true;
                    else maze.Move(Direction.right);

            if (!back.HasValue || back.Value != Direction.down)
                if (maze.Move(Direction.down))
                    if (CheckCheese(maze, Direction.up, depth++)) return true;
                    else maze.Move(Direction.up);

            if (!back.HasValue || back.Value != Direction.right)
                if (maze.Move(Direction.right))
                    if (CheckCheese(maze, Direction.left, depth++)) return true;
                    else maze.Move(Direction.left);

            if (!back.HasValue || back.Value != Direction.up)
                if (maze.Move(Direction.up))
                    if (CheckCheese(maze, Direction.down, depth++)) return true;
                    else maze.Move(Direction.down);

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
        // Will create a session and a maze.  
        // The maze will be no larger than 100 by 100 units in size.  
        // The mouse and a piece of cheese will be positioned at random locations within the maze.
        void Initialize();

        // Will attempt to move the mouse in one direction.  
        // If the move was successful, returns true.  
        // If there was a wall and the move failed, returns false.
        bool Move(Direction tryMovingMouseInThisDirection);

        // Will return true if the mouse and cheese are at the same location, false otherwise.
        bool Success();
    }

    public class Maze : IMaze
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
                        {1, 0, 1, 0, 0, 1, 0, 1},
                        {1, 0, 1, 3, 0, 1, 0, 1},
                        {1, 0, 1, 0, 0, 1, 0, 1},
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

            switch (tryMovingMouseInThisDirection)
            {
                case Direction.left:
                    newX--;
                    break;
                case Direction.down:
                    newY++;
                    break;
                case Direction.right:
                    newX++;
                    break;
                case Direction.up:
                    newY--;
                    break;
                default:
                    break;
            }

            const string debugFormatStr = "validate move to {0} ({1},{2}) : {3}";

            if (ValidateMove(newY, newX))
            {
                currentY = newY;
                currentX = newX;

                logger.Debug(string.Format(debugFormatStr, tryMovingMouseInThisDirection.ToString(), newX + 1, newY + 1, "Ok"));
                logger.Info(string.Format("Moved to ({0},{1})", newX + 1, newY + 1));
                return true;
            }
            logger.Debug(string.Format(debugFormatStr, tryMovingMouseInThisDirection.ToString(), newX + 1, newY + 1, "X"));

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
Your task is to write a function FindCheese that takes an IMaze interface (see below) as parameter and 
makes the mouse automatically find the cheese in the maze (labyrinth).  
Once your algorithm has found the cheese, simply display the message “Success!”.

public void FindCheese(IMaze maze)
{
    // Implement this method
}

(You don’t need to implement the maze interface below.)
public interface IMaze
{
    // Will create a session and a maze.  
    // The maze will be no larger than 100 by 100 units in size.  
    // The mouse and a piece of cheese will be positioned at random locations within the maze.
    public void Initialize();
 
    // Will attempt to move the mouse in one direction.  
    // If the move was successful, returns true.  
    // If there was a wall and the move failed, returns false.
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

You may use C#, Java, or C++ according to your personal preference.  
The code should be as syntactically correct as possible but there is no requirement that it compiles (notepad is OK.)  
You have 1 hour to answer this email with your proposed solution.  Enjoy!

*/
