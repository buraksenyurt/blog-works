---
layout: post
title: "Tasarım Desenleri - Interpreter"
date: 2009-08-03 06:30:00 +0300
categories:
  - tasarim-kaliplari-design-patterns
tags:
  - tasarim-kaliplari-design-patterns
  - csharp
  - generics
---
Yandaki legoya baktığımızda sanıyorum ki hepimizin aklına Romalılar gelmektedir. Aslında benim aklıma Ben Hur filmi ve müthiş atlı araba yarışı sahneleri geliyor. Her neyse...

![blg55_Giris.jpg](/assets/images/2009/blg55_Giris.jpg)

Romalılar, Mısırlıların fikirlerinden yola çıkarak harfler ile ifade edilebilen bir sayı sistemini geliştirmiştir. Bu nedenle mutlaka okul hayatımızın bir döneminde Roma Rakamları ile karşılaşmışızdır. Aslında son derece eğlenceli bir sayı sistemidir ve bazı filmlerin sonunda, çevrildikleri yıllar genellikle Roma rakamları ile ifade edilmektedir.

Tabi Romalılar, geliştirdikleri bu sayı sistemlerinin bir gün gelipte GOF'un tasarım kalıplarından birisine ilham vereceklerini eminimki düşünmemiştir. (Gerçi yazılım teknolojilerindeki pek çok sorunsalın çözümünde tarihten dersler alınmıştır. Örneğin Microsoft Solution Framework (MSF) eğitim materyallerinde Kartaca savaşından bahsedildiğini çok iyi hatırlarım ![Wink](/assets/images/2009/smiley-wink.gif)) İlhamı alan desen Interpreter tasarım kalıbıdır. Aslında kalıbın amacını anlamak için örnek senaryolara bir bakalım.

Diyelim ki çalışma ortamımız içerisinde şöyle bir bilgi yer alıyor. "MDCLXIV". Hatta bu bilgi, tarihsel kaynaklar ile ilişkili bir veritabanı giriş ekranından aynen bu metin formatında geliyor olsun. Ne varki, bu string bilgi yerine sayısal karşılığının bulunmasının daha önemli olduğu açıktır. Nitekim sayısal değer olması halinde bazı tarih bazlı hesaplamalar daha kolay yapılabilecektir. MDCLXIV değerinin karşılığı aslında 1664' tür. Nasıl mı? Aşağıdaki satıra geçmeden önce kağıt ve kalemi alıp hatırlamaya ve çözmeye çalışın

![Wink](/assets/images/2009/smiley-wink.gif)

MDCLXVI = (M=1000)+(D=500)+(C=100)+(L=50)+(X=10)+(V=5)-(I=1) = 1664

Çok güzel. Peki programatik ortamda bu tip bir ifadeyi kim, nasıl yorumlayacaktır? İşte Interpreter tasarım kalıbının ana fikri bu tip ifadelerden oluşan bazı özel veri gramerlerini yorumlayabilecek bir yapının oluşturulması için bir model sunmaktır. Bu örnek son derece popülerdir. Pek çok kaynakta (başta DoFactory ve OODesign) Roma rakamlarının sayıya dönüştürülmesi işlemlerinin tasarım kalıbına örnek bir senaryo olarak sunulduğunu görebilirsiniz.

Hemen konuya farklı bir örnekle devam edelim. Söz gelimi günün tarihini sistemde "MM - DD - YYYY" şeklinde elde etmek istediğimizi farzedelim (Tabi hile yapıp DateTime fonksiyonlarını kullanmıyoruz). Bunun için günün tarihini, formatta gösterilen şekilde sunmamız yeterli olacaktır. Ama olayı birde şu açıdan ele alalım. Burada tarih bilgisi için bir veri gramerimiz olduğunu düşünelim. Buna göre, günün tarihini sistemin o anki ihtiyaçları doğrultusunda "MM - DD - YYYY", "DD - MM - YYYY", "D - MMMM - YY", "DD - YYYY", "MMMM - YY" vb formatlara göre elde etmekte isteyebiliriz. Şimdi durum biraz değişti sanırım

![Wink](/assets/images/2009/smiley-wink.gif)

Hımmm... Bu durumda programatik tarafta ayrı ayrı parser yazmak çok da mantıklı olmayacaktır. Ne yapılabilir? Interpreter deseni ile bu tarih gramerini kolayca yorumlayabilir ve günün tarihinin istediğimiz formatta sunulmasını sağlayabiliriz.

