---
layout: post
title: "Interpreter Tasarım Kalıbı - İkinci Randevu"
date: 2009-08-15 17:37:00 +0300
description: "Bir süre önce tasarım kalıplarından Interpreter desenini incelemiş ve konu ile ilişkili bir kural motorunun çok basit anlamda nasıl yazılabileceğini araştıracağımızdan bahsetmiştik. Interpreter tasarım kalıbında hatırlayacağınız gibi Terminal ve NonTerminal tipleri bulunmaktadır..."
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - dotnet
  - generics
---
# Interpreter Tasarım Kalıbı - İkinci Randevu
![blg64_Giris_1.jpg](/assets/images/2009/blg64_Giris_1.jpg)

Merhaba Arkadaşlar,

Bir süre önce tasarım kalıplarından [Interpreter](https://www.buraksenyurt.com/post/Tasarc4b1m-Desenleri-Interpreter.aspx) desenini incelemiş ve konu ile ilişkili bir kural motorunun çok basit anlamda nasıl yazılabileceğini araştıracağımızdan bahsetmiştik. Interpreter tasarım kalıbında hatırlayacağınız gibi Terminal ve NonTerminal tipleri bulunmaktadır. NonTerminal tipler genellikle kural motoru gibi modellerde devreye girmektedir. Kural motorlarında (Rule Engine), işletilmek istenen ifadelerin içerisinde sıklıkla operatörlerin kullanılması söz konusudur.

Örneğin and, or, >=, <, küçüktür, eşittir gibi düşünebiliriz. Dikkat ederseniz eşittir ve küçüktür gibi kelimeleri de operatörler arasına kattım. Nitekim yorumlanacak ifade (Expression) bütününü kendimiz oluşturduğumuz için istediğimiz terimleri seçmemiz son derece doğaldır. Tam bu noktada sağ üstteki resmin konu ile ne alakası olduğunu düşünebilirsiniz.

![Laughing](/assets/images/2009/smiley-laughing.gif)

Aslında bu yazımızdaki amacımız, içerisinde değişik renklerde misketleri barındıran bir kutu (ki örneğimizde string tipten generic bir koleksiyon olarak ifade edilecek) üzerinde, string bazlı mantıksal bir ifadeyi işletmektir. Örnek olarak aşağıdaki gibi bir kural tanımladığımızı göz önüne alabiliriz.

"Kirmizi ve Mavi veya Mor"

Buna göre sepet içerisindeki misketlerin rengine göre yukarıdaki kurala uyan bir durum varsa bir takım işlemlerin yapılmasını veya yapılmamasını arzu ediyoruz. Aslında ne yapılması gerektiğinin şu aşamada bir önemi yok.

![Wink](/assets/images/2009/smiley-wink.gif)

Çünkü önemli olan ilk aşama, yukarıdaki kuralı söz konusu misket sepeti üzerinde işletebilmek. Peki bunu nasıl yapacağız? Dahası yapmak için Interpreter tasarım kalıbını nasıl kullanacağız?

İlk etapta, kural ifadesi içerisindeki materyalleri göz önüne almamızda yarar var. Renkleri aslında tek bir Terminal tipi ile ifade edebiliriz. Nitekim renklerin ayrı ayrı yapacakları bir işlevsellik yok.(Elbetteki kural ifadesi içerisindeki bilgilerin gerçek hayat kural motorlarında ayrı ve farklı görevleri olabilir. Bu durumda her biri için ayrı Terminal tiplerinin tasarlanması gerekir) Diğer taraftan renkler arasında ve, veya olmak üzere iki mantıksal operatör yer almaktadır. İşte bunlar NonTerminal tipler olarak tasarlanmalıdır. Nitekim kendi içlerinde, Expression tiplerinden ikisini taşıyacaklardır ki mantıksal olarak ve, veya işlemleri gerçeklenebilsin. Tabi birde içerisinde parantezler bulunmayan bir kural ifadesi ile karşı karşıyayız. Kuralın

"Kirmizi ve (Mavi veya Mor)" olması ile

"(Kirmizi ve Mavi) veya Mor"

olmasının arasında işlem öncelikleri açısından farklılıklar bulunur. Önce parantez içlerini çalıştırmak gerekir. Tabi bizim örneğimizde parantezleri işin içerisine şu an için katmıyor olacağız. Ama size parantezleri işin içerisine katarak geliştirme yapmaya çalışmanızı şiddetle öneririm. Özellikle string biçimdeki kuralı ayrıştırırken çok zorlu bir yoldan geçeceğinizi garanti edebilirim. Öyleki kuralı bir arkadaşınız yanlışlıkla şöylede yazabilir.

"(Kirmizi ve ((Mavi veya Mor)"...Upsss!

![Wink](/assets/images/2009/smiley-wink.gif)

Peki biz kuralı nasıl ayrıştıralım. Aşağıdaki şekil bize bu anlamda bir fikir verebilir.

![blg64_ColorSchema.gif](/assets/images/2009/blg64_ColorSchema.gif)

Aslında bunun programatik taraftaki karşılığını bir ifade ağacı (Expression Tree) olarak düşünebiliriz. Ancak yazacağımız kod içerisinde Interpreter tasarım kalıbının uygulanması dışında, bu şekilde bir ifade ağacının çıkartılabilmesi için Recursive bir fonksiyonada ihtiyacımız olacaktır. Ta ta ta taaa...

![Sealed](/assets/images/2009/smiley-sealed.gif)

(Kişisel Notum: Uzun yıllar çalıştığım eğitim firmasında verdiğim.Net derslerinde, Recursive metodları anlatırken çoğunlukla Faktoryel hesabı veya Fibonacci sayılarının bulunması problemlerini dile getirdiğimi hatırlıyorum da...Gerçek hayat çok ama çok daha farklı...Geniş düşünmek, vizyonu her zaman geniş tutmak gerekiyor. Çoğu zaman göz ardı ettiğiniz bir kavram, aslında bir problemin çözümünde kritik bir rol üstlenebiliyor. Recursive bir metodun örneğimizdeki ifade ağacının çıkartılmasında üstlendiği rolde olduğu gibi...)

Çünkü ifadenin n sayıda renk ve mantık operatörü içermesi söz konusudur. Bu durumda ifade ağacı oluşturulurken ve çalıştırılırken, bir önceki ifadeyi üreten ve bunu sonraki ifadeyi üretmek için girdi olarak kullanan bir fonksiyon yazılması şarttır. Artık örneğimizi geliştirmeye ne dersiniz? Şimdi aşağıdaki sınıf diagramı ve kodları içeren Console uygulamasını yazdığımızı düşünelim.

![blg64_ClassDiagram.gif](/assets/images/2009/blg64_ClassDiagram.gif)

```csharp
using System;
using System.Collections.Generic;

namespace Interpreter
{
    // Expression Type
    abstract class RuleExpression
    {   
        public abstract bool Interpret(List<string> context);
    }

    #region Terminal Expression Types

    class ArgumentExpression
    : RuleExpression
    {
        public string Name { get; set; }

        public override bool Interpret(List<string> context)
        {
            if(context.Contains(Name))
                return true;
            else
                return false;
        }
    }

    #endregion

    #region NonTerminal Expression Types

    class AndExpression
        : RuleExpression
    {
        public RuleExpression Left { get; set; }
        public RuleExpression Right { get; set; }

        public override bool Interpret(List<string> context)
        {
            return Left.Interpret(context) && Right.Interpret(context);
        }
    }

    class OrExpression
        : RuleExpression
    {
        public RuleExpression Left { get; set; }
        public RuleExpression Right { get; set; }

        public override bool Interpret(List<string> context)
        {
            return Left.Interpret(context) || Right.Interpret(context);
        }
    }

    #endregion

    // Expression ağacını oluşturmak ve çaşlıştırmakla görevli olan sınıf
    class RuleComputer
    {
        public List<RuleExpression> Expressions { get; set; }

        public RuleComputer()
        {
            Expressions = new List<RuleExpression>();
        }

        // Expression ağacının oluşturucusu ve çalıştırıcısı olan metoddur
        public bool RunExpressionTree(string ruleSyntax,List<string> context)
        {            
            bool result = false;
            
            // Önce kural metni içerisindeki boşluklara göre elemanlar ayrılır
            string[] ruleParts = ruleSyntax.Split(' ');

            // Küçük bir kontrol. Ancak fazlasınıda yapmak gerekir :) Yazılan kural metninin geçerli olup olmadığı denetlenmelidir.
            if (ruleParts.Length < 3)
                throw new Exception("Eleman sayısı kural için yeterli değildir");

            // Expression Tree oluşturulmasına başlanır(Recursive fonksiyonu kullandığımıza dikkat edelim)
            RuleExpression longExpression = Recursive(ruleParts, 1, null);
            // Expression ağacı koleksiyona eklenir
            Expressions.Add(longExpression);

            // Koleksiyondaki her bir Expression için Interpret operasyonu çalıştırılır
            foreach (RuleExpression expression in Expressions)
            {
                result = expression.Interpret(context);
            }

            return result;
        }

        // Expression ağacının oluşturulması için kullanılan recursive fonksiyon
        // Kuralı işletmek için en soldaki ikili daldan başlayarak sağa doğru ilerliyoruz
        RuleExpression Recursive(string[] parts, int step, RuleExpression expression)
        {           
            if (step == 1) // Soldan ilk operatör ile karşılaşıldığında
            {
                if (parts[step] == "ve")
                {
                    expression = new AndExpression { Left = new ArgumentExpression { Name = parts[step - 1] }, Right = new ArgumentExpression { Name = parts[step + 1] } };
                }
                if (parts[step] == "veya")
                {
                    expression = new OrExpression { Left = new ArgumentExpression { Name = parts[step - 1] }, Right = new ArgumentExpression { Name = parts[step + 1] } };
                }
            }
            else // İlk çift içerisindeki operator geçildikten sonra, her zaman bir önceki dalın, sonradan gelen argüman ile ve/veya işlemine sokulması sağlanır
            {
                if (parts[step] == "ve")
                {
                    expression = new AndExpression { Left = expression, Right = new ArgumentExpression { Name = parts[step + 1] } };
                }
                if (parts[step] == "veya")
                {
                    expression = new OrExpression { Left = expression, Right = new ArgumentExpression { Name = parts[step + 1] } };
                }
            }

            // Recursive metoddan bir notkada çıkılması gerekecektir. Bu çıkış noktası, son operatör ele alındıktan sonrasıdır.
            if (step == parts.Length - 2)
                return expression;

            // Öteleme yapılarak sonraki çifti almak üzere aynı metod tekrar işletilir
            return Recursive(parts, step + 2, expression);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Örnek kural
            string rule = "Kirmizi ve Mavi veya Mor ve Siyah";

            // Kuralın denetleneceğin veri içeriği (Context)
            List<string> myBasket =new List<string> { "Yesil", "Kahverengi", "Lacivert", "Sari", "Mor", "Siyah" };
            RuleComputer computer = new RuleComputer();

            // Kirmizi ve Mavi = 0 && 0 => 0
            // 0 veya Mor = 0 || 1 => 1
            // 1 ve Siyah = 1 && 1 => 1
            bool result=computer.RunExpressionTree(rule,myBasket);
            Console.WriteLine(result);

            // Kirmizi ve Mavi = 0 && 0 => 0
            // 0 veya Mor = 0 || 0 => 0
            // 0 ve Siyah = 0 && 0 => 0
            myBasket = new List<string> { "Yesil", "Kahve", "Lacivert", "Beyaz" };
            Console.WriteLine(computer.RunExpressionTree(rule,myBasket));

            // Kuralı değiştirelim
            rule = "Kirmizi veya Beyaz";

            // Kirmizi veya Beyaz = 0 || 1 => 1
            Console.WriteLine(computer.RunExpressionTree(rule,myBasket));

            // Exception testidir
            // rule = "Sari";
            // Console.WriteLine(computer.RunExpressionTree(rule, myBasket));
        }
    }
}
```

Kodu dikkatlice incelemenizi öneririm.

![blg64_Scenario.jpg](/assets/images/2009/blg64_Scenario.jpg)

Tasarım kalıbımıza göre, AndExpression ve OrExpression tipleri kural içerisindeki ve, veya terimlerini ifade etmektedir. Diğer taraftan renklerin her birini ArgumentExpression tipi ile temsil ediyoruz. AndExpression ve OrExpression tipleri aynı zamanda kendi sol ve sağ taraflarındaki nesneleri kullanabilmek için RuleExpression tipinden referansları kullanıyorlar. Kodun belkide en önemli tipi RuleComputer sınıfı.

Tabir yerinde ise, Interpreter kalıbının önüne geçtiğini söyleyebiliriz. RuleComputer içerisinde yer alan RunExpressionTree metodu, ifade ağacının oluşturulması ve çalıştırılmasından sorumludur. Bu metodda kendi içerisinde Recursive olan başka bir fonksiyonu çağırmaktadır. Yazımızın başlarında hatırlayacağınız üzere örnek bir kuralı soldan sağa doğru yorumlayarak ele aldığımızı görmüştük. Burada kuralın n sayıda argüman ve operatörden oluşturulması söz konusu olduğundan, ifade ağacının çıkartılmasının tek yolu kendi kendini çağıran ve bir önceki çağırımda oluşturduğu ifadeyi kullanan bir metod yazmaktır.

Main metodu içerisinde bir kaç test kuralı yazıldığını ve işletildiğini görmekteyiz. Kuralları işletiş şekline göre, ArgumentExpression tipine ait Interpret metodu içerisinde yaptğımız tek şey, parametre olarak gelen Context (yani renk bilgilerini içeren generic List koleksiyonu) içerisinde, söz konusu referansın taşıdığı rengin olup olmadığına bakmak ve buna göre geriye true veya false sonuç döndürmektir.

Uygulamamızı debug ederek çalıştırdığımızda ise son derece güzel noktalara ulaştığımızı görebiliriz Söz gelimi ilk kuralın işletilmesi sırasında RuleComputer içerisindeki Expressions özelliğinin aşağıdaki yapıda olduğunu hemen farkedebiliriz.

![blg64_QuickWatch.gif](/assets/images/2009/blg64_QuickWatch.gif)

Dikkat edileceği üzere string tabanlı yazılan basit kuralın her bir parçası Exrpression Tree üzerinde nesnel olarak yerini almış ve birbirlerine bağlanmıştır. Bundan sonrasında kodun yapması gereken tek şey, ağacı ilk elemandan sonuncuya kadar dolaşmak ve tüm gördüğü RuleExpression türevli tipler için Interpret metodlarını çağırmaktır. Ve işte çalışma zamanı sonucu;

![blg64_Runtime.gif](/assets/images/2009/blg64_Runtime.gif)

Peki neler yapamıyoruz?

- Herşeyden önce sadece ve, veya operasyonlarına hizmet veren bir sistem söz konusu. Buna ancak operatörünüde ekleyebiliriz.
- Diğer yandan, parantez yazımına destek verilmesi söz konusu olabilir. Bu duruma Expression Tree'nin oluşturulması sırasında parantez kullanımlarını değerlendirmemiz gerekecektir.
- Kural olarak yazılan ifade bütününün, gerçekten doğru bir stilde yazıldığını denetlemek gerekir. Bitişik yazımlar yada tanımlı olmayan bir operatör (ve yerine yahu yazmış olabiliriz ![Wink](/assets/images/2009/smiley-wink.gif)) hatalara neden olabilir.
- ...

Maddeler elbetteki çoğaltılabilir. Ancak sonuçta ulaştığımız noktalardan birisi, belirli bir Context üzerinde, bizim belirlediğimiz bir kuralın işletilmesi ve sonuç olarak true yada false değere indirgenebilen bir çıktının ürettirilebilmesidir. Bir başka deyişle bu yapıyı esnetmek (örneğin true/ false haricinde diğer tiplerin üretimine destek vermek yada =,!= gibi çift taraflı karşılaştırma operasyonları hesaba katabilmek...) tamamen klavyenin başındaki geliştiricini hayal gücü ile sınırlıdır. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[InterpreterV2.rar (26,55 kb)](/assets/files/2009/InterpreterV2.rar)