using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidTPC {
    internal class TestCode {

        public static string GetCode() {

			StringBuilder sb = new();

			const int NUM = 1;


			for (int i = 0; i < NUM; i++) {
				sb.Append(Code());
            }

			return sb.ToString();

        }


        static string Code() {

			return @"1=""0123456789"".substr(2) 
@msg.show
;
";

			

			return @"@v[1] = 5 + 3";

            return @"
#toClip

#include("".\\testcode2.txt"")

@msg.show ""abc{12}def""

variable {
				var1 = v[1]
	var2 = v[5..6]
	var3 = v[12, 13, 15]
}

			@gv[1].copyTo(v[1])

class Slime {
			const number MaxHP = 30
	const number ofs_HP = 1
	code ID

	new (code id) {
		ID = id
		}

		function code HP() {
			return v[ID + ofs_HP]
		}

		function code Percentage() {
			return @{ HP * 100 / MaxHP }
		}
	}

	Slime asd = new(v[35])

@v[v[2] + 12] = Slime.Percentage
@msg.show ""test""

";


		}
    }
}
