using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBDD.GestionCarpeta
{
    public class Gestion
    {
        public bool result { get; set; }

        public bool isCorrect()
        {
            return result;
        }

        public void Correct(string correctMessage = "")
        {
            result = true;
            correct = correctMessage;
        }

        public void setError(string errorMessage)
        {
            result = false;
            error = errorMessage;
        }

        public string error { get; set; }
        public string correct { get; set; }

        public dynamic data { get; set; }
    }
}
