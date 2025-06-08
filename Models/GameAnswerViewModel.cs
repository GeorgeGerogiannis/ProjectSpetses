namespace ProjectSpetses.Models
{
    public class GameAnswerViewModel
    {//change this into an all inclusive model for game answers
        public int QuestionId { get; set; }//int not ushort???
        public string SelectedValue { get; set; } // "True" or "False", but not bool..?//Capitalized
        public string GameType { get; set; }
    }
}
