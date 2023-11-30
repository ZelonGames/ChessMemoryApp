using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessMemoryApp.Model.Chess_Board.Pieces;

namespace ChessMemoryApp.Model.Chess_Board
{
    public static class BoardHelper
    {
        public static Piece.ColorType piecesToMove = Piece.ColorType.White;

        /// <summary>
        /// replaces the x coordinate with the corresponding letter
        /// </summary>
        /// <returns></returns>
        public static string GetLetterCoordinates(Piece.Coordinates<int> coordinate)
        {
            if (coordinate.X < 1 || coordinate.Y < 1 || coordinate.X > 8 || coordinate.Y > 8)
                return "";

            string letterCoordinates = "abcdefgh";
            return letterCoordinates[coordinate.X - 1].ToString() + coordinate.Y;
        }

        public static string GetMoveNotationCoordinates(ChessboardGenerator chessBoard, string moveNotation, Piece.ColorType pieceColor)
        {
            moveNotation = moveNotation.Split('=')[0].Replace("#", "").Replace("+", "");
            char row = pieceColor == Piece.ColorType.White ? '1' : '8';

            if (moveNotation == "O-O")
                return $"e{row}g{row}";
            else if (moveNotation == "O-O-O")
                return $"e{row}c{row}";

            string toCoordinates = GetToCoordinatesString(moveNotation);
            char pieceType = moveNotation[0];
            bool isPawnMove = char.IsLower(pieceType);

            if (isPawnMove)
                pieceType = pieceColor == Piece.ColorType.White ? 'P' : 'p';
            else
                pieceType = pieceColor == Piece.ColorType.White ? pieceType : char.ToLower(pieceType);

            // Coordinates are never specified if you move a pawn
            // Otherwise this would fail to something like dxe4 because dx is of length 2
            bool isCoordinateSpecified = !isPawnMove && moveNotation.Replace(toCoordinates, "").Length > 2;
            Dictionary<string, Piece> potentialPieces = GetPiecesOfType(chessBoard, pieceType);

            if (isCoordinateSpecified)
            {
                char coordinateChar = moveNotation[1];

                foreach (var piece in potentialPieces)
                {
                    if (piece.Value.coordinates[0] == coordinateChar ||
                        piece.Value.coordinates[1] == coordinateChar)
                        return piece.Value.coordinates + toCoordinates;
                }
            }
            else
            {
                foreach (var piece in potentialPieces)
                {
                    bool canPieceGoToDestination = piece.Value.GetAvailableMoves().Contains(toCoordinates);
                    if (canPieceGoToDestination)
                        return piece.Value.coordinates + toCoordinates;
                }
            }

            return null;
        }

        public static string GetMoveNotationFromCoordinates<T>(T chessBoard, string moveNotationCoordinates) where T : ChessboardGenerator
        {
            string fromCoordinates = GetFromCoordinatesString(moveNotationCoordinates);
            string toCoordinates = GetToCoordinatesString(moveNotationCoordinates);
            Piece capturedPiece = chessBoard.GetPiece(toCoordinates);
            Piece movedPiece = chessBoard.GetPiece(fromCoordinates);

            if (movedPiece == null)
                return null;

            Dictionary<string, Piece> piecesOfSameType = GetPiecesOfType(chessBoard, movedPiece.pieceType);
            if (piecesOfSameType.Count == 1)
            {
                char piecePrefix = movedPiece is Pawn ? fromCoordinates[0] : char.ToUpper(movedPiece.pieceType);
                return capturedPiece != null ? piecePrefix + "x" + toCoordinates : piecePrefix + toCoordinates;
            }

            string moveNotation = "";

            foreach (var piece in piecesOfSameType)
            {

            }

            return moveNotation;
        }

        public static Piece.Coordinates<int> GetNumberCoordinates(string letterCoordinates)
        {
            string allLetterCoordinates = "abcdefgh";
            int letterCoordinate = letterCoordinates.Length == 3 ? allLetterCoordinates.IndexOf(letterCoordinates[1]) + 1 : allLetterCoordinates.IndexOf(letterCoordinates[0]) + 1;
            char yCoordinate = letterCoordinates.Length == 3 ? letterCoordinates[2] : letterCoordinates[1];

            return new Piece.Coordinates<int>(letterCoordinate, (int)char.GetNumericValue(yCoordinate));
        }

        public static Piece.Coordinates<char> GetCoordinates(string moveNotation)
        {
            moveNotation = moveNotation.Replace("+", "").Replace("#", "");
            if (moveNotation.Contains('x'))
                moveNotation = moveNotation.Split('x')[1];

            char x = moveNotation[moveNotation.Length - 2];
            char y = moveNotation[moveNotation.Length - 1];

            return new Piece.Coordinates<char>(x, y);
        }

