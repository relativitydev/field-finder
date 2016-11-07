using System;
using System.Collections.Generic;
using System.Linq;

namespace TextExtractor.TestHelpers.TestingTools
{
	public class RandomGenerator : ADependency
	{
		private readonly System.Random randomGenerator = new System.Random();

		/// <summary>
		/// Returns true/false at a 50% chance 
		/// </summary>
		/// <returns></returns>
		public Boolean Boolean()
		{
			return this.randomGenerator.NextDouble() >= 0.5;
		}

		/// <summary>
		/// Returns the specified number of randomly generated boolean values 
		/// </summary>
		/// <param name="numberOfBooleans"></param>
		/// <returns></returns>
		public IEnumerable<Boolean> Booleans(Int32 numberOfBooleans = 10)
		{
			var booleans = new List<Boolean>();

			for (var i = 0; i < numberOfBooleans; i++)
			{
				booleans.Add(this.Boolean());
			}

			return booleans.AsEnumerable();
		}

		public Int32 Number(Int32 minSize = 0, Int32 maxSize = 2000000)
		{
			return this.randomGenerator.Next(minSize, maxSize);
		}

		/// <summary>
		/// Returns the specified number of randomly generated Int32 values 
		/// </summary>
		/// <param name="numberOfInts"></param>
		/// <param name="minSize"></param>
		/// <param name="maxSize"></param>
		/// <returns></returns>
		public IEnumerable<Int32> Numbers(Int32 numberOfInts = 10, Int32 minSize = 0, Int32 maxSize = 2000000)
		{
			var numbers = new List<Int32>();

			for (var i = 0; i < numberOfInts; i++)
			{
				numbers.Add(this.Number(minSize, maxSize));
			}

			return numbers.AsEnumerable();
		}

		/// <summary>
		/// Returns a new guid 
		/// </summary>
		/// <returns></returns>
		public Guid Guid()
		{
			return System.Guid.NewGuid();
		}

		/// <summary>
		/// Returns a random paragraph of Lorem Ipsum in Non-Unicode script.  If standard = true, returns the first paragraph.
		/// </summary>
		/// <returns></returns>
		public String Paragraph(Boolean standard = false)
		{
			var loremIpsum = new[] {
				"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.", 
				"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?",
			"At vero eos et accusamus et iusto odio dignissimos ducimus qui blanditiis praesentium voluptatum deleniti atque corrupti quos dolores et quas molestias excepturi sint occaecati cupiditate non provident, similique sunt in culpa qui officia deserunt mollitia animi, id est laborum et dolorum fuga. Et harum quidem rerum facilis est et expedita distinctio. Nam libero tempore, cum soluta nobis est eligendi optio cumque nihil impedit quo minus id quod maxime placeat facere possimus, omnis voluptas assumenda est, omnis dolor repellendus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Itaque earum rerum hic tenetur a sapiente delectus, ut aut reiciendis voluptatibus maiores alias consequatur aut perferendis doloribus asperiores repellat."
			};

			if (standard) { return loremIpsum[0]; }

			var index = Number(maxSize: loremIpsum.Count());

			return loremIpsum[index];
		}

		/// <summary>
		/// Returns a random word from the first paragraph of Lorem Ipsum in Non-Unicode script
		/// </summary>
		/// <returns></returns>
		public String Word()
		{
			var loremIpsum = new[] { "Lorem", "ipsum", "dolor", "sit", "amet,", "consectetur", "adipiscing", "elit,", "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua.", "Ut", "enim", "ad", "minim", "veniam,", "quis", "nostrud", "exercitation", "ullamco", "laboris", "nisi", "ut", "aliquip", "ex", "ea", "commodo", "consequat.", "Duis", "aute", "irure", "dolor", "in", "reprehenderit", "in", "voluptate", "velit", "esse", "cillum", "dolore", "eu", "fugiat", "nulla", "pariatur.", "Excepteur", "sint", "occaecat", "cupidatat", "non", "proident,", "sunt", "in", "culpa", "qui", "officia", "deserunt", "mollit", "anim", "id", "est", "laborum." };

			var index = Number(maxSize: loremIpsum.Count());

			return loremIpsum[index];
		}

