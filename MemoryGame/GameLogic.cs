using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MemoryGame
{
    class GameLogic
    {
        int colsAndRows, matchAttemptsCounter;
        bool checkingFirst = true;
        Tile tile;
        Tile firstCandidate = null;
        Tile secondCandidate;
        List<int> matchKeys = new List<int>();
        Tile[] foundArray = new Tile[2];

        public GameLogic(int colsAndRows)
        {
            this.colsAndRows = colsAndRows;
        }

        public bool ClickedFirst(Tile[,] tile, Button tileClicked)
        {
            if (GetCheckingFirst())
            {
                SwitchChecking(GetCheckingFirst());
                firstCandidate = GetOriginalTile(tile, tileClicked);
                SetCandidates(firstCandidate, null);
                return true;
            }
            else
                return false;
        }

        public bool ClickedSecond(Tile[,] tile, Button tileClicked)
        {
            bool result = false;
            SwitchChecking(GetCheckingFirst());
            secondCandidate = GetOriginalTile(tile, tileClicked);
            foundArray = GetCandidates();

            if (IsImageMatch(firstCandidate, secondCandidate))
                result = true;
            else
                result = false;

            matchAttemptsCounter++;
            SetCandidates(firstCandidate, secondCandidate);
            NullCandidates();
            return result;
        }

        public Tile GetOriginalTile(Tile[,] tile, Button tileClicked)
        {
            for (int i = 0; i < colsAndRows; i++)
                for (int j = 0; j < colsAndRows; j++)
                    if (tileClicked == tile[i, j].tileButton)
                        this.tile = tile[i, j];
            return this.tile;
        }

        public bool IsImageMatch(Tile first, Tile second)
        {
            return (first.GetMatchKey() == second.GetMatchKey());
        }

        public int AssignMatchKey()
        {
            Random rng = new Random();
            int x;

            do
            {
                x = rng.Next();
            } while (matchKeys.Contains(x));
            matchKeys.Add(x);

            return x;
        }

        public Tile[] GetCandidates()
        {
            return foundArray;
        }

        public void SetCandidates(Tile first, Tile second)
        {
            foundArray[0] = first;
            foundArray[1] = second;
        }

        public void NullCandidates()
        {
            firstCandidate = null;
            secondCandidate = null;
        }

        private void SwitchChecking(bool first)
        {
            SetCheckingFirst(!first);
        }

        public void SetCheckingFirst(bool status)
        {
            checkingFirst = status;
        }

        public bool GetCheckingFirst()
        {
            return checkingFirst;
        }
        
        public int GetMatchAttemptsCounter()
        {
            return matchAttemptsCounter;
        }

        public void SetMatchAttemptsCounter(int value)
        {
            matchAttemptsCounter = value;
        }
    }
}
