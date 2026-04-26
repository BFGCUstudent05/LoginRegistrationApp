namespace MinesweeperWeb.Models;

using System.ComponentModel.DataAnnotations;

public class StartGameViewModel
{
    [Required]
    [Display(Name = "Board Size")]
    public int BoardSize { get; set; } = 8;

    [Required]
    [Display(Name = "Difficulty")]
    public int Difficulty { get; set; } = 1;
}