        public static Piece.Coordinates<char> GetFromCoordinates(string numberCoordinates)
        {
            numberCoordinates = GetFromCoordinatesString(numberCoordinates);
            char x = numberCoordinates[0];
            char y = numberCoordinates[1];

            return new Piece.Coordinates<char>(x, y);
        }

        public static string GetFromCoordinatesString(string numberCoordinates)
        {
            return numberCoordinates[..2];
        }

        public static string GetToCoordinatesString(string numberCoordinates)
        {
            return numberCoordinates[^2..];
        }

        public static Piece.Coordinates<char> GetToCoordinates(string numberCoordinates)
        {
            numberCoordinates = GetToCoordinatesString(numberCoordinates);
            char x = numberCoordinates[0];
            char y = numberCoordinates[1];

            return new Piece.Coordinates<char>(x, y);
        }

        public static HashSet<string> GetControlledSquaresByColor(ChessboardGenerator chessBoard, Piece.ColorType color)
        {
            var squares = new HashSet<string>();

            foreach (var piece in chessBoard.pieces)
            {
                if (piece.Value.color == color)
                {
                    HashSet<string> coveredSquares = piece.Value.GetAvailableMoves();
                    foreach (var coveredSquare in coveredSquares)
                        squares.Add(coveredSquare);
                }
            }

            return squares;
        }

        public static bool IsSquareControlledByColor(ChessboardGenerator chessBoard, Piece.ColorType color, string squareCoordinates)
        {
            foreach (var piece in chessBoard.pieces)
            {
                if (piece.Value.color != color || piece.Value is King)
                    continue;

                HashSet<string> coveredSquares = piece.Value.GetAvailableMoves();
                foreach (var coveredSquare in coveredSquares)
                {
                    if (coveredSquare == squareCoordinates)
                        return true;
                }
            }

            return false;
        }

        public static Dictionary<string, Piece> GetPiecesOfType(ChessboardGenerator chessBoard, char pieceType, bool colorSpecific = true)
        {
            var pieces = new Dictionary<string, Piece>();

            foreach (var piece in chessBoard.pieces)
            {
                if (colorSpecific && piece.Value.pieceType == pieceType ||
                    !colorSpecific && char.ToLower(piece.Value.pieceType) == char.ToLower(pieceType))
                    pieces.Add(piece.Key, piece.Value);
            }

            return pieces;
        }

        public static void MakeMove<T>(T chessBoard, Piece.ColorType pieceColor, string moveNotation) where T : ChessboardGenerator
        {
            moveNotation = moveNotation.Replace("+", "").Replace("#", "");

            bool isCastling = moveNotation == "O-O" || moveNotation == "O-O-O";
            if (isCastling)
            {
                #region Castle Moves
                bool kingSide = moveNotation == "O-O";
                char king = chessBoard.boardColorOrientation == Piece.ColorType.White ? 'K' : 'k';
                char rook = chessBoard.boardColorOrientation == Piece.ColorType.White ? 'R' : 'r';
                string rank = chessBoard.boardColorOrientation == Piece.ColorType.White ? "1" : "8";

                if (kingSide)
                {
                    chessBoard.RemovePiece(chessBoard.GetPiece("e" + rank));
                    chessBoard.TryAddPieceToSquare(king, "g" + rank);
                    chessBoard.RemovePiece(chessBoard.GetPiece("h" + rank));
                    chessBoard.TryAddPieceToSquare(rook, "f" + rank);
                }
                else
                {
                    chessBoard.RemovePiece(chessBoard.GetPiece("e" + rank));
                    chessBoard.TryAddPieceToSquare(king, "c" + rank);
                    chessBoard.RemovePiece(chessBoard.GetPiece("a" + rank));
                    chessBoard.TryAddPieceToSquare(rook, "d" + rank);
                }

                return;
                #endregion
            }
            else
            {
                string[] promotionComponents = moveNotation.Split('=');
                bool promotion = promotionComponents.Length == 2;
                string toCoordinates = promotionComponents[0][^2..];
                string fromCoordinates = GetLetterFromCoordinates(chessBoard, pieceColor, promotionComponents[0]);

                if (fromCoordinates == "")
                    return;

                Piece piece = chessBoard.GetPiece(fromCoordinates);
                Piece pieceToCapture = chessBoard.GetPiece(toCoordinates);
                Piece pieceToRemove;

                bool pawnTakes = moveNotation.Contains('x') && piece is Pawn;
                if (pawnTakes)
                {
                    bool isEnpassant = pieceToCapture == null;
                    if (isEnpassant)
                    {
                        // Remove pawn from behind
                        Piece.Coordinates<int> numberToCoordinates = BoardHelper.GetNumberCoordinates(toCoordinates);
                        numberToCoordinates.Y += chessBoard.boardColorOrientation == Piece.ColorType.White ? -1 : 1;
                        pieceToRemove = chessBoard.GetPiece(BoardHelper.GetLetterCoordinates(numberToCoordinates));
                        chessBoard.RemovePiece(pieceToRemove);
                    }
                    else
                    {
                        pieceToRemove = chessBoard.GetPiece(toCoordinates);
                        chessBoard.RemovePiece(pieceToRemove);
                    }
                }

                pieceToRemove = chessBoard.GetPiece(fromCoordinates);
                chessBoard.RemovePiece(pieceToRemove);
                if (promotion)
                {
                    char promotedPiece = chessBoard.boardColorOrientation == Piece.ColorType.White ? char.ToUpper(promotionComponents[1][0]) : char.ToLower(promotionComponents[1][0]);
                    chessBoard.TryAddPieceToSquare(promotedPiece, toCoordinates);
                }
                else
                    chessBoard.TryAddPieceToSquare(piece.pieceType, toCoordinates);
            }
        }

