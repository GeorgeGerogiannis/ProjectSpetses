namespace ProjectSpetses.Models
{
    public class GameAnswerViewModel
    {//change this into an all inclusive model for game answers
        public int QuestionId { get; set; }
        public string SelectedValue { get; set; } // "True" or "False", but not bool...
        public string GameType { get; set; }
    }
}
