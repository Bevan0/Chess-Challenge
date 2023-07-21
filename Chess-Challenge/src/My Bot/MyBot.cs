using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    // stores piece values, None is -50 to encourage capturing
    int[] pieceValues = { -50, 100, 300, 320, 500, 1000, 10000 };

    public Move Think(Board board, Timer timer)
    {
        Random rng = new Random();
        Move[] legalMoves = board.GetLegalMoves();
        // Set to a random move incase we don't find one
        Move moveToPlay = legalMoves[rng.Next(legalMoves.Length)];
        int moveToPlayValue = 0;
        foreach (Move move in board.GetLegalMoves())
        {
            int moveValue = 0;
            PieceType capturedPieceType = board.GetPiece(move.TargetSquare).PieceType;
            moveValue += pieceValues[(int)capturedPieceType]; // if capturing a piece, adds its value

            board.MakeMove(move);

            if (board.SquareIsAttackedByOpponent(move.TargetSquare)) moveValue -= (pieceValues[(int)move.MovePieceType] - 100); // punish if move is unsafe
            if (board.IsInCheckmate()) return move; // always play M1
            if (board.IsInCheck()) moveValue += 200; // add bonus if move is a check
            if (move.IsPromotion && !board.SquareIsAttackedByOpponent(move.TargetSquare)) moveValue += 1200; // add mega bonus is move is safe promotion
            
            if (board.IsDraw()) moveValue = 0; // if move is forced draw, set value to 0

            board.UndoMove(move);

            if (moveToPlayValue < moveValue)
            {
                moveToPlay = move;
                moveToPlayValue = moveValue;
            }
        }
        return moveToPlay;
    }
}