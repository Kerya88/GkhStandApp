using GkhStandApp.Enums;

namespace GkhStandApp.Entities
{
    public class Question
    {
        //id вопроса
        public string Id { get; set; }
        //формулировка вопроса
        public string Name { get; set; }
        //тип вопроса
        public QuestionType QuestionType { get; set; }
        //вариативный ответ
        public string Answer { get; set; }
        //список ответов на вопрос
        public List<Answer>? Answers { get; set; }
    }
}
