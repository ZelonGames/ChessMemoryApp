using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.Lichess.Lichess_API;

namespace ChessMemoryApp.Model.CourseMaker
{
    public static class FenHelper
    {
        public const string STARTING_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public static string ConvertFenToChessableUrl(string fen, string courseID)
        {
            if (fen == null)
                return null;

            string startingHtml = "https://www.chessable.com/course/" + courseID + "/fen/";
            return startingHtml + fen.Split(' ')[0].Replace('/', ';');
        }

        public static string ConvertFenToLichessUrl(string fen, FenSettings fenSettings, bool white)
        {
            fen = fen.Split(' ')[0];
            string colorName = white ? "white" : "black";
            return "https://lichess.org/analysis/" + fen + fenSettings.GetAppliedSettings(FenSettings.SpaceEncoding.SPACE) + "?color=" + colorName;
        }

        public static HashSet<string> GetSquaresWithPiece(string fen, char pieceType)
        {
            char[,] board = GetBoardFromFEN(fen);

            // Find all squares containing a piece of the specified type and color
            var squares = new HashSet<string>();
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    char piece = board[rank, file];
                    if (piece != '\0' && Char.ToLower(piece) == Char.ToLower(pieceType) && (Char.IsUpper(piece) == Char.IsUpper(pieceType)))
                        squares.Add(Convert.ToChar('a' + file) + (8 - rank).ToString());
                }
            }

