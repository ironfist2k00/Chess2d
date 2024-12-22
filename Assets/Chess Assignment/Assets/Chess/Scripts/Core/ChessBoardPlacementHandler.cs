using System;
using UnityEngine;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using Chess.Scripts.Core;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public sealed class ChessBoardPlacementHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] _rowsArray;
    [SerializeField] private GameObject _highlightPrefab;
    private GameObject[,] _chessBoard;

    internal static ChessBoardPlacementHandler Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        GenerateArray();
    }

    private void GenerateArray()
    {
        _chessBoard = new GameObject[8, 8];
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                _chessBoard[i, j] = _rowsArray[i].transform.GetChild(j).gameObject;
            }
        }
    }

    internal GameObject GetTile(int i, int j)
    {
        if (i < 0 || i >= 8 || j < 0 || j >= 8)
        {
            Debug.LogError("Invalid row or column.");
            return null;
        }

        return _chessBoard[i, j];
    }

    internal void Highlight(int row, int col)
    {
        var tile = GetTile(row, col).transform;
        if (tile == null)
        {
            Debug.LogError("Invalid tile for highlighting.");
            return;
        }

        Instantiate(_highlightPrefab, tile.position, Quaternion.identity, tile);
    }

    /*internal void Highlight(int row, int col)
    {
        List<(int, int)> moves = ChessPlayerPlacementHandler.instance.GetValidMoves();
        var tile = GetTile(row, col).transform;
        foreach (var move in moves)
        {
            
            int targetRow = move.Item1;
            int targetCol = move.Item2;

            // Get the target tile from the board
            var targetTile = ChessBoardPlacementHandler.Instance.GetTile(targetRow, targetCol);

            // Check if the target tile is valid and highlight it
            if (targetTile != null)
            {
                ChessBoardPlacementHandler.Instance.Highlight(targetRow, targetCol);
            }
        }
        Instantiate(_highlightPrefab, tile.position, Quaternion.identity, tile);
    }*/
    internal void ClearHighlights()
    {
        for (var i = 0; i < 8; i++)
        {
            for (var j = 0; j < 8; j++)
            {
                var tile = GetTile(i, j);
                if (tile == null || tile.transform.childCount <= 0) continue;

                foreach (Transform child in tile.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}

