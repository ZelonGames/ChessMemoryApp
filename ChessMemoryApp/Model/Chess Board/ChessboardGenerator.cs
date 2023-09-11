using ChessMemoryApp.Model.Chess_Board.Pieces;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Model.CourseMaker;
using ChessMemoryApp.Model.Lichess.Lichess_API;

namespace ChessMemoryApp.Model.Chess_Board
{
    public class ChessboardGenerator
    {
        public event Action<MovedPieceData> MovedPiece;
        public event Action<MovedPieceData> ChangedPieces;

        public Dictionary<string, Piece> pieces = new();
        public FenSettings fenSettings = new();

        public Piece.ColorType boardColorOrientation;
        public MoveNotationGenerator moveNotationHelper;
        public bool IsEmpty => pieces.Count == 0;

        public ChessboardGenerator(bool playAsBlack)
        {
            boardColorOrientation = playAsBlack ? Piece.ColorType.Black : Piece.ColorType.White;
            fenSettings.EnableAllCastleMoves();
            fenSettings.SetColorToPlay(FenSettings.FenColor.GetColorFromPieceColor(Piece.GetOppositeColor(boardColorOrientation)));
        }

        // TODO: Remove this
        public ChessboardGenerator(Piece.ColorType boardColorOrientation) :
            this(boardColorOrientation == Piece.ColorType.Black)
        {

        }

        public void AddPiecesFromFen(string fen)
        {
            if (!FenHelper.IsValidFen(fen))
                return;

            var piecesToRemove = new List<Piece>();
            var piecesToAdd = new Dictionary<string, char>();

            Dictionary<string, char?> newPieces = FenHelper.GetPiecesFromFen(fen);

            if (!IsEmpty)
            {
                foreach (var piece in pieces)
                {
                    bool isPieceCaptured =
                        newPieces[piece.Key].HasValue &&
                        newPieces[piece.Key].Value != piece.Value.pieceType;

                    bool isNewPieceSameAsCurrentPiece =
                        newPieces[piece.Key].HasValue &&
                        newPieces[piece.Key].Value == piece.Value.pieceType;

                    if ((!newPieces[piece.Key].HasValue || isPieceCaptured) &&
                        pieces.ContainsKey(piece.Key) && !isNewPieceSameAsCurrentPiece)
                        piecesToRemove.Add(piece.Value);
                }
            }

            foreach (var newPiece in newPieces)
            {
                bool isPieceCaptured =
                    pieces.ContainsKey(newPiece.Key) &&
                    newPiece.Value.HasValue &&
                    pieces[newPiece.Key].pieceType != newPiece.Value.Value;

                if (!pieces.ContainsKey(newPiece.Key) && newPiece.Value.HasValue ||
                    isPieceCaptured)
                    piecesToAdd.Add(newPiece.Key, newPiece.Value.Value);
            }

            foreach (var pieceToRemove in piecesToRemove)
                RemovePiece(pieceToRemove);

            foreach (var pieceToAdd in piecesToAdd)
                TryAddPieceToSquare(pieceToAdd.Value, pieceToAdd.Key);

            var removedPieces = new Dictionary<string, char>();
            foreach (var pieceToRemove in piecesToRemove)
                removedPieces.Add(pieceToRemove.coordinates, pieceToRemove.pieceType);

            ChangedPieces?.Invoke(new MovedPieceData(removedPieces, piecesToAdd, "", "", false));
        }

        public void RemovePiece(Piece piece)
        {
            pieces.Remove(piece.coordinates);
        }

        public void ClearBoard()
        {
            pieces.Clear();
        }

        private TeleportedPieceResult TeleportPiece(string moveNotationCoordinates)
        {
            var removedPieces = new Dictionary<string, char>();
            var addedPieces = new Dictionary<string, char>();
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveNotationCoordinates);
            string fromCoordinates = BoardHelper.GetFromCoordinatesString(moveNotationCoordinates);

            pieces.TryGetValue(fromCoordinates, out Piece movedPiece);
            if (movedPiece == null)
                return new TeleportedPieceResult();

            pieces.TryGetValue(toCoordinates, out Piece capturedPiece);
            if (movedPiece is Pawn)
            {
                string enPassantSquare = fenSettings.GetEnPassantSquare();
                if (enPassantSquare == toCoordinates)
                {
                    // The captured pawn is behind the moved piece after an en passant move
                    int captureDirection = movedPiece.color == Piece.ColorType.White ? -1 : 1;
                    Piece.Coordinates<int> numberCoordinates = BoardHelper.GetNumberCoordinates(toCoordinates);
                    numberCoordinates.Y += captureDirection;
                    string captureCoordinates = BoardHelper.GetLetterCoordinates(numberCoordinates);
                    capturedPiece = pieces[captureCoordinates];
                }
            }

            if (capturedPiece != null)
            {
                removedPieces.Add(capturedPiece.coordinates, capturedPiece.pieceType);
                RemovePiece(capturedPiece);
            }

            removedPieces.Add(movedPiece.coordinates, movedPiece.pieceType);
            RemovePiece(movedPiece);
            TryAddPieceToSquare(movedPiece.pieceType, toCoordinates);
            addedPieces.Add(toCoordinates, pieces[toCoordinates].pieceType);

