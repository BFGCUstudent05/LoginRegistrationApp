using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MinesweeperWeb.Models;
using MinesweeperWeb.Services;
using System.Security.Claims;
using System.Text.Json;

namespace MinesweeperWeb.Controllers;

[Authorize]
public class GameController : Controller
{
    private readonly IBoardService _boardService;

    public GameController(IBoardService boardService)
    {
        _boardService = boardService;
    }

    [HttpGet]
    public IActionResult StartGame()
    {
        return View(new StartGameViewModel());
    }

    [HttpPost]
    public IActionResult StartGame(StartGameViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var board = new BoardModel(model.BoardSize);
        _boardService.SetupBombs(board, model.Difficulty);
        _boardService.CountBombsNearby(board);
        _boardService.PlaceReward(board);
        board.Difficulty = model.Difficulty;
        board.StartTime = DateTime.UtcNow;

        string sessionKey = GetSessionKey();
        HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(board));

        return RedirectToAction("MineSweeperBoard");
    }

    [HttpGet]
    public IActionResult MineSweeperBoard()
    {
        var board = GetBoardFromSession();
        if (board == null) return RedirectToAction("StartGame");

        var elapsed = DateTime.UtcNow - board.StartTime;
        var viewModel = new GameBoardViewModel
        {
            Board = board,
            ElapsedTime = $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}"
        };

        return View(viewModel);
    }

    [HttpPost]
    public IActionResult HandleCellClick(int row, int col)
    {
        var board = GetBoardFromSession();
        if (board == null) return RedirectToAction("StartGame");

        var cell = board.Cells[row][col];

        // Ignore clicks on already-visited cells
        if (cell.IsVisited) return RedirectToAction("MineSweeperBoard");

        if (cell.IsBomb)
        {
            cell.IsVisited = true;
            board.EndTime = DateTime.UtcNow;
            board.GameState = GameStateEnum.Lost;
            SaveBoardToSession(board);
            return RedirectToAction("Loss");
        }

        _boardService.FloodFill(board, row, col);

        if (cell.HasSpecialReward)
        {
            board.RewardsRemaining++;
        }

        var state = _boardService.DetermineGameState(board);
        board.GameState = state;

        if (state == GameStateEnum.Won)
        {
            board.EndTime = DateTime.UtcNow;
            SaveBoardToSession(board);
            return RedirectToAction("Win");
        }

        SaveBoardToSession(board);
        return RedirectToAction("MineSweeperBoard");
    }

    [HttpGet]
    public IActionResult Win()
    {
        var board = GetBoardFromSession();
        if (board == null) return RedirectToAction("StartGame");

        var elapsed = board.EndTime - board.StartTime;
        int difficultyMultiplier = board.Difficulty switch
        {
            1 => 1, 2 => 2, 3 => 3, _ => 1
        };
        int score = (int)((board.Size * board.Size * difficultyMultiplier) / Math.Max(elapsed.TotalSeconds, 1) * 1000);

        var model = new ScoreModel
        {
            Score = score,
            ElapsedTime = $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}",
            BoardSize = board.Size,
            Difficulty = board.Difficulty switch
            {
                1 => "Easy", 2 => "Medium", 3 => "Hard", _ => "Unknown"
            }
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Loss()
    {
        var board = GetBoardFromSession();
        if (board == null) return RedirectToAction("StartGame");

        // Reveal all cells for the loss screen
        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
                board.Cells[r][c].IsVisited = true;

        var elapsed = board.EndTime - board.StartTime;
        var viewModel = new GameBoardViewModel
        {
            Board = board,
            ElapsedTime = $"{(int)elapsed.TotalMinutes}:{elapsed.Seconds:D2}"
        };

        return View(viewModel);
    }

    private BoardModel? GetBoardFromSession()
    {
        string sessionKey = GetSessionKey();
        var json = HttpContext.Session.GetString(sessionKey);
        if (string.IsNullOrEmpty(json)) return null;
        return JsonSerializer.Deserialize<BoardModel>(json);
    }

    private void SaveBoardToSession(BoardModel board)
    {
        string sessionKey = GetSessionKey();
        HttpContext.Session.SetString(sessionKey, JsonSerializer.Serialize(board));
    }

    private string GetSessionKey()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return $"GameBoard_{userId}";
    }
}