		/// <summary>
		/// Returns a random paragraph of Lorem Ipsum in Unicode script.  Chinese, Arabic, Hindi supported
		/// </summary>
		/// <returns></returns>
		public String ParagraphUnicode(StringType type, Boolean standard = false)
		{
			// Cannot be certain the word will be within the first paragraph, thus cannot get a standard
			// paragraph unless a specific type is set
			if (type == StringType.All && standard == true) { standard = false; }

			var loremIpsumUnicode = new List<String>();

			var loremIpsumChinese = new List<String> {
					                        "化雑緑司覧期鹿格手革映従。高更横質読進偵来存市大断。上組須写流属私購越危際雨幸詐。時県吉冨定学近八快弁忠補名。単晴容退容際品本止掛著事。冬部情海際動医夜臨内柏紙聞止朝紹業武写紙。写演浦後取真見業芸一庁道商記置物分売。供新広会換統当験祉人和図福開担。込浦防発聞羽医団替指新朝危名践清戸。王以月氏事紙報道応勝名上。",
					                        "援長吾面京転拍在政秀以質小一禁物設。止俳込人試健著明原間事身夜円風確渡図。比予収字告伊著価読芸健言提民昭近。健条球本京時東直朝囲境世皇任政欧。両図考明作図火乱持割越深声。誉探増変人等記戦再新組込載紙由誌快置変併。正土夜課国江勝重導台支済身物売寄社護趣雑。投真最玲報生市相変羽全意多灰。産真道情重罪野権年会員建報球導備。",
					                        "出教入責導包族同広境横断具央復字斎億棋。紙掲中官馬係中転竹将百発編国捜株巡。舵間遷痛舎込量風据読門兵音映活実必。限念紀戦局打楽需経政権真携経能競使自。登拉情絶索系書英近援入世。意堀国期利体康感問測初豆富後済。報平載表暮付面政給織宮最。必目覧督捨分出省三残長質和持。相現経真疑辺謙何離倍読時。輔好校情江式断維宮式題無輪。",
					                        "更資誌必暮遣毎変久区女育記伝元治取放式察。町謙隔完目郎点読以情制介区拓高。成面談検件転援案料物盛次。界著香佐技将考象裁護店徳長予長様仕潤功著。水央討治夫一球報更示猛社姫禁委拉少規。問面将多児画機映秋一像遭強。企馬落意仏嘘量者万認成現家酢栖経。宅美影振紙審芳載生著筋老容再合中計曲企思。監山最府発掲強演代相政切指談。"
				                        };

			var loremIpsumArabic = new List<String> {
					                       "لها تم الأرض ألمانيا العالمية, حلّت يذكر واندونيسيا، ما إيو, ان دار عقبت الهجوم. فرنسا لتقليعة ويكيبيديا تحت تم. عرفها ليبين من كان, ثم بقسوة بمحاولة شيء, وبدأت أصقاع و أسر. بل سقوط الشتوية جهة, ليبين الإمداد بالمحور بها بل, تم الأمم المبرمة ويكيبيديا، بعد. بـ بسبب بزمام أخر, العاصمة أفريقيا الإقتصادية ذات تم. سابق دأبوا أن ومن, نفس أن والفرنسي الإنذار،, بـ أسر هنا؟ تعداد أطراف.",
					                       "تم إيو صفحة معاملة وهولندا،. إذ بين بهيئة للحكومة الشتوية, ان حين ٢٠٠٤ حادثة البشريةً, انه بل دخول ودول انتهت. لم ذات رجوعهم جزيرتي البشريةً, بـ قام للسيطرة اقتصادية, عن الدمج اعتداء التحالف فقد. المواد البشريةً تعد تم. ٣٠ إبّان الجديدة، انه.",
					                       "اللا المحيط مما ما, ثم الدول وأزيز الغالي به،, الحكم أجزاء فصل من. لم تونس مسؤولية أضف. ما اليها رجوعهم حتى. تعد كل الثالث ديسمبر. ليبين استمرار أن به،, سقطت عالمية الواقعة دول ثم, مع الستار إستعمل هذا. ذلك و وأزيز الأمور مكثّفة, يقوم أفريقيا الشهيرة لم ذات, من وصغار والمانيا بالإنزال وفي."
				                       };

			var loremIpsumHindi = new List<String> {
					                      "अनुवाद बिन्दुओ पत्रिका मजबुत देते सदस्य तकनिकल मुश्किल विभाजनक्षमता सेऔर संदेश अंतर्गत आंतरजाल असरकारक उनके प्रति उसीएक् प्राधिकरन विकेन्द्रित नाकर गएआप वर्तमान सोफ़्टवेर ढांचा बनाति लक्ष्य उपेक्ष करती बनाए परस्पर गयेगया मर्यादित सहयोग कलइस भाषाओ संपादक उपलब्ध पहोच। अधिकार सार्वजनिक व्याख्यान अन्य",
					                      "देखने नीचे एसलिये है।अभी किएलोग सहयोग शुरुआत प्रतिबध अविरोधता उशकी करते जिसकी माध्यम नयेलिए विभाजनक्षमता ७हल सदस्य बेंगलूर भेदनक्षमता व्रुद्धि सुस्पश्ट कराना समूह मानव ध्वनि प्रेरना निर्माता करके(विशेष समजते संस्था मुश्किल एवम् सक्षम भाषए द्वारा सिद्धांत अन्तरराष्ट्रीयकरन अथवा तकरीबन जानकारी मुक्त मानसिक अंतर्गत क्षमता। संसाध पेदा ७०है अधिकांश अंग्रेजी दौरान सहयोग होभर आधुनिक असक्षम देते दुनिया निर्देश अमितकुमार जिसकी ध्वनि सभिसमज आंतरजाल विचारशिलता समाज",
					                      "अथवा ज्यादा केन्द्रित सार्वजनिक होसके क्षमता विचारशिलता अन्तरराष्ट्रीयकरन उनके जानते होगा मेंभटृ सोफ़तवेर पुर्व शुरुआत आवश्यक लक्ष्य गएआप ध्येय माध्यम कार्यलय प्रौध्योगिकी हमारि हैं। विकास हैं। ऎसाजीस निरपेक्ष लगती पत्रिका व्याख्यान हिंदी होसके पुर्णता व्याख्यान आपके होसके कुशलता सिद्धांत हार्डवेर एकएस विकास भाषए पहोचाना सम्पर्क"
				                      };

			switch (type)
			{
				case StringType.Chinese:

					loremIpsumUnicode.AddRange(loremIpsumChinese);

					if (standard) { return loremIpsumUnicode[0]; }
					break;

				case StringType.Arabic:

					loremIpsumUnicode.AddRange(loremIpsumArabic);

					if (standard) { return loremIpsumUnicode[0]; }
					break;

				case StringType.Hindi:

					loremIpsumUnicode.AddRange(loremIpsumHindi);

					if (standard) { return loremIpsumUnicode[0]; }
					break;

				case StringType.All:

					loremIpsumUnicode.AddRange(loremIpsumChinese);
					loremIpsumUnicode.AddRange(loremIpsumArabic);
					loremIpsumUnicode.AddRange(loremIpsumHindi);
					break;
			}

			var index = Number(maxSize: loremIpsumUnicode.Count());

			return loremIpsumUnicode[index];
		}

