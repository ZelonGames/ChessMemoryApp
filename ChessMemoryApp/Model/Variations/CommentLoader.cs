using ChessMemoryApp.Model.Chess_Board;
using ChessMemoryApp.Model.ChessMoveLogic;
using ChessMemoryApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMemoryApp.Model.Variations
{
    /// <summary>
    /// Loads the correct comment for the current fen
    /// </summary>
    public class CommentLoader
    {
        public delegate void LoadedCommentEventHandler(Comment loadedComment);
        public event LoadedCommentEventHandler LoadedComment;

        private readonly Editor editorComment;

        public CommentLoader(Editor editorComment)
        {
            this.editorComment = editorComment;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscribers">CustomVariationMoveNavigator, CustomVariationCommentManager</param>
        public void SubscribeToEvents(CustomVariationMoveNavigator customVariationMoveNavigator)
        {
            customVariationMoveNavigator.RevealedMove += CustomVariationCommentLoader_RevealedMove;
            customVariationMoveNavigator.GuessedCorrectMove += CustomVariationCommentLoader_GuessedCorrectMove;
        }


        public async Task LoadComment(string fen)
        {
            Comment comment = await CommentService.Get(fen);
            if (comment != null)
                editorComment.Text = comment.Text;
            else
                editorComment.Text = "";

            LoadedComment?.Invoke(comment);
        }

        private async void CustomVariationCommentLoader_GuessedCorrectMove(MoveHistory.Move moveToMake)
        {
            await LoadComment(moveToMake.fen);
        }

        private async void CustomVariationCommentLoader_RevealedMove(string fen)
        {
            await LoadComment(fen);
        }
    }
}
