using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SQLite.TableMapping;

namespace ChessMemoryApp.Model
{
    public class SelectorPageController<T> where T : ChessboardGenerator
    {
        public Size BoardSize { get; private set; }
        private readonly List<T> chessBoards = new();
        private readonly AbsoluteLayout coursesLayout;

        private Course selectedCourse;

        public SelectorPageController(List<T> customVariationBoards, AbsoluteLayout coursesLayout, Course selectedCourse)
        {
            this.chessBoards = customVariationBoards;
            this.coursesLayout = coursesLayout;
            this.selectedCourse = selectedCourse;
        }

        public void Window_SizeChanged(object sender, EventArgs e)
        {
            if (sender is not ContentPage)
                return;

            if (chessBoards.Count == 0)
                return;


            var contentPage = sender as ContentPage;

            int margin = 10;
            int outerMargin = 50;
            coursesLayout.WidthRequest = contentPage.Bounds.Width - outerMargin;

            int columns = 3;
            int minChessBoardSize = 200;
            double maxChessBoardSize = (contentPage.Bounds.Height - outerMargin) / 2;
            double autoChessBoardSize = coursesLayout.WidthRequest / columns - margin;
            double chessBoardSize = Math.Max(minChessBoardSize, Math.Min(autoChessBoardSize, maxChessBoardSize));

            if (chessBoardSize == minChessBoardSize)
            {
                int minColumns = 1;
                columns = Math.Max(minColumns, (int)(coursesLayout.WidthRequest / (chessBoardSize + margin)));
                if (chessBoards.Count < columns)
                    columns = chessBoards.Count;
            }

            coursesLayout.WidthRequest = (chessBoardSize + margin) * columns - margin;

            int currentColumn = 0;
            int currentRow = 0;

            foreach (var chessBoard in chessBoards)
            {
                if (currentColumn >= columns)
                {
                    currentColumn = 0;
                    currentRow++;
                }

                chessBoard.UpdateBoardSize(chessBoardSize);
                chessBoard.offset.X = (chessBoardSize + margin) * currentColumn;
                chessBoard.offset.Y = (chessBoardSize + margin) * currentRow;
                chessBoard.UpdateSquaresBounds();
                currentColumn++;
            }

            coursesLayout.HeightRequest = (chessBoardSize + margin) * (currentRow + 1) - margin;
        }

        public void Window_SizeChanged2(object sender, EventArgs e)
        {
            if (sender is not ContentPage)
                return;

            if (chessBoards.Count == 0)
                return;

            int margin = 10;
            int minColumns = 1;
            int maxColumns = 3;
            int columns = Math.Max(minColumns, Math.Min(maxColumns, (int)((sender as ContentPage).Bounds.Width / (BoardSize.Width + margin)) - 1));
            if (chessBoards.Count < columns)
                columns = chessBoards.Count;

            double totalWidth = (BoardSize.Width + margin) * columns - margin;

            int currentColumn = 0;
            int currentRow = 0;

            foreach (var chessBoard in chessBoards)
            {
                if (currentColumn >= columns)
                {
                    currentColumn = 0;
                    currentRow++;
                }

                chessBoard.offset.X = (BoardSize.Width + margin) * currentColumn;
                chessBoard.offset.Y = (BoardSize.Height + margin) * currentRow;
                chessBoard.UpdateSquaresBounds();
                currentColumn++;
            }

            coursesLayout.HeightRequest = (BoardSize.Width + margin) * (currentRow + 1) - margin;
            coursesLayout.WidthRequest = totalWidth;
        }
    }
}