		/// <summary>
		/// Returns a random word from the first paragraph of Lorem Ipsum in Unicode script.  Chinese, Arabic, Hindi supported
		/// </summary>
		/// <returns></returns>
		public String WordUnicode(StringType type)
		{
			var loremIpsumUnicode = new List<String>();

			var loremIpsumChinese = new List<String> {
					                        "化", "雑", "緑", "司", "覧", "期", "鹿", "格", "手", "革", "映", "従", "高", "更", "横", "質", "読", "進",
					                        "偵", "来", "存", "市", "大", "断", "上", "組", "須", "写", "流", "属", "私", "購", "越", "危", "際", "雨",
					                        "幸", "詐", "時", "県", "吉", "冨", "定", "学", "近", "八", "快", "弁", "忠", "補", "名", "単", "晴", "容",
					                        "退", "容", "際", "品", "本", "止", "掛", "著", "事", "冬", "部", "情", "海", "際", "動", "医", "夜", "臨",
					                        "内", "柏", "紙", "聞", "止", "朝", "紹", "業", "武", "写", "紙", "写", "演", "浦", "後", "取", "真", "見",
					                        "業", "芸", "一", "庁", "道", "商", "記", "置", "物", "分", "売", "供", "新", "広", "会", "換", "統", "当",
					                        "験", "祉", "人", "和", "図", "福", "開", "担", "込", "浦", "防", "発", "聞", "羽", "医", "団", "替", "指",
					                        "新", "朝", "危", "名", "践", "清", "戸", "王", "以", "月", "氏", "事", "紙", "報", "道", "応", "勝", "名",
					                        "上"
				                        };

			var loremIpsumArabic = new List<String> {
					                       "لها", "تم", "الأرض", "ألمانيا", "العالمية,", "حلّت", "يذكر", "واندونيسيا،", "ما", "إيو,",
					                       "ان", "دار", "عقبت", "الهجوم.", "فرنسا", "لتقليعة", "ويكيبيديا", "تحت", "تم.", "عرفها",
					                       "ليبين", "من", "كان,", "ثم", "بقسوة", "بمحاولة", "شيء,", "وبدأت", "أصقاع", "و", "أسر.",
					                       "بل", "سقوط", "الشتوية", "جهة,", "ليبين", "الإمداد", "بالمحور", "بها", "بل,", "تم", "الأمم",
					                       "المبرمة", "ويكيبيديا،", "بعد.", "بـ", "بسبب", "بزمام", "أخر,", "العاصمة", "أفريقيا",
					                       "الإقتصادية", "ذات", "تم.", "سابق", "دأبوا", "أن", "ومن,", "نفس", "أن", "والفرنسي",
					                       "الإنذار،,", "بـ", "أسر", "هنا؟", "تعداد", "أطراف."
				                       };

			var loremIpsumHindi = new List<String> {
					                      "अनुवाद", "बिन्दुओ", "पत्रिका", "मजबुत", "देते", "सदस्य", "तकनिकल", "मुश्किल",
					                      "विभाजनक्षमता", "सेऔर", "संदेश", "अंतर्गत", "आंतरजाल", "असरकारक", "उनके", "प्रति", "उसीएक्",
					                      "प्राधिकरन", "विकेन्द्रित", "नाकर", "गएआप", "वर्तमान", "सोफ़्टवेर", "ढांचा", "बनाति",
					                      "लक्ष्य", "उपेक्ष", "करती", "बनाए", "परस्पर", "गयेगया", "मर्यादित", "सहयोग", "कलइस", "भाषाओ",
					                      "संपादक", "उपलब्ध", "पहोच।", "अधिकार", "सार्वजनिक", "व्याख्यान", "अन्य"
				                      };

			switch (type)
			{
				case StringType.Chinese:

					loremIpsumUnicode.AddRange(loremIpsumChinese);
					break;

				case StringType.Arabic:

					loremIpsumUnicode.AddRange(loremIpsumArabic);
					break;

				case StringType.Hindi:

					loremIpsumUnicode.AddRange(loremIpsumHindi);
					break;

				case StringType.All:

					loremIpsumUnicode.AddRange(loremIpsumChinese);
					loremIpsumUnicode.AddRange(loremIpsumArabic);
					loremIpsumUnicode.AddRange(loremIpsumHindi);
					break;
			}

			var index = Number(maxSize: loremIpsumUnicode.Count());

			return loremIpsumUnicode[index];
		}

		/// <summary>
		/// Returns a random item from your array of objects 
		/// </summary>
		/// <param name="objects"></param>
		/// <returns></returns>
		public T Randomize<T>(object[] objects) where T : class
		{
			var i = this.randomGenerator.Next(0, objects.Count());

			return objects[i] as T;
		}

		public enum StringType
		{
			All,
			Chinese,
			Arabic,
			Hindi
		}
	}
}