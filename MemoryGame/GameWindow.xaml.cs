using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MemoryGame
{
    public partial class GameWindow : Window
    {
        int colsAndRows;
        const int AREAMARGIN = 70, TILEMARGIN = 30, WINDOWSIZE = 600, FOOTERSIZE = 30;
        TimeSpan timeDifference;
        Tile[] candidates;
        Tile[,] tile;
        List<Tile[]> matchedPairs;
        Grid playAreaGrid;
        Label elapsedLabel, triesLabel;
        GameLogic logic;
        DispatcherTimer compareImagesTimer;
        DispatcherTimer elapsedTimer;
        DateTime startTime;

        public GameWindow()
        {
            InitializeComponent();
        }
        
        private void PrepareGameField()
        {
            logic = new GameLogic(colsAndRows);
            matchedPairs = new List<Tile[]>();
            ImageList.AddImagesToList();
            CreateGrid();
            CreateTiles();
            AssignImages();
            CompareTimerConfig();
            StatsFooterConfig();
        }

        private void CreateGrid()
        {
            sizeSelectionGrid.Visibility = Visibility.Hidden;

            Height = WINDOWSIZE + FOOTERSIZE;
            Width = WINDOWSIZE;

            CenterOnScreen();
            playAreaGrid = new Grid
            {
                Name = "playAreaGrid",
                Height = Height - AREAMARGIN,
                Width = Width - AREAMARGIN,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            RowDefinition[] rowDef = new RowDefinition[colsAndRows+1];
            for (int i = 0; i < colsAndRows+1; i++)
            {
                rowDef[i] = new RowDefinition();
                if(i < colsAndRows)
                {
                    playAreaGrid.ColumnDefinitions.Add(new ColumnDefinition());
                    playAreaGrid.RowDefinitions.Add(rowDef[i]);
                }
                else
                {
                    rowDef[i].Height = new GridLength(FOOTERSIZE);
                    playAreaGrid.RowDefinitions.Add(rowDef[i]);
                }

            }

            Content = playAreaGrid;
        }

        private void CenterOnScreen()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;
            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private void CreateTiles()
        {
            tile = new Tile[colsAndRows, colsAndRows];

            for (int i = 0; i < colsAndRows; i++)
                for (int j = 0; j < colsAndRows; j++)
                {
                    tile[i, j] = new Tile();
                    TileButtonConfiguration(tile[i, j], i, j);
                }
        }
        
        private void TileButtonConfiguration(Tile tile, int row, int column)
        {
            tile.tileButton.Tag = row.ToString() + column.ToString();

            tile.tileButton.Width = WINDOWSIZE / colsAndRows - TILEMARGIN;
            tile.tileButton.Height = WINDOWSIZE / colsAndRows - TILEMARGIN;
            Grid.SetRow(tile.tileButton, row);
            Grid.SetColumn(tile.tileButton, column);
            tile.tileButton.Click += new RoutedEventHandler(tileButton_Click);
            tile.ImageHidden();
            
            playAreaGrid.Children.Add(tile.tileButton);

            Console.WriteLine("created i: {0}, j: {1}", row, column);
        }

        private void AssignImages()
        {
            Random rng = new Random();
            int randomImageNumber, x, y;
            
            for (int i = 0; i < (colsAndRows*colsAndRows)/2; i++)
            {
                randomImageNumber = rng.Next(ImageList.GetImageList().Count);
                string imagePath = ImageList.GetImage(randomImageNumber);
                int matchKey = logic.AssignMatchKey();
                do
                {
                    x = rng.Next(colsAndRows);
                    y = rng.Next(colsAndRows);
                } while (tile[x, y].IsOccupied());

                tile[x, y].SetTileImage(imagePath);
                tile[x, y].SetMatchKey(matchKey);

                do
                {
                    x = rng.Next(colsAndRows);
                    y = rng.Next(colsAndRows);
                } while (tile[x, y].IsOccupied());

                tile[x, y].SetTileImage(imagePath);
                tile[x, y].SetMatchKey(matchKey);

                ImageList.DeleteImageFromList(randomImageNumber);
                Console.WriteLine(ImageList.GetImageList().Count);
            }
        }

        private void CompareTimerConfig()
        {
            compareImagesTimer = new DispatcherTimer();
            compareImagesTimer.Tick += new EventHandler(CompareMismatched);
            compareImagesTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
        }

        private void StatsFooterConfig()
        {
            elapsedLabel = new Label();
            elapsedLabel.Content = "Time elapsed: ";
            Grid.SetRow(elapsedLabel, colsAndRows);
            Grid.SetColumnSpan(elapsedLabel, 2);
            playAreaGrid.Children.Add(elapsedLabel);

            triesLabel = new Label();
            Grid.SetRow(triesLabel, colsAndRows);
            Grid.SetColumn(triesLabel, 3);
            playAreaGrid.Children.Add(triesLabel);

            elapsedTimer = new DispatcherTimer();
            elapsedTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            elapsedTimer.Tick += new EventHandler(ElapsedTime);
            startTime = DateTime.Now;
            elapsedTimer.Start();
        }

        private void sizeSelection_Click(object sender, RoutedEventArgs e)
        {
            colsAndRows = Int32.Parse(((Button)sender).Tag.ToString());
            PrepareGameField();
        }

        private void tileButton_Click(object sender, RoutedEventArgs e)
        {
            Button tileClicked = (Button)sender;
            Tile tile = logic.GetOriginalTile(this.tile, tileClicked);

            tile.tileButton.BorderBrush = Brushes.Lime;
            tile.tileButton.BorderThickness = new Thickness(3);

            tile.ImageVisible();
            if (logic.ClickedFirst(this.tile, tileClicked))
            {
                candidates = logic.GetCandidates();
                candidates[0].tileButton.IsHitTestVisible = false;
            }
            else if (logic.ClickedSecond(this.tile, tileClicked))
            {
                StatsUpdate();
                candidates = logic.GetCandidates();
                for (int i = 0; i < candidates.Length; i++)
                    candidates[i].tileButton.IsHitTestVisible = false;

                matchedPairs.Add(candidates);
                if (matchedPairs.Count == (colsAndRows * colsAndRows) / 2)
                {
                    elapsedTimer.Stop();
                    MessageBoxResult option = MessageBox.Show(
                        "You won the game! Your time: " + timeDifference.Minutes + "m " + timeDifference.Seconds + "s, tries: " 
                        + logic.GetMatchAttemptsCounter().ToString() + "\n\nDo you want to play again?",
                        "Winner",
                        MessageBoxButton.YesNo);
                    switch(option)
                    {
                        case MessageBoxResult.Yes:
                            Restart();
                            break;
                        case MessageBoxResult.No:
                            Close();
                            break;
                    }
                }
            }
            else
            {
                StatsUpdate();
                candidates = logic.GetCandidates();
                candidates[0].tileButton.IsHitTestVisible = true;
                playAreaGrid.IsHitTestVisible = false;
                compareImagesTimer.Start();
            }
        }

        private void Restart()
        {
            triesLabel.Content = " ";
            elapsedLabel.Content = " ";
            PrepareGameField();
        }

        private void StatsUpdate()
        {
            triesLabel.Content = "Tries: " + logic.GetMatchAttemptsCounter().ToString();
        }

        private void ElapsedTime(object sender, EventArgs e)
        {
            timeDifference = DateTime.Now.Subtract(startTime);
            elapsedLabel.Content = "Time elapsed: " + timeDifference.Minutes.ToString() + "m " + timeDifference.Seconds.ToString() + "s";
        }

        private void CompareMismatched(object sender, EventArgs e)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i].tileButton.ClearValue(BorderBrushProperty);
                candidates[i].tileButton.BorderThickness = new Thickness(1);
                candidates[i].ImageHidden();
            }

            playAreaGrid.IsHitTestVisible = true;

            compareImagesTimer.Stop();
        }
    }
}
