---
layout: post
title: "LINQ Sorgusu mu? ForEach mi? Bir Türlü Karar Veremedim"
date: 2010-05-27 20:40:00 +0300
categories:
  - csharp
  - linq
tags:
  - csharp
  - language-integrated-query
  - foreach
---
Bilim Kurgu fanatiklerinin kafasında her zaman hayranı oldukları filmlerden kesitler, sahneler kalır. Matrix filmini izleyenler eminimki Neo'ya uzatılan kırmızı ve mavi hap serenatını gayet iyi hatırlayacaktır. Morpheus haplardan birisinde Alice Harikalar Diyarının kapılarını ardına kadar açabileceğini ifade ederken, diğer hapı yuttuğunda, Neo'nun yatağında hiç bir şey olmamış gibi uyanacağını ve tüm bunların bir hayalden ibaret olduğunu düşüneceğini belirtir. Tabi Neo amacına ulaşmak için zaten hangi hapı içmesi gerektiğini biliyordur ki son bölümde aslında gerçekten hapı yutmaktadır

![blg196_GirisNew.jpg](/assets/images/2010/blg196_GirisNew.jpg)

![Yell](/assets/images/2010/smiley-yell.gif)

Bizde yazılımcılar olarak bazen karar verirken tabir yerinde ise sürüncemede kalabiliriz. Böyle durumlarda ufak tefek gözüken noktaların aslında çok büyük riskler taşıdığını da düşünmemiz gerekmektedir. Çünkü karar vermek için basit bir kaç test kodu çok işimize yarayacaktır. İşte bu yazımızda böyle bir konuya değiniyor olacağız.

Aslında konunun çıkış noktası [Microsoft Teknoloji Günleri Akşam Sınıfındaki](Microsoft Teknoloji Günleri Akşam Sınıfında Buluşalım.md)bir meslektaşımın sorusu oldu. Değerli meslektaşım uygulama kodunda koleksiyon bazlı sorgulamaları gerçekleştirirken pek çok vakada foreach döngülerini tercih ettiğini söyledi. Tabi her durumda değil. Bende bu noktada aynı amaca hizmet eden bir LINQ sorgusu ile ForEach çalışması arasındaki performans farklılıklarını irdelemeye karar verdim. Nitekim performans her zaman için karar vermeden önem arz eden kriterlerden birisidir. Anlayacağınız basit bir test ve sonuçlarını irdeliyor olacağız bu kısa yazımızda.

Örnek uygulamamızda Enumerable.Range metodu yardımıyla elde edilen bir int sayı dizisi içerisinde 2 ile tam bölünebilen sayıların adedini hesap ettirmekteyiz. Tahmin edeceğiniz üzere bu tip bir işlemi LINQ sorgusu yardımıyla anlamlı bir kod ifadesi ile yerine getirebiliriz. Ayrıca bunu bir foreach döngüsü ile de gerçekleştirebiliriz. İşte test kodlarımız.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LINQForEachPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i < 10; i++)
            {
                IEnumerable<int> range = Enumerable.Range(i,(i+1)*10000000);
                WithLinq(range);
                WithForeach(range);
            }
            
        }

        static void WithLinq(IEnumerable<int> range)
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();

            int count = (from i in range
                         where i % 2 == 0
                         select i).Count<int>();
            Console.WriteLine(count.ToString());

            sWatch.Stop();
            Console.WriteLine("LINQ Total Time : {0}",sWatch.ElapsedMilliseconds.ToString());
        }

        static void WithForeach(IEnumerable<int> range)
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();

            int count = 0;
            foreach (int i in range)
            {
                if (i % 2 == 0)
                    count++;
            }
            Console.WriteLine("{0}",count.ToString());

            sWatch.Stop();
            Console.WriteLine("ForEach Total Time : {0}", sWatch.ElapsedMilliseconds.ToString());
        }
    }
}
```

Örnekte arka arkaya 10 deneme yapılmaktadır. WithLinq metodu LINQ sorgusunu kullanarak ikiye tam bölünen sayıların adedini vermektedir. WithForeach metodu ise aynı işlemi foreach döngüsü yardımıyla gerçekleştirmektedir. Stopwatch tipi yardımıyla her hesaplamanın toplam süresi bulunmaktadır. Uygulamanın Intel çift çekirdek işlemcili, 4Gb Ram'i olan makinemdeki çalışma zamanı sonuçlarından bir tanesi aşağıdaki ekran görütüsündeki gibidir.

![blg196_FirstReport.gif](/assets/images/2010/blg196_FirstReport.gif)

Aslında her zaman için süreler farklı olacaktır ancak grafiksel eğriler benzer olacaktır. Durumun daha net bir şekilde görülmesi için değerlerin Excel üzerinde Chart olarak gösterilmesi yeterlidir. İşte sonuçlar.

![blg196_ExcelReport.gif](/assets/images/2010/blg196_ExcelReport.gif)

Görüldüğü üzere foreach döngüsü ile yapılan hesaplamalar değer aralığı büyüse dahi LINQ sorgusuna göre daha kısa sürede icra edilmektedir. Buna göre foreach'in daha hızlı olduğunu söyleyebilir miyiz? Bu senaryo için evet.

![Wink](/assets/images/2010/smiley-wink.gif)

Ama bildiğiniz üzere LINQ daha karmaşık sorgular yazılması noktasında elbetteki iç içe geçecek sayısız foreach kullanımından çok daha etkili bir yöntemdir. Ancak başta da belirttiğimiz gibi insan bir an için hangi hapı yutacağına karar veremiyor. Tabi farklı sorgulama senaryoları ile farklı denemeler yaparak karşılaştırmalara devam etmekte yarar olabilir. Bu kutsal görevi de siz değerli okurlarıma bırakıyorum. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[LINQForEachPerformance.rar (23,30 kb)](/assets/files/2010/LINQForEachPerformance.rar) [Örnek Visual Studio 2010 Ultimate ile geliştirilmiş ve test edilmiştir]