            return new TeleportedPieceResult(removedPieces, addedPieces, true);
        }

        public MovedPieceData MakeMove(string moveNotationCoordinates, bool updateBoardState = true)
        {
            MovedPieceData movedPieceData;
            string fromCoordinates = BoardHelper.GetFromCoordinatesString(moveNotationCoordinates);
            string toCoordinates = BoardHelper.GetToCoordinatesString(moveNotationCoordinates);

            TeleportedPieceResult teleportedPieceResult = TryMakeCastleMove(fromCoordinates, toCoordinates);
            if (teleportedPieceResult.madeMove)
            {
                movedPieceData = new MovedPieceData(teleportedPieceResult.removedPieces, teleportedPieceResult.addedPieces, fromCoordinates, toCoordinates, updateBoardState);
                MovedPiece?.Invoke(movedPieceData);
                ChangedPieces?.Invoke(movedPieceData);
                return movedPieceData;
            }

            teleportedPieceResult = TeleportPiece(fromCoordinates + toCoordinates);
            if (teleportedPieceResult.madeMove)
            {
                movedPieceData = new MovedPieceData(teleportedPieceResult.removedPieces, teleportedPieceResult.addedPieces, fromCoordinates, toCoordinates, updateBoardState);
                MovedPiece?.Invoke(movedPieceData);
                ChangedPieces?.Invoke(movedPieceData);
                return movedPieceData;
            }

            return new MovedPieceData();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromCoordinates"></param>
        /// <param name="toCoordinates"></param>
        /// <returns>returns true if a castle move was made</returns>
        private TeleportedPieceResult TryMakeCastleMove(string fromCoordinates, string toCoordinates)
        {
            var removedPieces = new Dictionary<string, char>();
            var addedPieces = new Dictionary<string, char>();
            TeleportedPieceResult teleportedPieceResult;

            pieces.TryGetValue(fromCoordinates, out Piece king);
            if (king == null || king is not King)
                return new TeleportedPieceResult();

            string moveNotationCoordinates = fromCoordinates + toCoordinates;
            char row = king.color == Piece.ColorType.White ? '1' : '8';

            // Short Castle
            Piece rook = pieces["h" + row];
            if (rook != null && moveNotationCoordinates == $"e{row}g{row}" || moveNotationCoordinates == $"e{row}h{row}")
            {
                toCoordinates = "g" + row;
                teleportedPieceResult = TeleportPiece(fromCoordinates + toCoordinates);
                removedPieces.UnionWith(teleportedPieceResult.removedPieces);
                addedPieces.UnionWith(teleportedPieceResult.addedPieces);

                teleportedPieceResult = TeleportPiece($"h{row}f{row}");
                removedPieces.UnionWith(teleportedPieceResult.removedPieces);
                addedPieces.UnionWith(teleportedPieceResult.addedPieces);

                return new TeleportedPieceResult(removedPieces, addedPieces, true);
            }

            // Long Castle
            rook = pieces["a" + row];
            if (rook != null && moveNotationCoordinates == $"e{row}c{row}" || moveNotationCoordinates == $"e{row}a{row}")
            {
                toCoordinates = "c" + row;
                teleportedPieceResult = TeleportPiece(fromCoordinates + toCoordinates);
                removedPieces.UnionWith(teleportedPieceResult.removedPieces);
                addedPieces.UnionWith(teleportedPieceResult.addedPieces);

                teleportedPieceResult = TeleportPiece($"a{row}d{row}");
                removedPieces.UnionWith(teleportedPieceResult.removedPieces);
                addedPieces.UnionWith(teleportedPieceResult.addedPieces);

                return new TeleportedPieceResult(removedPieces, addedPieces, true);
            }

            return new TeleportedPieceResult();
        }

        public Piece TryAddPieceToSquare(char pieceType, string squareCoordinates)
        {
            if (pieces.ContainsKey(squareCoordinates))
                return null;

            string className = Piece.pieceNames[char.ToUpper(pieceType)];
            Type type = Type.GetType("ChessMemoryApp.Model.Chess_Board.Pieces." + className);
            Piece piece = (Piece)Activator.CreateInstance(type, this, pieceType);
            piece.coordinates = squareCoordinates;
            pieces.Add(piece.coordinates, piece);

            return piece;
        }

        public Dictionary<string, Piece> GetPieceOfType(char pieceType)
        {
            var pieces = new Dictionary<string, Piece>();

            foreach (var piece in this.pieces)
            {
                if (piece.Value.pieceType == pieceType)
                    pieces.Add(piece.Key, piece.Value);
            }

            return pieces;
        }

        public Piece GetPiece(string coordinates)
        {
            pieces.TryGetValue(coordinates, out Piece piece);
            return piece;
        }

        public List<Piece> GetPiecesByColor(Piece.ColorType color)
        {
            var pieces = new List<Piece>();

            foreach (var piece in this.pieces)
            {
                if (piece.Value.color == color)
                    pieces.Add(piece.Value);
            }

            return pieces;
        }

        public string GetFen()
        {
            string fen = BoardHelper.GetPositionFenFromChessBoardPieces(pieces);
            fen += fenSettings.GetAppliedSettings(FenSettings.SpaceEncoding.SPACE);

            return fen;
        }
    }
}
