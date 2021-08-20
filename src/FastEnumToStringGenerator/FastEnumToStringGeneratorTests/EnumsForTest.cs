using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastEnumToStringGenerator;

namespace FastEnumToStringGeneratorTests
{
    [ExtraFastEnum(typeof(System.Base64FormattingOptions))]
    class Placeholder
    {
        private Placeholder()
        {

        }
    }

    enum CustomEnum1
    {
        MyFirstValue = 1,
        MySecondValue = 2,
        MyThirdValue = 3,
        MyFourthValue = 4,
        MyFifthValue = 5,
        MySixthValue = 6,
        MySeventhValue = 7,
    }
    enum CustomEnum2
    {
        MyFirstValue = 1,
        MySecondValue = 2,
        MyThirdValue = 3,
        MyFourthValue = 4,
        MyFifthValue = 5,
        MySixthValue = 6,
        MySeventhValue = 7,
    }
}