Elbette başka örneklerde vermek mümkündür. Söz gelimi çok geliştirilmemekle birlikte kural işletme motorları (Rule Engine) söz konusu tasarım kalıbını sıklıkla kullanırlar. Buna göre kuralı oluşturan ifadeler ayrıştırılarak yorumlanır ve genellikle boolean (true/false) sonuçlar üretilir. Bir başka deyişle içeriğin, tanımlanan bir kurala uygun olup olmadığının kontrolü yapılır. Uygunluk true anlamındadır. Tabi desenin uygulanış biçiminde mantıksal değerlerin üretilmesi zorunlu değildir. Roma rakamı örneğinde bu açıkça görülmektedir.

Oldukça heyecan uyandıran bir desen olmasına karşılık, şekli belirli ve düzgün olan gramer ifadelerinde (Formal Grammer) değerlendirildiği için kısıtlı bir kullanım alanı söz konusudur. Sanıyorumki dofactory.com sitesinde desenin kullanım oranının neden %20' ler seviyesinde kaldığını bu cümle açıklamaktadır. Dilerseniz heyecanımızı kırmayalım ve desene ait UML şeması ile yolumuza devam edelim.

![blg55_uml.gif](/assets/images/2009/blg55_uml.gif)

Aktörlerimizden Context, yorumlanacak içeriği taşımaktadır. Genellikle ifade bütününü ve yorumlama sonucunu kendi içerisinde taşıyan bir tip olarak düşünülebilir. Context içerisinde değerlendirilmesi gereken her bir parçanın yorumlanması operasyonunu ise AbstractExpression tipi sunmaktadır. Bu tip, abstract class olarak tasarlanır ve grameri yorumlama kısımlarına ilişkin ana iş mantığını (Business Logic) üstlenebilir. Bazı durumlarda interface olarak tasarlandığınıda ve yorumlama işinin kendisinden türeyen tiplere bırakıldığını görebiliriz. UML şemasında dikkat çekici noktalardan birisi TerminalExpression ile NonterminalExpression isimli iki farklı Exrepssion tipi olmasıdır.

Aslında burada örnekler üzerinden hareket etmemizde yarar vardır. Roma rakamları örneğimizde her bir harf aslında TerminalExpression tipinden sınıflar içerisinde değerlendirilir. Ancak bir kural motorunda, ifadeler arasında bazı semboller ile işlemler yapılması gerekiyor olabilir. Örneğin iki Terminal (yada Nonterminal) ifadenin or veya and ile bağlanması yada matematiksel operasyona sokulması gibi. İşte bu tip durumlarda kullanılan ara semboller (and, or, + vb...) NonterminalExpression tipleri içerisinde değerlendirilir. Şemadan görüleceği üzere, NonTerminalExpression tipinden AbstractExpression tipine doğru tanımlanmış bir Aggregation ilişki söz konusudur. Yani NonTerminalExpression kendi içerisinde AbstractExpression türevli referansları taşıyabilmelidir. Bu gereklidir, nitekim NonTerminal sembollerin Terminal ifadeleri üzerinde değerlendirilmesi söz konusudur. Söz gelimi and operasyonunun uygulanması istenen durumlarda iki adet operand'ın (TerminalExpression) olması gerekir. Bunlar NonTerminalExpression içerisinde birer üye olarak bulunmalı ve initialize edilmelidirler. Bu nedenle bir Aggregation ilişkisi söz konusudur.

Artık bir örnek ile devam edebiliriz. Senaryo bulmak konusunda sıkıntımız olsada amacımızın desenin nasıl uygulandığını kavramak olduğunu bir kez daha hatırlatalım. Konuyu kolay bir şekilde ele almak için NonTerminalExpression nesnelerini hesaba katmayacağız. Elimizde bir projenin içerisinde yer alan çalışanların sembolik tanımlamalarını string bazlı taşıyan bir Context tipi olduğunu düşünelim. Örneğin mimarlar için A, danışmanlar için C, uzman geliştiriciler için S ve geliştiriciler için D harflerini göz önüne alabiliriz. Buna göre, örneğin ACSSDDDD şeklindeki bir metnin bizim için anlamı,

ACSSDDDD = 1 Architecture + 1 Consultant + 2 Senior Developer + 4 Junior Developer

şeklinde olacaktır.

Bu metinsel bilgidende bir projenin adam başı maliyetini çıkartabildiğimizi düşünebiliriz. Dolayısıyla string ifadeyi tarayıp bize sayısal değer döndürecek bir yorumlayıcı modele ihtiyacımız var (Burada string bir içeriği basit olarak alıp parse etmeyi hedeflemediğimizi belirtelim). Buna göre A, C, S ve D harflerinin aslında birer TerminalExpression tipi olarak ifade edilebileceğini düşünebiliriz. İşte örneğimize ait sınıf diagramımız,

![blg55_1.gif](/assets/images/2009/blg55_1.gif)

ve kod içeriğimiz.

