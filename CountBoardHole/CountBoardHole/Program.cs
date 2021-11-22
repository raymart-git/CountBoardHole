using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountBoardHole
{
    class Program
    {
        public const int iCount = 10;
        public const int jCount = 10;

        static bool previousIsZero = false;
        static string parentIndexZero = "";

        private static List<string> _lstOfAddedPairIndexes = new List<string>();
        static Dictionary<string, string> _dictZeroRowsConnection = new Dictionary<string, string>();
        static Dictionary<string, string> _dictZeroRowsConnectionClone = new Dictionary<string, string>();
        static List<string> _updatedParent = new List<string>();
        static List<string> _pendingToAddParent = new List<string>();

        static List<int> _holeCount = new List<int>();

        static void Main(string[] args)
        {
            // create grid
            var grid = new int[iCount, jCount] {
                   { 1, 1, 0, 0, 0, 1, 1, 1, 1, 0},
                   { 1, 1, 1, 1, 0, 1, 1, 1, 1, 0},
                   { 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
                   { 1, 0, 0, 1, 0, 0, 1, 1, 1, 0},
                   { 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
                   { 1, 1, 1, 1, 0, 1, 1, 1, 1, 0},
                   { 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
                   { 1, 1, 0, 0, 1, 1, 1, 1, 1, 0},
                   { 1, 1, 0, 0, 1, 1, 1, 1, 1, 1},
                   { 1, 1, 0, 0, 1, 1, 1, 1, 1, 1}
            };

            for (int i = 0; i < 10; i++) // x
            {
                for (int j = 0; j < 10; j++) //y
                {
                    if (grid[i, j] == 0)
                    {
                        string pairIndexs = string.Format("{0},{1}", i, j);
                        _lstOfAddedPairIndexes.Add(pairIndexs);

                        if (!previousIsZero) //possible parent index
                        {
                            if (!_dictZeroRowsConnection.ContainsKey(pairIndexs))
                            {
                                if (i == 0) // loop still on topmost
                                {
                                    parentIndexZero = pairIndexs;
                                    _dictZeroRowsConnection.Add(pairIndexs, parentIndexZero);
                                }
                                else
                                {
                                    if (CanMoveUp(pairIndexs, grid))
                                    {
                                        parentIndexZero = pairIndexs;
                                        _dictZeroRowsConnection.Add(pairIndexs, parentIndexZero);
                                    }
                                    else
                                    {
                                        if (CanMoveRight(pairIndexs, grid))
                                        {
                                            parentIndexZero = pairIndexs;
                                            _pendingToAddParent.Add(pairIndexs);
                                        }
                                        else
                                        {
                                            parentIndexZero = pairIndexs;
                                            _dictZeroRowsConnection.Add(pairIndexs, parentIndexZero);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (i == 0)
                            {
                                _dictZeroRowsConnection.Add(pairIndexs, parentIndexZero);
                            }
                            else
                            {
                                if (CanMoveUp(pairIndexs, grid))
                                {
                                    if (!_dictZeroRowsConnection.ContainsKey(parentIndexZero)) // set parent if previous is zero but can't go up
                                    {
                                        parentIndexZero = pairIndexs;
                                    }

                                    if (!_dictZeroRowsConnection.ContainsKey(pairIndexs))
                                    {
                                        _dictZeroRowsConnection.Add(pairIndexs, parentIndexZero);
                                    }
                                }
                                else
                                {
                                    if (CanMoveRight(pairIndexs, grid))
                                    {
                                        _pendingToAddParent.Add(pairIndexs);
                                    }
                                    else
                                    {
                                        if (!_dictZeroRowsConnection.ContainsKey(pairIndexs))
                                        {
                                            _dictZeroRowsConnection.Add(pairIndexs, parentIndexZero);
                                        }
                                    }
                                }

                                // add coordinates with pending parents
                                foreach (var s in _pendingToAddParent)
                                {
                                    if (!_dictZeroRowsConnection.ContainsKey(s))
                                    {
                                        _dictZeroRowsConnection.Add(s, parentIndexZero);
                                    }
                                }
                                _pendingToAddParent.Clear();
                            }
                        }
                        previousIsZero = true;
                    }
                    else
                    {
                        previousIsZero = false;
                    }
                }

                foreach (var s in _pendingToAddParent)
                {
                    if (!_dictZeroRowsConnection.ContainsKey(s))
                    {
                        _dictZeroRowsConnection.Add(s, parentIndexZero);
                    }
                }
                _pendingToAddParent.Clear();

            }

            var fragmentParentPair = _dictZeroRowsConnection.Values.Distinct().ToList();
            _updatedParent.AddRange(fragmentParentPair);
            _dictZeroRowsConnectionClone = new Dictionary<string, string>(_dictZeroRowsConnection);
            MainRecursive(fragmentParentPair, _dictZeroRowsConnection);

            // print grid
            PrintGrid(grid);

            // print holeCount
            string holeCount = string.Join(",", _holeCount.Select(s => s));
            Console.WriteLine("HoleCount : " + holeCount);
            Console.ReadLine();
        }

        static void MainRecursive(List<string> fragmentParentPair, Dictionary<string, string> dicZero)
        {
            int connectionCount = 0;
            if (_updatedParent.Count == 0) return;
            foreach (var startrow in fragmentParentPair)
            {
                if (_updatedParent.Count == 0) break;

                string endrow = "";

                int itr = 0;
                string minRowVal = "";
                string maxRowVal = "";
                foreach (var c in dicZero.Where(n => n.Value == startrow).OrderBy(n => n.Key))
                {
                    itr++;
                    string[] splitShit = c.Key.Split(',');
                    if (itr == 1)
                    {
                        minRowVal = splitShit[1];
                    }
                    maxRowVal = splitShit[1];

                    endrow = c.Key;

                    _dictZeroRowsConnectionClone.Remove(c.Key);
                    _updatedParent.Remove(c.Value);
                    connectionCount++;
                }


                string[] splitStart = startrow.Split(',');
                string[] splitEnd = endrow.Split(',');
                int fromX = Convert.ToInt32(splitStart[0]);
                int toX = Convert.ToInt32(splitEnd[1]);

                Dictionary<string, string> newDict = new Dictionary<string, string>(_dictZeroRowsConnectionClone);
                SubRecursive(fromX, Convert.ToInt32(minRowVal), Convert.ToInt32(maxRowVal), connectionCount, newDict);
            }
        }

        static bool SubRecursive(int y, int fromX, int toX, int connectionCount, Dictionary<string, string> dictZeroRowsConnection)
        {
            y = y + 1;
            string parent = "";

            foreach (var c in dictZeroRowsConnection.Where(n => n.Value.StartsWith(y.ToString())).OrderBy(n => n.Key))
            {
                if (fromX == toX) // range is equal (minimum previousX to maximum previousX)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        string[] splitConnectedPair = c.Key.Split(',');
                        int conPairX = Convert.ToInt32(splitConnectedPair[0]);
                        int conPairY = Convert.ToInt32(splitConnectedPair[1]);
                        if (conPairY == toX)
                        {
                            if (_updatedParent.Any(n => n == c.Key))
                            {
                                parent = c.Key;
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = fromX; i <= toX; i++)
                    {
                        string[] splitConnectedPair = c.Key.Split(',');
                        int conPairX = Convert.ToInt32(splitConnectedPair[0]);
                        int conPairY = Convert.ToInt32(splitConnectedPair[1]);
                        if (conPairY == i)
                        {
                            if (_updatedParent.Any(n => n == c.Key))
                            {
                                parent = c.Key;
                                break;
                            }
                        }
                    }
                }
            }

            if (!String.IsNullOrEmpty(parent))
            {
                string minRowVal = "";
                string maxRowVal = "";
                int itr = 0;
                foreach (var zrc in dictZeroRowsConnection.Where(n => n.Value == parent).OrderBy(n => n.Key))
                {
                    itr++;
                    string[] splitZRC = zrc.Key.Split(',');

                    if (itr == 1) // first loop
                    {
                        minRowVal = splitZRC[1];
                    }
                    maxRowVal = splitZRC[1];
                    _updatedParent.Remove(zrc.Value);

                    foreach (var r in _dictZeroRowsConnectionClone.Where(kvp => kvp.Value == zrc.Value).ToList())
                    {
                        _dictZeroRowsConnectionClone.Remove(r.Key);
                    }

                    connectionCount++;
                }

                if (String.IsNullOrEmpty(minRowVal) && String.IsNullOrEmpty(maxRowVal)) return false;
                SubRecursive(y, Convert.ToInt32(minRowVal), Convert.ToInt32(maxRowVal), connectionCount, new Dictionary<string, string>(_dictZeroRowsConnectionClone));
            }
            else
            {
                List<string> newList = new List<string>();
                newList.AddRange(_updatedParent);
                _holeCount.Add(connectionCount); // Add Hole Count

                if (_updatedParent.Count > 0)
                {
                    MainRecursive(newList, new Dictionary<string, string>(_dictZeroRowsConnectionClone));
                }
            }

            return false;

        }

        /// <summary>
        /// Can Move Up and Value Number on Top is 0
        /// </summary>
        /// <param name="pairIndexes"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        static bool CanMoveUp(string pairIndexes, int[,] grid)
        {
            string[] splitIndex = pairIndexes.Split(',');
            int x = Convert.ToInt32(splitIndex[0]);
            int y = Convert.ToInt32(splitIndex[1]);

            if ((x - 1) >= 0)
            {
                if (grid[x - 1, y] == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return (x - 1) >= 0;
        }

        /// <summary>
        /// Can Move Right and Value Number on Right is 0
        /// </summary>
        /// <param name="pairIndexes"></param>
        /// <param name="grid"></param>
        /// <returns></returns>
        static bool CanMoveRight(string pairIndexes, int[,] grid)
        {
            string[] splitIndex = pairIndexes.Split(',');
            int x = Convert.ToInt32(splitIndex[0]);
            int y = Convert.ToInt32(splitIndex[1]);

            if ((y + 1) <= 9)
            {
                if (grid[x, y + 1] == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return (y + 1) >= 9;
        }


        public static void PrintGrid(int[,] grid)
        {
            Console.WriteLine("+----------+");
            for (int i = 0; i < iCount; i++)
            {
                for (int j = 0; j < jCount; j++)
                {
                    if (j == 0) Console.Write("|");
                    Console.Write(grid[i, j].ToString());
                    if (j == jCount - 1)
                    {
                        Console.Write("|");
                        Console.Write(System.Environment.NewLine);
                    }

                }
            }
            Console.WriteLine("+----------+");
        }

    }
}
