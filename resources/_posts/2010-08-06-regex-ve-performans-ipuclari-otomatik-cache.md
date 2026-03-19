---
layout: post
title: "Regex ve Performans İpuçları – Otomatik Cache"
date: 2010-08-06 07:05:00 +0300
description: "Aslında Regular Expression terimini ağırlıklı olarak Asp.Net Web uygulamalarından tanımaktayız. Bu anlamda özellikle RegularExpressionValidator web kontrolünden yararlanarak, girilen verinin doğrulanması için bazı desenleri kullanabiliyoruz."
categories:
  - csharp
tags:
  - csharp
  - dotnet
  - aspnet
  - http
  - authentication
  - javascript
  - performance
  - caching
  - visual-studio
---
# Regex ve Performans İpuçları – Otomatik Cache
[![blg221_Giris](/assets/images/2010/blg221_Giris_thumb.jpg)](/assets/images/2010/blg221_Giris.jpg) Merhaba Arkadaşlar,

Şu an yazıyı hazırlamaya çalıştığım an İstanbul’ un tarihinde gördüğü en sıcak gecelerden birisine denk gelmekte sanırım. Gündüz yaklaşık olarak 53 derece olarak hissedilen sıcaklığı ofisteki kuvvetli klimalar sayesinde fazla hissetmedik belki ama eve dönüş yolunda, gerek otobüslerde gerekse minibüs veya diğer toplu taşıma araçlarında fazlasıyla hissettiğimize eminim

![Sealed](/assets/images/2010/smiley-sealed.gif)

Gece çökmesine ve balkonda oturmama rağmen ne yazık ki yapraklar bile sıcak dolayısıyla kendinden geçmiş durumda ve bu nedenle sallanmak dahi istemiyorlar. Hal böyle olunca serinletici esintilerinde tatile çıktıklarını ifade edebilirim.

