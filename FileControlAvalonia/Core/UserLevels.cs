using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public enum UserLevels
    {
        [Description("Оператор")]
        Operator = 3,
        [Description("Куратор ИБ")]
        Kurator = 4,
        [Description("Инженер")]
        Ingeneer = 2,
        [Description("Администратор")]
        Admin = 1,
        [Description("Прочие")]
        Nobody = 6
    }
}