```csharp
using System;
using System.Collections.Generic;

namespace Interpreter
{
    // Context class
    class Context
    {
        public string Formula { get; set; }
        public int TotalPoint { get; set; }
    }

    // Expression
    abstract class RoleExpression
    {
        public abstract void Interpret(Context context);
    }

    #region Terminal Expression Sınıfları

    // TerminalExpression
    class ArchitectureExpression
        : RoleExpression
    {
        public override void Interpret(Context context)
        {
            if (context.Formula.Contains("A"))
            {
                context.TotalPoint += 5;
            }
        }
    }

    // TerminalExpression
    class ConsultantExpression
        : RoleExpression
    {
        public override void Interpret(Context context)
        {
            if (context.Formula.Contains("C"))
                context.TotalPoint += 10;
        }
    }

    // TerminalExpression
    class SeniorExpression
        : RoleExpression
    {
        public override void Interpret(Context context)
        {
            if (context.Formula.Contains("S"))
                context.TotalPoint += 15;
        }
    }

    // TerminalExpression
    class DeveloperExpression
        : RoleExpression
    {
        public override void Interpret(Context context)
        {
            if (context.Formula.Contains("D"))
                context.TotalPoint += 20;
        }
    }

    #endregion

    // Client
    class Program
    {
        static List<RoleExpression> CreateExpressionTree(string formula)
        {
            // Expression ağacı oluşturulur
            List<RoleExpression> tree = new List<RoleExpression>();

            foreach (char role in formula)
            {
                if (role == 'A')
                    tree.Add(new ArchitectureExpression());
                else if (role == 'S')
                    tree.Add(new SeniorExpression());
                else if (role == 'D')
                    tree.Add(new DeveloperExpression());
                else if (role == 'C')
                    tree.Add(new ConsultantExpression());
            }
            return tree;
        }

        static void RunExpression(Context context)
        {
            foreach (RoleExpression expression in CreateExpressionTree(context.Formula))
            {
                expression.Interpret(context); // TerminalExpression tiplerine ait harf sembolleri buradaki metod çağrısındada gönderilebilir.
            }
            Console.WriteLine("{0} için maliyet puanı {1}", context.Formula, context.TotalPoint);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Architecture = 5, Consultant=10, Senior=15,Developer=20\n");
            // 1 Architect, 1 Consultan, 2 Senior Developer , 4 Junior Developer
            Context context = new Context { Formula = "ACSSDDDD" };
            RunExpression(context);

            // 1 Consultant, 1 Senior Developer, 2 Developer
            context = new Context { Formula = "CSDD" };
            RunExpression(context);

            // 1 Consultant, 1 Senior Developer, 2 Developer
            context = new Context { Formula = "SD" };
            RunExpression(context);            
        }
    }
}
```

Dikkat edileceği üzere Main metodunda bir Expression Tree oluşturulmaktadır. Bu Expression Tree'nin modellenmesi için string ifade içerisindeki tüm harfler tek tek dolaşılır ve uygun olan TerminalExpression nesne örnekleri üretilip, ağaca eklenir. Sornasında ise ağaç içerisindeki her bir TerminalExpression üzerinden Interpret metodu çalıştırılarak bir yorumlama işleminin gerçekleştirilmesi sağlanır. Yorumlama işlemi bu örnek için, karşılaşılan harflere göre bir puanlamanın, Context nesne örneği içerisindeki TotalPoint özelliğine yansıtılmasıdır. İşte uygulamanın çalışmasının sonucu.

![blg55_2.gif](/assets/images/2009/blg55_2.gif)

Yaptığımız bu basit yorumlayıcı sadece metinsel bir ifade bütününü yorumlayarak ele almıştır. Aslında yapılan iş, berlirli bir grameri alıp sınıflara dönüştürmekle alakalıdır. Bu dönüştürme işlemi sırasında devreye Interpreter kalıbının aktörleri girmektedir. Grammer içerisinde yer alan herhangibir parça aslında bir TerminalExpression veya NonTerminalExpression olarak birer sınıfa dönüşür ve AbstractExpression tipi içerisinde veya türevlerinde işlenir. Sonrasında ise istemci uygulama, bir Expression Tree bütününü, kullandığı bir Context tipi üzerinde çalıştırır. Tabiki desenin bu basit uygulanış şekli dışında kural motorlarında olduğu gibi NonTerminalExpression tiplerininde işin içerisinde girdiği daha karmaşık uyarlamaları vardır. (Dikkat edeceğiniz üzere birleşik bir metin var. Arada boşluklar veya aritmetiksel operatörler yok) Bu uyarlamalardan basit bir örneğini ilerleyen blog yazılarımdan birisinde aktarmaya çalışıyor olacağım. Böylece geldik bir tasarım deseninin daha sonuna. Tekrardan görüşünceye dek hepinze mutlu günler dilerim.

[Interpreter.rar (253,31 kb)](/assets/files/2009/Interpreter.rar)