            return squares;
        }

        public static List<char> GetPiecesOnBoardByColor(string fen, Piece.ColorType color)
        {
            var pieces = new List<char>();

            char[,] board = GetBoardFromFEN(fen);



            return pieces;
        }

        public static char? GetPieceOnSquare(string fen, string coordinate)
        {
            // Get the rank and file from the coordinates string
            int column = coordinate[0] - 'a';
            int row = 8 - (coordinate[1] - '0');

            // Get the board from the FEN string
            char[,] board = GetBoardFromFEN(fen);

            // Get the piece at the specified square
            char piece = board[row, column];

            // Return empty character if there is no piece on the square
            if (piece == '\0')
                return null;

            return piece;
        }

        public static string AddPieceToFEN(string fen, string coordinates, char piece)
        {
            // Get the rank and file from the coordinates string
            int file = coordinates[0] - 'a';
            int rank = 8 - (coordinates[1] - '0');

            // Get the board from the FEN string
            char[,] board = GetBoardFromFEN(fen);

            // Set the specified square to the new piece
            board[rank, file] = piece;

            // Modify the FEN string with the updated board state
            string newFEN = ModifyFEN(fen, board);

            return newFEN;
        }

        public static string RemovePieceFromFEN(string fen, string coordinates)
        {
            var board = RemovePieceFromBoard(fen, coordinates, out _);
            return ModifyFEN(fen, board);
        }

        public static string RemovePieceFromFEN(string fen, string coordinates, out char? removedPiece)
        {
            var board = RemovePieceFromBoard(fen, coordinates, out removedPiece);
            return ModifyFEN(fen, board);
        }

        private static char[,] RemovePieceFromBoard(string fen, string coordinates, out char? removedPiece)
        {
            // Get the rank and file from the coordinates string
            int file = coordinates[0] - 'a';
            int rank = 8 - (coordinates[1] - '0');

            // Get the board from the FEN string
            char[,] board = GetBoardFromFEN(fen);
            removedPiece = board[rank, file];
            if (removedPiece == '\0')
                removedPiece = null;

            // Set the specified square to an empty square
            board[rank, file] = '\0';

            return board;
        }

        private static string GetLetterFromCoordinates(string fen, string moveNotation, Piece.ColorType color)
        {
            moveNotation = moveNotation.Replace("+", "").Replace("#", "");
            bool isPawn = false;
            bool isCapture = moveNotation.Contains('x');
            char pieceType = moveNotation.First();

            if (char.IsLower(pieceType))
            {
                pieceType = color.Equals(Piece.ColorType.White) ? 'P' : 'p';
                isPawn = true;
            }

            if (color.Equals(Piece.ColorType.Black))
                pieceType = char.ToLower(pieceType);

            string letterToCoordinates = moveNotation[^2..];
            Piece.Coordinates<int> numberToCoordinates = BoardHelper.GetNumberCoordinates(letterToCoordinates);

            HashSet<string> squares = GetSquaresWithPiece(fen, pieceType);
            HashSet<string> possibleMoves;

            switch (char.ToLower(pieceType))
            {
                case 'r':
                    possibleMoves = Rook.GetAvailableMoves(numberToCoordinates, color, fen);
                    break;
                case 'n':
                    possibleMoves = Knight.GetAvailableMoves(numberToCoordinates, color, fen);
                    break;
                case 'b':
                    possibleMoves = Bishop.GetAvailableMoves(numberToCoordinates, color, fen);
                    break;
                case 'q':
                    possibleMoves = Queen.GetAvailableMoves(numberToCoordinates, color, fen);
                    break;
                case 'k':
                    possibleMoves = King.GetAvailableMoves(numberToCoordinates, color, fen);
                    break;
                default:
                    bool whiteDirection = color.Equals(Piece.ColorType.White);
                    bool direction = !whiteDirection;
                    if (isCapture)
                    {
                        //direction = !whiteDirection ? whiteDirection : whiteDirection;
                    }

                    possibleMoves = Pawn.GetAvailableMoves(numberToCoordinates, color, fen, direction, isCapture);
                    break;
            }

            string letterFromCoordinates = "";

            squares.IntersectWith(possibleMoves);

            if (squares.Count == 0)
            {
                return "";
            }

            if (squares.Count > 1)
            {
                if (isPawn && isCapture)
                    letterFromCoordinates = squares.Where(x => x[0] == moveNotation[0]).FirstOrDefault();
                else if (char.IsDigit(moveNotation[1]))
                    letterFromCoordinates = squares.Where(x => x[1] == moveNotation[1]).FirstOrDefault();
                else
                    letterFromCoordinates = squares.Where(x => x[0] == moveNotation[1]).FirstOrDefault();
            }
            else
            {
                letterFromCoordinates = squares.First();
            }

            return letterFromCoordinates;
        }

        /// <summary>
        /// Calculates the fen you would get if you made the move from the currentFen
        /// </summary>
        /// <param name="currentFen"></param>
        /// <param name="moveNotation"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string MakeMove(string currentFen, string moveNotation, Piece.ColorType color)
        {
            moveNotation = moveNotation.Replace("+", "").Replace("#", "");
            string newFEN = currentFen;

            bool isCastling = moveNotation == "O-O" || moveNotation == "O-O-O";
            if (isCastling)
            {
                #region Castle Moves
                bool kingSide = moveNotation == "O-O";
                char king = color.Equals(Piece.ColorType.White) ? 'K' : 'k';
                char rook = color.Equals(Piece.ColorType.White) ? 'R' : 'r';
                string rank = color.Equals(Piece.ColorType.White) ? "1" : "8";

                if (kingSide)
                {
                    newFEN = RemovePieceFromFEN(newFEN, "e" + rank);
                    newFEN = AddPieceToFEN(newFEN, "g" + rank, king);
                    newFEN = RemovePieceFromFEN(newFEN, "h" + rank);
                    newFEN = AddPieceToFEN(newFEN, "f" + rank, rook);
                }
                else
                {
                    newFEN = RemovePieceFromFEN(newFEN, "e" + rank);
                    newFEN = AddPieceToFEN(newFEN, "c" + rank, king);
                    newFEN = RemovePieceFromFEN(newFEN, "a" + rank);
                    newFEN = AddPieceToFEN(newFEN, "d" + rank, rook);
                }

                return newFEN;
                #endregion
            }
            else
            {
                string[] promotionComponents = moveNotation.Split('=');
                bool promotion = promotionComponents.Length == 2;
                string toCoordinates = promotionComponents[0][^2..];
                string fromCoordinates = GetLetterFromCoordinates(currentFen, promotionComponents[0], color);

                if (fromCoordinates == "")
                    return currentFen;

                char? piece = GetPieceOnSquare(currentFen, fromCoordinates);
                char? pieceToCapture = GetPieceOnSquare(currentFen, toCoordinates);

                bool pawnTakes = moveNotation.Contains('x') && char.ToLower(piece.Value) == 'p';
                if (pawnTakes)
                {
                    bool isEnpassant = !pieceToCapture.HasValue;
                    if (isEnpassant)
                    {
                        // Remove pawn from behind
                        Piece.Coordinates<int> numberToCoordinates = BoardHelper.GetNumberCoordinates(toCoordinates);
                        numberToCoordinates.Y += color.Equals(Piece.ColorType.White) ? -1 : 1;
                        newFEN = RemovePieceFromFEN(newFEN, BoardHelper.GetLetterCoordinates(numberToCoordinates));
                    }
                    else
                        newFEN = RemovePieceFromFEN(newFEN, toCoordinates);
                }

                newFEN = RemovePieceFromFEN(newFEN, fromCoordinates);
                if (promotion)
                {
                    char promotedPiece = color.Equals(Piece.ColorType.White) ? Char.ToUpper(promotionComponents[1][0]) : Char.ToLower(promotionComponents[1][0]);
                    newFEN = AddPieceToFEN(newFEN, toCoordinates, promotedPiece);
                }
                else
                    newFEN = AddPieceToFEN(newFEN, toCoordinates, piece.Value);
            }


            return newFEN;
        }

        public static string MakeMove(string currentFen, string moveNotationCoordinates)
        {
            string fromCoordinates = moveNotationCoordinates[..2];
            char? pieceChar = GetPieceOnSquare(currentFen, fromCoordinates);
            if (!pieceChar.HasValue)
                return string.Empty;

            string toCoordinates = moveNotationCoordinates.Substring(moveNotationCoordinates.Length - 2);

            string newFen = RemovePieceFromFEN(currentFen, fromCoordinates);
            return AddPieceToFEN(newFen, toCoordinates, pieceChar.Value);
        }

        public static int GetXCoordinateFromFenRow(string fenRow)
        {
            int xCoordinate = 0;

            foreach (var character in fenRow)
            {
                if (char.IsDigit(character))
                    xCoordinate += (int)char.GetNumericValue(character);
                else
                    xCoordinate++;
            }

            return xCoordinate;
        }

        /// <summary>
        /// Converts for example Nf3 to g2f3 based on the given fen
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="moveNotation"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ConvertToMoveNotationCoordinates(string fen, string moveNotation, Piece.ColorType color)
        {
            if (moveNotation == "0-0" || moveNotation == "O-O")
            {
                if (color == Piece.ColorType.Black)
                    return "e8g8";
                else
                    return "e1g1";
            }
            else if (moveNotation == "0-0-0" || moveNotation == "O-O-O")
            {
                if (color == Piece.ColorType.Black)
                    return "e8c8";
                else
                    return "e1c1";
            }

            moveNotation = moveNotation.Replace("+", "").Replace("#", "");
            string fromCoordinates = GetLetterFromCoordinates(fen, moveNotation, color);
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveNotation);

            return fromCoordinates + toCoordinates;
        }

        public static string GetMoveNotationCoordinates(string previousFen, string nextFen, Piece.ColorType color)
        {
            char[,] previousBoard = GetBoardFromFEN(previousFen, true);
            char[,] nextBoard = GetBoardFromFEN(nextFen, true);

            string fromCoordinates = "";
            string toCoordinates = "";

            for (int x = 0; x < previousBoard.GetLength(0); x++)
            {
                for (int y = 0; y < previousBoard.GetLength(1); y++)
                {
                    if (previousBoard[x, y] != nextBoard[x, y])
                    {
                        var piece = previousBoard[x, y];
                        bool isSquareEmpty = piece == '\0';
                        var pieceColor = char.IsUpper(piece) ? Piece.ColorType.White : Piece.ColorType.Black;
                        string letterCoordinates = BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(x + 1, 8 - y));
                        if (pieceColor != color || isSquareEmpty)
                            toCoordinates = letterCoordinates;
                        else if (pieceColor == color)
                            fromCoordinates = letterCoordinates;

                        if (fromCoordinates != "" && toCoordinates != "")
                            return fromCoordinates + toCoordinates;
                    }
                }
            }

            return "";
        }

        private static string ModifyFEN(string fen, char[,] board)
        {
            // Generate the new FEN string
            var newFEN = new StringBuilder();
            for (int rank = 0; rank < 8; rank++)
            {
                int emptySquares = 0;
                for (int file = 0; file < 8; file++)
                {
                    char currentPiece = board[rank, file];
                    if (currentPiece == '\0')
                    {
                        emptySquares++;
                    }
                    else
                    {
                        if (emptySquares > 0)
                        {
                            newFEN.Append(emptySquares);
                            emptySquares = 0;
                        }
                        newFEN.Append(currentPiece);
                    }
                }
                if (emptySquares > 0)
                    newFEN.Append(emptySquares);
                if (rank != 7)
                    newFEN.Append('/');
            }

            // Add the other FEN components to the new FEN string
            string[] fenComponents = fen.Split(' ');
            if (fenComponents.Length > 1)
            {
                newFEN.Append(' ');
                newFEN.Append(fenComponents[1]);
                newFEN.Append(' ');
                newFEN.Append(fenComponents[2]);
                newFEN.Append(' ');
                if (fenComponents.Length >= 4)
                {
                    newFEN.Append(fenComponents[3]);
                    newFEN.Append(' ');
                    if (fenComponents.Length >= 5)
                    {
                        newFEN.Append(fenComponents[4]);
                        newFEN.Append(' ');
                        if (fenComponents.Length >= 6)
                            newFEN.Append(fenComponents[5]);
                    }
                }
            }

            return newFEN.ToString();
        }

        private static char[,] GetBoardFromFEN(string fen, bool flipped = false)
        {
            // Split the FEN string into its components
            string[] fenComponents = fen.Split(' ');

            // Initialize the board
            char[,] board = new char[8, 8];

            // Initialize the row and column indices
            int row = 0;
            int col = 0;

            // Parse the FEN string to populate the board
            foreach (char c in fenComponents[0])
            {
                if (c == '/')
                {
                    row++;
                    col = 0;
                }
                else if (Char.IsDigit(c))
                    col += (int)Char.GetNumericValue(c);
                else
                {
                    if (flipped)
                        board[col, row] = c;
                    else
                        board[row, col] = c;
                    col++;
                }
            }

            return board;
        }

        public static bool IsValidFen(string fen)
        {
            return fen != string.Empty && fen != null;
        }
    }
}

