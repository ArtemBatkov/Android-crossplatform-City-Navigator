using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Models
{
    public class Person
    {
        private string Name;
        private int Age;

        public void setName(string name) { Name = name; }
        public void setAge(int age) { Age = age; }

        public int getAge() => Age;
        public string getName() => Name;   
    }
}