Acaba tüm bu yaşadıklarımız, garip olan bu yaz mevsimi, yağmurlarla geçen günler ve aşırı sıcaklar gerçekten de Küresel Isınmanın sonuçların mı? Bu konuda dünyadaki 6 derecelik bir ısı değişiminin sonuçlarını anlatan bir kitap okumuştum aslında ([6 Derece](http://www.ntvyayinlari.com/tanim.asp?sid=SWBUBQIYKD1DPH7YQ3A8)) Merak edenlere tavsiye ederim.

Neyse. Dilerseniz biz konumuza geri dönelim. Bu yazımızda belki de tek satırlık bir kod parçasının önemine değiniyor olacağız. Ancak sonuçları irdelediğimizde bunun ne kadar önemli bir fark yarattığına da şahit olacağız. Konumuz Regex tipinin kullanımına dair ip uçlarından birisi olan otomatik ön bellekleme işlemini ele almakta.

Aslında Regular Expression terimini ağırlıklı olarak Asp.Net Web uygulamalarından tanımaktayız. Bu anlamda özellikle RegularExpressionValidator web kontrolünden yararlanarak, girilen verinin doğrulanması için bazı desenleri kullanabiliyoruz. Bilindiği üzere bu doğrulama işlemleri Javascript ile istemci tarafında ve her ihtimale karşın sunucu tarafında da uygulanmakta (İstemcinin javascript çalıştırmama olasılığına karşın). Tabi işin güzel yanı Regex ifadelerinin aslında dilden bağımsız olmaları. RegularExpressionValidator kontrolünün ValidationExpression özelliğinde yer alan desenlerden bazılarını aşağıda bulabilirsiniz.

- Internet Email Adres Deseni \w+([-+.']\w+) *@\w+([-.]\w+)* \.\w+([-.]\w+)*
- Internet URL Deseni http (s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?
- US Phone Number ((\(\d{3}\)?)|(\d{3}-))?\d{3}-\d{4}
- US Social Security Number \d{3}-\d{2}-\d{4}
- US Zip Code \d{5}(-\d{4})?
- German Phone Number ((\(0\d\d\) |(\(0\d{3}\))?\d)?\d\d \d\d \d\d|\(0\d{4}\) \d \d\d-\d\d?)

Bu desenler yardımıyla kullanıcıların girmiş olduğu verilerin geçerli bir elektronik posta/url adresi, telefon numarası, posta kodu, sosyal güvenlik bilgisi olup olmadığı kolaylıkla kontrol edilebilmekte. Tabi dilersek özel Regex ifadeleri de oluşturabiliriz.

Diğer taraftan bazı projelerde Doğrulama (Validation) operasyonlarını içeren katmanlarda, Regex tipinden yararlanarak verinin kontrol edilmesi işlemleri gerçekleştirilebilmektedir. Bu anlamda Regex tipi ve üyeleri bize önemli avantajlar sağlamakta. Ancak Regex tipinden yararlanırken zaman zaman performans sorunları ile karşılaşılabilir. Bu noktada ön bellekleme işlemlerinin hızlanma açısından bir avantaj sağladığı ortadadır.

[![Exclamation](/assets/images/2010/Exclamation_thumb_9.gif)](/assets/images/2010/Exclamation_9.gif) Aslında.Net Framework 1.1 versiyonunda yer alan Regex tipinin nesne örneklemelerinin, desen (Pattern) ile ilişkili bir ön bellekleme mekanizması zaten mevcuttur. Ancak.Net Framework 2.0 ve sonraki versiyonlarda söz konusu desen ön bellekleme işlemi static IsMatch metodu üzerine yıkılmıştır. Bir başka deyişle sadece IsMatch metodunun, parametre olarak gelen Regular Expression ifadesi için ön bellekleme yaptığını ifade edebiliriz. Normal şartlarda kaç desenin ön bellekleneceği bilgisi Regex tipinin static CacheSize özelliği (Property) ile belirlenebilir. Bu özelliğin varsayılan değeri ise 15 dir.

Şimdi basit bir test uygulaması geliştireceğiz ve aslında ön belleklemenin nasıl bir faydası olduğunu görmeye gayret edeceğiz. Bu amaçla 160bin satırdan oluşan ve aşağıdaki gibi bazı URL adres bilgilerini içeren (ki çoğu aynı değerlerin tekrarıdı ![Wink](/assets/images/2010/smiley-wink.gif)) Urls.txt isimli bir Text dosyasını göz önüne alıyor olacağız. Söz konusu dosya içeriğinin bir kısmını aşağıdaki şekilden görebilirsiniz.

[![blg221_TextFileContent](/assets/images/2010/blg221_TextFileContent_thumb.gif)](/assets/images/2010/blg221_TextFileContent.gif)

Uygulama içerisinde yer alan akışımız, söz konusu Text dosya içeriğindeki her bir satırı okuyacak ve bu adreslerden hangilerinin geçerli olduğunu kontrol edecek şekilde geliştirilecektir. İşte bu amaçla ele alacağımız örnek uygulama kodları;

```csharp
using System; 
using System.Diagnostics; 
using System.IO; 
using System.Text.RegularExpressions;

namespace RegExPerformanceTips 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string urlPattern=@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"; 
            string[] urls = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "Urls.txt"));            
            for (int i = 0; i < 10; i++) 
            { 
                Console.WriteLine("Test Number {0}",i); 
                Process(urlPattern, urls); 
            }            
        }

        private static void Process(string urlPattern, string[] urls) 
        { 
            Stopwatch watcher = new Stopwatch(); 
            watcher.Start(); 
            int validCount = 0;

            foreach (string url in urls) 
            { 
                if (IsValid(urlPattern, url)) 
                    validCount++; 
                //Console.WriteLine("{0} {1}",url,IsValid(urlPattern,url)?"is valid":"isn't valid"); 
            }

            watcher.Stop(); 
            Console.WriteLine("Valid Urls Count {0} , Total validation time : {1}", validCount, watcher.ElapsedMilliseconds); 
        }

        static bool IsValid(string pattern,string content) 
        { 
            Regex regex = new Regex(pattern); 
            bool result = regex.IsMatch(content); 
            //bool result = Regex.IsMatch(content, pattern);            
            return result; 
        } 
    } 
}
```

Kodun dikkat edilmesi gereken en önemli noktası IsValid metodunun içeriğidir. Söz konusu metod kontrol edilecek string bilgi ile ilgili Regular Expression desenini parametre olarak almaktadır. İlk test vakası için Regex tipinden bir nesne örneğinin oluşturulduğu görülmektedir. Regex nesnesi örneklenirken parametre olarak kontrol desenini almaktadır. Sonrasında ise nesne örneği üzerinden yapılan IsMatch metod çağrısı ile gerekli kontrol işlemi gerçekleştirilmektedir. Dosya içerisindeki bilgiler için gerekli doğrulama kontrolü süre farklılıklarını irdelemek amacıyla 10 defa üst üste yapılmaktadır. Buna göre ilk test vakamızın çalışma zamanı çıktısı uygulamanın yazıldığı sistemin özelliklerine göre aşağıdaki gibidir.

[![blg221_InstanceTestResult](/assets/images/2010/blg221_InstanceTestResult_thumb.gif)](/assets/images/2010/blg221_InstanceTestResult.gif)

Yaklaşık olarak 4 ile 9 saniye arasında değişen süreler söz konusudur. Sürelerin tutarsızlığı bir yana, kontrol işlemlerinin de oldukça da uzun sürdüğü görülmüştür. Peki uzun sürdükleri sonucuna nasıl vardık?

![Wink](/assets/images/2010/smiley-wink.gif)

Eğer IsValid metodu içerisinde Regex tipi üzerinden static IsMatch metodunu kullandığımız durumda ki çalışma zamanı çıktısına bakarsak, çok daha kısa sürelerde doğrulama işlemlerinin yapıldığını görebiliriz. İşte aynı makine konfigurasyonundaki yeni test vakasının çalışma zamanı sonuçları.

[![blg221_IsMatchTestResult](/assets/images/2010/blg221_IsMatchTestResult_thumb.gif)](/assets/images/2010/blg221_IsMatchTestResult.gif)

Buna göre test sonuçlarını aşağıdaki Excel grafiğinde görüldüğü gibi özetleyebiliriz.

[![blg221_FirstTestResults](/assets/images/2010/blg221_FirstTestResults_thumb.gif)](/assets/images/2010/blg221_FirstTestResults.gif)

Peki bu korkunç sayılabilecek süre farkı neden oluşmuştur. Yazımızın giriş kısmında da belirttiğimiz üzere Regex tipinden nesne örnekleri oluşturulduğunda her seferinde bir desen bildirimi yapılmaktadır ancak bu bildirim sürekli olarak tekrar edilmektedir. Oysaki Regex tipinin static IsMatch metodu ilk seferde kullanılan Regular Expression ifadesini ön belleklemekte ve sonraki çağrılarda da bu desen için gerekli iç hazırlıkları yapmadan sadece kontrol işlemine geçiş yapmaktadır. Tabi istenirse varsayılan olarak 15 farklı Regular Expression ifadesi için bu bellekleme işlemi kullanılabilir.

Regex tipinin performanslı kullanımına ilişkin farklı ip uçları da mevcuttur. Ancak sucuk gibi terldiğimden bu sıcakta ancak bu kadar yazabildim. Söz konusu ipuçlarını ilereyen yazılarımızda ele almaya çalışıyor olacağız. Umarım havalar buna müsade eder. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[RegExPerformanceTips.rar (33,72 kb)](/assets/files/2010/RegExPerformanceTips.rar) [Örnek Visual Studio 2010 Ultimate Ortamında Geliştirilmiş ve Test Edilmiştir]