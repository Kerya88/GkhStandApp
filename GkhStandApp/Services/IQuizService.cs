using GkhStandApp.Entities;

namespace GkhStandApp.Services
{
    public interface IQuizService
    {
        public List<Quiz> GetNowQuizzes();
        public Dictionary<string, string[]> GetMatchesROs(string enteredAddress);
        public void SendPassedQuiz(Quiz passedQuiz);
    }
}