        public static string GetLetterFromCoordinates<T>(T chessBoard, Piece.ColorType pieceColor, string moveNotation) where T : ChessboardGenerator
        {
            moveNotation = moveNotation.Replace("+", "").Replace("#", "");
            char pieceType = moveNotation.First();
            string[] moveNotationComponents = moveNotation.Split('x');
            bool isPawnMove = char.IsLower(moveNotation[0]);
            if (isPawnMove)
                pieceType = 'P';

            pieceType = pieceColor == Piece.ColorType.White ? char.ToUpper(pieceType) : char.ToLower(pieceType);
            string letterToCoordinates = moveNotation[^2..];
            char? file = null;
            char? row = null;

            if (moveNotationComponents.Length == 2 && moveNotationComponents[0].Length == 2)
            {
                char specificPiece = moveNotationComponents[0][1];
                if (char.IsLetter(specificPiece))
                    file = specificPiece;
                else if (char.IsDigit(specificPiece))
                    row = specificPiece;
            }

            Dictionary<string, Piece> pieces = GetPiecesOfType(chessBoard, pieceType);
            var availableMoves = new HashSet<string>();

            foreach (var piece in pieces)
            {
                string pieceCoordinates = piece.Key;

                char pieceFile = pieceCoordinates[0];
                char pieceRow = pieceCoordinates[1];

                if (file.HasValue && pieceFile != file.Value)
                    continue;
                else if (row.HasValue && pieceRow != row.Value)
                    continue;

                availableMoves = piece.Value.GetAvailableMoves();

                if (availableMoves.Contains(letterToCoordinates))
                    return pieceCoordinates;
            }

            return null;
        }

        /// <summary>
        /// Converts for example Nf3 to g2f3 based on the given fen
        /// </summary>
        /// <param name="fen"></param>
        /// <param name="moveNotation">Nf3</param>
        /// <returns>"g2f3"</returns>
        public static string ConvertToMoveNotationCoordinates<T>(T chessBoard, Piece.ColorType pieceColor, string moveNotation) where T : ChessboardGenerator
        {
            if (moveNotation == "0-0" || moveNotation == "O-O")
                return chessBoard.boardColorOrientation == Piece.ColorType.Black ? "e8g8" : "e1g1";
            else if (moveNotation == "0-0-0" || moveNotation == "O-O-O")
                return chessBoard.boardColorOrientation == Piece.ColorType.Black ? "e8c8" : "e1c1";

            moveNotation = moveNotation.Replace("+", "").Replace("#", "");
            string fromCoordinates = GetLetterFromCoordinates(chessBoard, pieceColor, moveNotation);
            string toCoordinates = GetToCoordinatesString(moveNotation);

            return fromCoordinates + toCoordinates;
        }

        public static string GetPositionFenFromChessBoardPieces(Dictionary<string, Piece> pieces)
        {
            string fen = "";

            for (int row = 7; row >= 0; row--)
            {
                int emptySquares = 0;
                for (int column = 0; column < 8; column++)
                {
                    string currentSquare = GetLetterCoordinates(new Piece.Coordinates<int>(column + 1, row + 1));
                    pieces.TryGetValue(currentSquare, out Piece piece);
                    if (piece == null)
                        emptySquares++;
                    else
                    {
                        if (emptySquares > 0)
                            fen += emptySquares;
                        fen += piece.pieceType;
                        emptySquares = 0;
                    }
                }

                if (emptySquares > 0)
                    fen += emptySquares;

                if (row > 0)
                    fen += "/";
            }

            return fen;
        }
    }
}

