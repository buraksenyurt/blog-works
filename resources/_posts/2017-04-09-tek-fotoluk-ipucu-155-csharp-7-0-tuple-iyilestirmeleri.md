---
layout: post
title: "Tek Fotoluk İpucu 155 - C# 7.0 Tuple İyileştirmeleri"
date: 2017-04-09 21:42:00 +0300
categories:
  - csharp-7-0
tags:
  - csharp
  - tuple
  - tuple<>
  - deconstruction
---
C# 7.0 tarafında geliştiricileri mutlu eden iyileştirmelerden birisi de Tuple tipi ile ilgili. Klasik olarak bir tip tanımı yapmamıza ihtiyaç duymadan özellikle metodlardan dönüş yaptığımız noktalarda faydalanabildiğimiz generic Tuple tipinin en büyük handikapı, üye isilmendirmeleri. Aşağıdaki kod parçasında bu durumu açık bir şekilde görebiliriz.

```csharp
using System;

namespace Classics
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = Calculate(8.2, 4.6);
            Console.WriteLine("{0},{1},{2},{3}",
                result.Item1,
                result.Item2,
                result.Item3,
                result.Item4
                );
        }

        static Tuple<double, double, double, double> Calculate(double x, double y)
        {
            return new Tuple<double, double, double, double>(
                x+y,
                x-y,
                x*y,
                x/y
                );
        }
    }
}
```

Calculate isimli kobay metodumuz x ve y değişkenleri için 4 işlem yapıp sonuçları bir Tuple tipi ile geri döndürmekte. Hoşa gitmeyen nokta bu alanlara Item1,Item2,Item3 ve Item4 gibi isimlerle erişiyor olmamız. Normalde bir sınıf yazdığımızda özellik adları ile içeriğine erişmek isteriz. Üstelik bu kullanımda geliştirici dostu pek çok dildeki yazım stilinden uzak bir tanımlama şekli söz konusu (Biraz Ruby, biraz Python, az biraz Go yazınca göze hoş gelen pratik sytnax'lara alışıyor insan) Peki C# 7.0 tarafında Tuple tipi için ne gibi yenilikler söz konusu. Aşağıdaki fotoğrafta bir kısmını görebilirsiniz.

![tfi155_1.gif](/assets/images/2017/tfi155_1.gif)

İlk göze çarpan muhtemelen Tuple diye bir anahtar kelime kullanmıyor oluşumuzdur. codes'a string türde iki değeri olan bir tuple atamaktayız. Alan adlarını belirtmediğimizden ilgili değerlere codes.Item1 ve codes.Item2 şeklinde erişebiliriz. cCodes'un tanımlanmasında ise xCode ve yCode isimli alanları içeren bir Tuple tipi söz konusudur. Dilersek ItemN adlarını eşitliğin sağ tarafında değişken değerleri verirken de belirtebiliriz. mCodes değişkeni için bu tip bir kullanım gerçekleştirilmektedir. Calculate metodu sum,dif,mul ve div isimli alan adlarını içeren bir Tuple döndürmektedir. Dikkat edilmesi gereken nokta Calculate metodunun Tuple örneğini nasıl döndürdüğüdür. result değişkenine atanan nesne örneği üzerinden sum,dif,mul ve div isimli öğelere de erişilmiştir.

C# 7.0 da Tuple ile ismi anılan bir diğer kavram da Deconstruct metodudur. Bu yetenek sayesinde bir nesne örneğinin doğrudan bir Tuple tipine atanıp kullanılabilmesi mümkündür. Aşağıdaki fotoğrafı inceleyelim.

![tfi155_2.gif](/assets/images/2017/tfi155_2.gif)

Product sınıfında Deconstruct isimli bir metod tanımlanmıştır. Metodda out ile belirtilen parametrelere sınıfın ProductId ve Title özelliklerinin değerleri atanmıştır. Main metodunda box isimli değişken de bu parametre yapısına uygun bir Tuple'a atanmıştır. Yani bir sınıf örneğinin Tuple türüne nasıl atanabileceğini belirtebiliriz.

Böylece geldik bir ipucumuzun daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.