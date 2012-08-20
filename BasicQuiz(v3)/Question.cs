using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicQuiz_v3_
{
    class Question
    {
        public int intQuestionID { get; set; }
        public string strQuestionDescription { get; set; }
        public int intAnswer1ID { get; set; }
        public string strAnswer1Description { get; set; }
        public int intAnswer2ID { get; set; }
        public string strAnswer2Description { get; set; }
        public int intAnswer3ID { get; set; }
        public string strAnswer3Description { get; set; }
        public int intCorrectAnswerID { get; set; }
        public string strCorrectAnswerDescription { get; set; }
    }
}
