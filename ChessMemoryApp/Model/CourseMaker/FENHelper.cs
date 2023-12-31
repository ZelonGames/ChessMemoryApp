﻿using System;
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

        public static Piece.ColorType GetColorFromFen(string fen)
        {

            string[] fenComponents = fen.Split(' ');
            string colorFromFen = fenComponents.Length == 1 ? fen : fenComponents[1];
            return colorFromFen == "w" ? Piece.ColorType.White : Piece.ColorType.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fen"></param>
        /// <returns>returns null if there is no en passant available</returns>
        public static string GetEnPassantSquareFromFen(string fen)
        {
            string[] fenComponents = fen.Split(' ');
            string enPassantSquare = fenComponents.Length >= 4 ? fenComponents[3] : null;
            return enPassantSquare != "-" ? enPassantSquare : null;
        }

        public static string ConvertFenToChessableUrl(string fen, string courseID)
        {
            if (fen == null)
                return null;

            string startingHtml = "https://www.chessable.com/course/" + courseID + "/fen/";
            return startingHtml + fen.Split(' ')[0].Replace('/', ';');
        }

        public static string ConvertFenToLichessUrl(string fen, FenSettings fenSettings, Piece.ColorType colorToPlay)
        {
            fen = fen.Split(' ')[0];
            string colorName = colorToPlay == Piece.ColorType.White ? "white" : "black";
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

        public static char? GetPieceOnSquare(string fen, string coordinate)
        {
            if (!IsValidFen(fen) || string.IsNullOrEmpty(coordinate))
                return null;

            int column = coordinate[0] - 'a';
            int row = 8 - (coordinate[1] - '0');

            char[,] board = GetBoardFromFEN(fen);
            char piece = board[row, column];

            // Empty character \0 means there is no piece there
            if (piece == '\0')
                return null;

            return piece;
        }

        public static Dictionary<string, char?> GetPiecesFromFen(string fen)
        {
            var pieces = new Dictionary<string, char?>();

            if (!IsValidFen(fen))
                return pieces;

            for (char row = '1'; row <= '8'; row++)
            {
                for (char column = 'a'; column <= 'h'; column++)
                {
                    string coordinate = column.ToString() + row.ToString();
                    pieces.Add(coordinate, GetPieceOnSquare(fen, coordinate));
                }
            }

            return pieces;
        }

        public static Dictionary<string, char> GetPiecesByColorFromFen(string fen, Piece.ColorType color)
        {
            var pieces = new Dictionary<string, char>();

            if (!IsValidFen(fen))
                return pieces;

            for (char row = '1'; row <= '8'; row++)
            {
                for (char column = 'a'; column <= 'h'; column++)
                {
                    string coordinate = column.ToString() + row.ToString();

                    char? piece = GetPieceOnSquare(fen, coordinate);
                    if (piece.HasValue && color == Piece.ColorType.White && char.IsUpper(piece.Value) ||
                        piece.HasValue && color == Piece.ColorType.Black && char.IsLower(piece.Value))
                        pieces.Add(coordinate, piece.Value);
                }
            }

            return pieces;
        }

        public static Dictionary<string, char> GetPiecesOfTypeFromFen(char pieceType, string fen)
        {
            var pieces = new Dictionary<string, char>();

            if (!IsValidFen(fen))
                return pieces;

            for (char row = '1'; row <= '8'; row++)
            {
                for (char column = 'a'; column <= 'h'; column++)
                {
                    string coordinate = column.ToString() + row.ToString();
                    char? piece = GetPieceOnSquare(fen, coordinate);
                    if (piece.HasValue && piece.Value == pieceType)
                    {
                        pieces.Add(coordinate, piece.Value);

                        // There is only one king
                        if (char.ToLower(pieceType) == 'k')
                            return pieces;
                    }
                }
            }

            return pieces;
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

        public static string MakeMoveWithCoordinates(string currentFen, string moveNotationCoordinates, bool updateColorToPlay = true)
        {
            string newFen = currentFen;
            string fromCoordinates = moveNotationCoordinates[..2];
            string toCoordinates = moveNotationCoordinates[2..];
            char? piece = GetPieceOnSquare(currentFen, fromCoordinates);
            if (!piece.HasValue)
                return null;
            
            #region Castling Moves
            bool isCastlingMove = false;
            bool isMovingPieceKing = piece.HasValue && char.ToLower(piece.Value) == 'k';
            if (isMovingPieceKing)
            {
                if (moveNotationCoordinates == "e1h1" || moveNotationCoordinates == "e1g1")
                {
                    // White king side castle
                    newFen = TeleportPiece(currentFen, "e1g1", false);
                    newFen = TeleportPiece(newFen, "h1f1", false);
                    isCastlingMove = true;
                }
                else if (moveNotationCoordinates == "e1a1" || moveNotationCoordinates == "e1c1")
                {
                    // White queen side castle
                    newFen = TeleportPiece(currentFen, "e1c1", false);
                    newFen = TeleportPiece(newFen, "a1d1", false);
                    isCastlingMove = true;
                }
                else if (moveNotationCoordinates == "e8h8" || moveNotationCoordinates == "e8g8")
                {
                    // Black king side castle
                    newFen = TeleportPiece(currentFen, "e8g8", false);
                    newFen = TeleportPiece(newFen, "h8f8", false);
                    isCastlingMove = true;
                }
                else if (moveNotationCoordinates == "e8a8" || moveNotationCoordinates == "e8c8")
                {
                    // Black queen side castle
                    newFen = TeleportPiece(currentFen, "e8c8", false);
                    newFen = TeleportPiece(newFen, "a8d8", false);
                    isCastlingMove = true;
                }
            }

            if (isCastlingMove)
                return updateColorToPlay ? UpdateFenColorToPlay(newFen) : newFen;
            #endregion

            char? capturedPiece = GetPieceOnSquare(currentFen, toCoordinates);
            if (capturedPiece.HasValue)
                newFen = RemovePieceFromFEN(newFen, toCoordinates);

            string enPassantSquare = GetEnPassantSquareFromFen(currentFen);
            if (toCoordinates == enPassantSquare)
            {
                char movedPiece = GetPieceOnSquare(currentFen, fromCoordinates).Value;
                bool isMovedPiecePawn = char.ToLower(movedPiece) == 'p';
                if (isMovedPiecePawn)
                {
                    bool isWhitePawn = movedPiece == 'P';

                    if (isWhitePawn)
                    {
                        Piece.Coordinates<int> toNumberCoordinates = BoardHelper.GetNumberCoordinates(toCoordinates);
                        string capturedPieceCoordinates = BoardHelper.GetLetterCoordinates(new Piece.Coordinates<int>(toNumberCoordinates.X, toNumberCoordinates.Y - 1));
                        newFen = RemovePieceFromFEN(newFen, capturedPieceCoordinates);
                    }
                }
            }

            return TeleportPiece(newFen, moveNotationCoordinates, updateColorToPlay);
        }

        public static string TeleportPiece(string currentFen, string moveNotationCoordinates, bool updateColorToPlay = true)
        {
            string fromCoordinates = moveNotationCoordinates[..2];
            string toCoordinates = moveNotationCoordinates[2..];

            char? pieceChar = GetPieceOnSquare(currentFen, fromCoordinates);
            if (!pieceChar.HasValue)
                return string.Empty;

            string newFen = RemovePieceFromFEN(currentFen, fromCoordinates);
            newFen = updateColorToPlay ? UpdateFenColorToPlay(newFen) : newFen;
            return AddPieceToFEN(newFen, toCoordinates, pieceChar.Value);
        }

        public static int GetAmountOfPlayedPlyMoves(string fen)
        {
            string colorToPlay = GetColorToPlayFromFen(fen);
            int fullMoves = GetFullmoves(fen);

            return colorToPlay == "w" ? fullMoves * 2 : fullMoves * 2 - 1;
        }

        public static int GetFullmoves(string fen)
        {
            return Convert.ToInt32(fen.Split(' ').Last()) - 1;
        }

        public static string GetColorToPlayFromFen(string fen)
        {
            return fen.Split(' ')[1];
        }

        public static string SetColorToPlayInFen(string fen, Piece.ColorType color)
        {
            string[] fenComponents = fen.Split(' ');
            string fenColor = FenSettings.FenColor.GetColorFromPieceColor(color);
            string newFen = "";

            for (int i = 0; i < fenComponents.Length; i++)
            {
                if (i == 1)
                    newFen += fenColor + " ";
                else
                    newFen += fenComponents[i] + " ";
            }

            return newFen[..^1];
        }

        public static Piece.ColorType GetColorTypeToPlayFromFen(string fen)
        {
            return GetColorToPlayFromFen(fen) == "w" ? Piece.ColorType.White : Piece.ColorType.Black;
        }

        public static string GetOppositeColor(string color)
        {
            return color == "w" ? "b" : "w";
        }

        private static string UpdateFenColorToPlay(string fen)
        {
            if (fen.Split(' ').Any(x => x == "w"))
                fen = fen.Replace(" w ", " b ");
            else
                fen = fen.Replace(" b ", " w ");

            return fen;
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
        /// Converts for example g2f3 to Nf3 based on the given fen
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="moveNotationCoordinates">g2f3</param>
        /// <returns>"Nf3"</returns>
        public static string ConvertCoordinatesToMoveNotation(string fen, string moveNotationCoordinates)
        {
            string fromCoordinates = moveNotationCoordinates[2..];
            string toCoordinates = moveNotationCoordinates[..2];

            if (fromCoordinates == "e1" && toCoordinates == "h1" ||
                fromCoordinates == "e8" && toCoordinates == "h8")
                return "O-O";

            if (fromCoordinates == "e1" && toCoordinates == "c1" ||
                fromCoordinates == "e8" && toCoordinates == "c8")
                return "O-O-O";

            char? piece = GetPieceOnSquare(fen, fromCoordinates);
            if (!piece.HasValue)
                return null;

            char? capturedPiece = GetPieceOnSquare(fen, toCoordinates);

            string moveNotation;

            if (Char.ToUpper(piece.Value) != 'P')
                moveNotation = Char.ToUpper(piece.Value).ToString();
            else
                moveNotation = fromCoordinates[0].ToString();

            if (capturedPiece.HasValue)
                moveNotation += "x";

            moveNotation += toCoordinates;

            return moveNotation;
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
            string[] fenComponents = fen.Split(' ');
            char[,] board = new char[8, 8];

            int row = 0;
            int col = 0;

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

        #region Castling State From Fen

        public static bool CanWhiteCastleKingSide(string fen)
        {
            string[] fenComponents = fen.Split(' ');

            if (fenComponents.Length >= 3)
            {
                string castlingComponent = fenComponents[2];
                return castlingComponent.Contains('K');
            }

            return false;
        }

        public static bool CanWhiteCastleQueenSide(string fen)
        {
            string[] fenComponents = fen.Split(' ');

            if (fenComponents.Length >= 3)
            {
                string castlingComponent = fenComponents[2];
                return castlingComponent.Contains('Q');
            }

            return false;
        }

        public static bool CanBlackCastleKingSide(string fen)
        {
            string[] fenComponents = fen.Split(' ');

            if (fenComponents.Length >= 3)
            {
                string castlingComponent = fenComponents[2];
                return castlingComponent.Contains('k');
            }

            return false;
        }

        public static bool CanBlackCastleQueenSide(string fen)
        {
            string[] fenComponents = fen.Split(' ');

            if (fenComponents.Length >= 3)
            {
                string castlingComponent = fenComponents[2];
                return castlingComponent.Contains('q');
            }

            return false;
        }

        #endregion

        public static bool IsValidFen(string fen)
        {
            return fen != string.Empty && fen != null;
        }
    }
}

