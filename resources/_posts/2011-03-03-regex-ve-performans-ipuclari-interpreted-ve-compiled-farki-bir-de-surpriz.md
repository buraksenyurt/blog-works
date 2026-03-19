---
layout: post
title: "Regex ve Performans İpuçları – Interpreted ve Compiled Farkı, Bir de Sürpriz"
date: 2011-03-03 17:00:00 +0300
description: "İnce ayarlar çekilmiş bir yazılım, zaman zaman çok hızlı sonuçlar verebilir. Hatırlayacağınız üzere Regex ve Performans İpuçları – Otomatik Cache başlıklı bir önceki yazımızda son derece sıcak bir gecede, Regex tipinin performanslı kullanımına ilişkin ilk ip ucunu aktarmış ve sonuçlarını incelemeye çalışmıştık. Regex kullanımında dikkat çeken noktalardan bir diğeri de(bir başka deyişle yapılabilecek ince ayarlardan bir diğeri de) yorumlanarak(Interpret) veya önceden derlenerek(Compiled) çalıştırılabilen ifadeler ile ilişkilidir."
categories:
  - csharp
tags:
  - csharp
  - http
  - performance
  - caching
  - reflection
  - visual-studio
---
# Regex ve Performans İpuçları – Interpreted ve Compiled Farkı, Bir de Sürpriz
[![blg222_Giris](/assets/images/2011/blg222_Giris_thumb.jpg)](/assets/images/2011/blg222_Giris.jpg) Merhaba Arkadaşlar,

Formula 1 merakı olanlar, yarışan araçların mühendislik olarak birbirlerine çok yakın teknolojiler ile üretildikleri ve benzer olduklarını bilirler. Gerçi bazı zamanlarda ön plana çıkan araçlar da söz konusudur. Frenaj veya hızlanma sistemlerine getirilen iyileştirmeler sonucu, diğer yarış araçlarının pilotları kim olursa olsun belirgin bir şekilde öne fırlarlar.

Ancak bazen de araçlar bir birlerine o kadar denktir ki, yarışın kaderini ve sonuçlarını sürücüler ile Pit-Stop’ lar sırasında yapılan kritik değişiklikler belirler. Örneğin lastik seçimlerİ, ön veya arka kanatların açısal değerleri, rüzgarın hızına göre yapılan ayarlamalar, yakıt tankının ne kadar doldurulacağı vb…

E tabi en iyi sonuçları elde edebilmek için takımlar yıl boyu sayısız test sürüşü gerçekleştirir ve sürekli olarak istatistikler tutarak raporlamalarda bulunur ve stratejik kararları veren yöneticilerin önlerini daha iyi görmelerini sağlamaya çalışırlar. Çok doğal olarak yazılım süreçlerinde de benzer durumlar söz konusu değil midir?

![Wink](/assets/images/2011/smiley-wink.gif)

İnce ayarlar çekilmiş bir yazılım, zaman zaman çok hızlı sonuçlar verebilir. Hatırlayacağınız üzere [Regex ve Performans İpuçları – Otomatik Cache](../2010/Regex ve Performans İpuçları – Otomatik Cache.md) başlıklı bir önceki yazımızda son derece sıcak bir gecede, Regex tipinin performanslı kullanımına ilişkin ilk ip ucunu aktarmış ve sonuçlarını incelemeye çalışmıştık. Regex kullanımında dikkat çeken noktalardan bir diğeri de (bir başka deyişle yapılabilecek ince ayarlardan bir diğeri de) yorumlanarak (Interpret) veya önceden derlenerek (Compiled) çalıştırılabilen ifadeler ile ilişkilidir.

Bu notkada Regex tipine ait nesne örneklerinin oluşturulması sırasında devreye giren RegexOptions enum sabitinin Compiled değerinin kullanılması halinde ilgili regex deseninin derlenmiş halinin kullanılması söz konusudur. Normal şartlar altında Compiled değeri belirtilmediği takdirde, Interpret moda göre kontroller yapılmaktadır. Yani çalışma zamanı desen ile ilgili satıra geldiğinde bir takım işlemleri gerçekleştirir. Aslında konuyu teknik hatları ile irdelemek dışında örnek bir kod parçası üzerinde ilerleyerek çalışma zamanı sonuçlarına bakmamız da yarar vardır.

Örnek senaryomuzda bu kez 150 paragraflık [Lorem Ipsum](http://tr.lipsum.com/) içeriğinin defalarca arttırılmış ve aşağıdaki şekilden de görüleceği üzere aralara bir kaç URL adresi serpiştirilmiş bir versiyonu kullanılmaktadır. Söz konusu içerik LoremIpsum.txt isimli Text tabanlı bir dosyada toplanmış olup 9567 satırlık bir test içeriği üretilmiştir.

[![blg222_LoremIpsumText](/assets/images/2011/blg222_LoremIpsumText_thumb.gif)](/assets/images/2011/blg222_LoremIpsumText.gif)

Gelelim test için ele alacağımız örnek kodlarımıza.

```csharp
using System; 
using System.Diagnostics; 
using System.IO; 
using System.Text.RegularExpressions;

namespace RegexTips2 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string urlPattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"; 
            string fileContent=File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "LoremIpsum.txt"));

            #region Matches Metodu Kullanımı(Yorumlamalı)

            Console.WriteLine("***Matches (Interpreted)***"); 
            for (int i = 0; i < 10; i++) 
            { 
                MatchesTest(urlPattern, fileContent,false); 
            }

            #endregion

            #region Match ve NextMatch Kullanımı(Yorumlamalı)

            Console.WriteLine("***Match ve NextMatch (Interpreted)***"); 
            for (int i = 0; i < 10; i++) 
            { 
                NextMatchTest(urlPattern, fileContent,false); 
            }

            #endregion

            #region Matches Metodu Kullanımı(Derlemeli)

            Console.WriteLine("***Matches (Compiled)***"); 
            for (int i = 0; i < 10; i++) 
            { 
                MatchesTest(urlPattern, fileContent, true); 
            }

            #endregion

            #region Match ve NextMatch Kullanımı(Derlemeli)

            Console.WriteLine("***Match ve NextMatch (Compiled)***"); 
            for (int i = 0; i < 10; i++) 
            { 
                NextMatchTest(urlPattern, fileContent, true); 
            }

            #endregion 
        }

        private static void NextMatchTest(string urlPattern, string fileContent,bool isCompiled) 
        { 
            Stopwatch watcher = new Stopwatch(); 
            watcher.Start(); 
            Regex regex=null; 
            if (isCompiled) 
                regex = new Regex(urlPattern, RegexOptions.Multiline | RegexOptions.Compiled); 
            else 
                regex = new Regex(urlPattern, RegexOptions.Multiline); 
            Match match = regex.Match(fileContent); 
            int foundedMatch = 0; 
           if (match.Success) 
            { 
                do 
                { 
                    foundedMatch++; 
                    match = match.NextMatch(); 
                } while (match.Success); 
            } 
            watcher.Stop(); 
            Console.WriteLine("Bulunan eşleşme sayısı {0}.Toplam süre {1}", foundedMatch, watcher.ElapsedMilliseconds); 
        }

        private static void MatchesTest(string urlPattern, string fileContent,bool isCompiled) 
        { 
            Stopwatch watcher = new Stopwatch(); 
            watcher.Start(); 
            Regex regex = null; 
           if (isCompiled) 
                regex = new Regex(urlPattern, RegexOptions.Singleline | RegexOptions.Compiled); 
            else 
                regex = new Regex(urlPattern, RegexOptions.Singleline); 
            MatchCollection matches = regex.Matches(fileContent); 
            Console.Write("Bulunan eşleşme sayısı {0}.", matches.Count); 
            watcher.Stop(); 
            Console.WriteLine("Toplam süre {0}", watcher.ElapsedMilliseconds); 
        } 
    } 
}
```

Bu örnek Console uygulamasının Main metoduna ait kodlarda iki test metodu olduğu görülmektedir. MatchesTest isimli metod Regex nesne örneğinin Matches isimli fonksiyonunu değerlendirmektedir. Matches metodu ilk parametre olarak regex desenini almaktadır.

Örneğimizde bir önceki yazımızda olduğu gibi bir URL deseni ele alınmaktadır. Bir başka deyişle LoremIpsum.txt içerisinde URL formatına uygun olan cümlelerin bulunması hedeflenmektedir. Matches metodu, MatchCollection tipinden bir koleksiyon döndürmektedir ve bu koleksiyon içerisinde, URL desenine uygun olan cümleler Match tipinden nesne örnekleri halinde yer almaktadır. Aşağıdaki Debug zamanı resminde URL desenin uygun olan cümlelere ait bir görüntü yer almaktadır.

[![blg222_DebugTime](/assets/images/2011/blg222_DebugTime_thumb.gif)](/assets/images/2011/blg222_DebugTime.gif)

Tabi Debug görüntüsünden de anlaşılacağı üzere içeriğe ulaşmak için kod tarafında foreach gibi bir döngüden yararlanılması gerekmektedir.

NextMatchTest metodu ise daha farklı bir yaklaşım kullanmaktadır. Bu metodda öncelikli olarak Match metodu ile desene uygun bir cümle olup olmadığı kontrol edilmekte ve eğer varsa (ki bu durumda Match nesne örneği Success değerini verecektir) arkasından do…while döngüsü yardımıyla bir sonraki desen kontrolü operasyonu ile çalışmaya devam edilmektedir.

Her iki test metodu içerisinde Regex nesne örneği oluşturulurken RegexOptions enum sabitinin ilgili değerleri kullanılmakta ve metodların Compiled modda mı yoksa Interperted modda mı çalışacakları belirlenmektedir. Buna göre çalışma zamanı sonuçlarına baktığımızda aşağıdaki örnek çıktı ile karşılaştığımızı görürüz (Tabi ki bu sonuçlar uygulamanın çalıştırıldığı sistemin çevresel özelliklerine göre farklılık gösterecek ancak kimin daha hızlı olduğu konusu pek fazla değişmeyecektir ![Wink](/assets/images/2011/smiley-wink.gif))

[![blg222_LoremIpsumRuntime](/assets/images/2011/blg222_LoremIpsumRuntime_thumb.gif)](/assets/images/2011/blg222_LoremIpsumRuntime.gif)

Interpeted ve Compiled moda göre Matches ve NextMatch metodlarının kullanımının arka arkaya 10 kere tekrar ediliği bu testin sonuçları aşağıdaki Excel grafiğinden daha net bir şekilde anlaşılabilir.

[![blg222_LoremIpsumReport](/assets/images/2011/blg222_LoremIpsumReport_thumb.gif)](/assets/images/2011/blg222_LoremIpsumReport.gif)

Burada en çok dikkat çeken nokta Interpreted mod ile, Compiled moda göre çok daha hızlı sürelerde sonuç alınabilmesidir. Bunun en büyük nedenlerinden birisi, Compiled modda, desenin ilk kullanıldığı sırada oluşan başlatma işlemleri için yapılan zaman kaybıdır. Ancak durum her zaman bu şekilde de gelişmeyebilir

![Sealed](/assets/images/2011/smiley-sealed.gif)

Ne demek istiyorum acaba? Gelin daha önceki yazımızda ele aldığımız 160bin satırdan oluşan ve sadece doğru ve yanlış URL bilgileri içeren text dosyasını göz önüne alalım. Bu kez dosyanın satır sayısını 80bin olarak tutacağız. İşte yakaladığım çalışma zamanı sonuçlarından bir tanesi.

[![blg222_UrlsTextRuntime](/assets/images/2011/blg222_UrlsTextRuntime_thumb.gif)](/assets/images/2011/blg222_UrlsTextRuntime.gif)

Ve bu sonuçlara göre oluşan Excel grafiğinin yeni hali.

[![blg222_UrlsTextReport](/assets/images/2011/blg222_UrlsTextReport_thumb.gif)](/assets/images/2011/blg222_UrlsTextReport.gif)

Dikkat edileceği üzere Compiled çalışma zamanı sonuçlarında yer yer Interpreted moda göre daha hızlı süreler elde edilebildiği görülmektedir. Hatta NextMatch metodunun kullanıldığı senaryo ile en hızlı erişim süreleri elde edilmiştir (İlk denemedeki hariç

![Wink](/assets/images/2011/smiley-wink.gif)

)

[![Exclamation](/assets/images/2011/Exclamation_thumb_10.gif)](/assets/images/2011/Exclamation_10.gif) Şu da unutulmamalıdır ki burada Instance üzerinden çağırdığımız Matches veya Match gibi metodların, Regex tipi üzerinden çağrılabilen ve otomatik ön belleklemeyi kullanan Static versiyonları da mevcuttur. Bu versiyonların kullanımının daha hızlı olabileceğini düşünebiliriz. Ancak ben örneklerimdeki testler sırasında ve Base Class Library takımının konu ile ilgili araştırma yazılarında Instance üzerinden çağırılan Interpreted metodların, static olan versiyonlarına göre daha hızlı olabileceğini de gördüm. Bu konunun araştırılmasını da siz değerli okurlarıma bırakmak istiyorum

![Laughing](/assets/images/2011/smiley-laughing.gif)

## Yazının Ana Konusu Dışında Bir Mevzu

Hazır Compiled modda çalışabilen Regex metodlarına değinmişken bir de bunların ayrı bir Assembly içerisine nasıl derlenip kullanılabileceklerini incelemeye ne dersiniz?

![Wink](/assets/images/2011/smiley-wink.gif)

Burada amacımız derlenmiş Regex ifadelerini ayrı bir assembly içerisinde saklamak ve kullanmaktır. Bu amaçla örnek olarak aşağıdaki gibi bir kod parçasını göz önüne alabiliriz.

```csharp
using System.Reflection; 
using System.Text.RegularExpressions;

namespace CompiledRegexAssembly 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            // Derlenecek örnek Regex deseni 
            string urlPattern = @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"; 
            // Derleme ile ilişkili ön bilgiler. Desen, RegexOptions enum sabiti değeri vb... 
            RegexCompilationInfo compInfo =new RegexCompilationInfo(urlPattern, RegexOptions.Multiline, "UrlPattern", "Azon.Common.Regex", true); 
            RegexCompilationInfo[] regexes = { compInfo }; 
            // Kaydedilecek Assembly için gerekli bilgileri içeren AssemblyName nesne örneği oluşturulur 
            AssemblyName assemName = new AssemblyName("AzonRegexLib, Version=1.0.0.1001, Culture=neutral, PublicKeyToken=null"); 
            // Regex bilgileri ilgili Assembly içerisine CompileToAssembly metodu ile kaydedilir. 
            Regex.CompileToAssembly(regexes, assemName); 
        } 
    } 
}
```

Kodun çalıştırılması sonucu AzonRegexLib.dll isimli bir Assembly’ ın uygulamaya ait Exe ile aynı yere çıkartıldığı görülecektir.

[![blg222_CompiledDll](/assets/images/2011/blg222_CompiledDll_thumb.gif)](/assets/images/2011/blg222_CompiledDll.gif)

Eğer söz konusu Assembly içeriğine kendimize işkence yaparak ILDASM (Intermediate Language Disassembler Tool) aracı yardımıyla bakarsak, aşağıdaki ekran görüntüsünde yer alan içeriğe ulaşabiliriz.

[![blg222_Il1](/assets/images/2011/blg222_Il1_thumb.gif)](/assets/images/2011/blg222_Il1.gif)

UrlPattern isimli sınıf Regex tipinden türetilmiştir. Yapıcı metoda (Constructor) baktığımızda ise aşağıdaki IL içeriğinin üretildiğini görebiliriz.

```text
.method public specialname rtspecialname 
        instance void  .ctor() cil managed 
{ 
  // Code size       51 (0x33) 
  .maxstack  4 
  IL_0000:  ldarg.0 
  IL_0001:  call       instance void [System]System.Text.RegularExpressions.Regex::.ctor() 
  IL_0006:  ldarg.0 
  IL_0007:  ldstr      "http(s)\?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./\?%&=]*)\?" 
  IL_000c:  stfld      string [System]System.Text.RegularExpressions.Regex::pattern 
  IL_0011:  ldarg.0 
  IL_0012:  ldc.i4.s   2 
  IL_0014:  stfld      valuetype [System]System.Text.RegularExpressions.RegexOptions [System]System.Text.RegularExpressions.Regex::roptions 
  IL_0019:  ldarg.0 
  IL_001a:  newobj     instance void Azon.Common.Regex.UrlPatternFactory1::.ctor() 
  IL_001f:  stfld      class [System]System.Text.RegularExpressions.RegexRunnerFactory [System]System.Text.RegularExpressions.Regex::factory 
  IL_0024:  ldarg.0 
  IL_0025:  ldc.i4.s   4 
  IL_0027:  stfld      int32 [System]System.Text.RegularExpressions.Regex::capsize 
  IL_002c:  ldarg.0 
  IL_002d:  call       instance void [System]System.Text.RegularExpressions.Regex::InitializeReferences() 
  IL_0032:  ret 
} // end of method UrlPattern::.ctor
```

0001 numaralı IL koduna baktığımızda Regex tipinden bir nesne örneğinin oluşturulması için gerekli yapıcı metodun çağırıldığını görebiliriz. Sonrasında 0007 numaralı satırda URL deseni için string tipinden yerel bir değişkenin tanımlandığı gözlemlenir. İlerleyen kısımlarda dikkat çeken noktalardan birisi de UlrPatternFactory1 isimli bir tip için nesne örneklenmesidir. UrlPatternFactory1 sınıfı System.Text.RegularExpressions isim alanında (Namespace) yer alan RegexRunnerFactory tipinden türeyen bir fabrikadır. Özellikle CreateInstance metodunun içeriği dikkate değerdir.

```text
.method public virtual instance class [System]System.Text.RegularExpressions.RegexRunner 
        CreateInstance() cil managed 
{ 
  // Code size       6 (0x6) 
  .maxstack  1 
  IL_0000:  newobj     instance void Azon.Common.Regex.UrlPatternRunner1::.ctor() 
  IL_0005:  ret 
} // end of method UrlPatternFactory1::CreateInstance
```

Görüldüğü üzere UrlPatternRunner1 tipine ait bir nesne örneğinin üretildiği görülmektedir. UrlPatternRunner1 tipi ise System.Text.RegularExpressions isim alanında yer alan RegexRunner sınıfından türetilmiştir. UrlPatternRunner1 tipinin içeriğinde yer alan operasyonlar tahmin edileceği üzere desen araştırma işlemlerinin yapılması için gerekli fonksiyonellikleri de içermektedir. Açıkçası bu tipin operasyonlarının içeriklerini tam olarak incelemeye çalıştığımızda çok fazla yere gittiğimiz için takibin zorlaştığını itiraf edebilirim

![Undecided](/assets/images/2011/smiley-undecided.gif)

Aslında bu noktada söz konusu assembly’ ı örnek bir projede kullanmamızda yarar olacağı kanısındayım. Gelin örnek bir Console uygulamasından söz konusu Assembly’ ı referans ederek LoremIpsum.txt dosyası içeriği üzerinden test edelim. İşte örnek uygulama kodlarımız.

```csharp
using System; 
using System.IO; 
using System.Text.RegularExpressions; 
using Azon.Common.Regex;

namespace UsingCompiledRegexAssembly 
{ 
    class Program 
    { 
        static void Main(string[] args) 
        { 
            string fileContent=File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "LoremIpsum.txt")); 
            UrlPattern pattern = new UrlPattern(); 
            MatchCollection matches=pattern.Matches(fileContent); 
            foreach (Match mtch in matches) 
            { 
                Console.WriteLine(mtch.Value); 
            } 
        } 
    } 
}
```

Diğer örnekleri düşündüğümüzde herhangibir yerde kontrol edilecek bilgiye ait desen (Regex Pattern) tanımının yapılmadığını görebiliriz. Sadece desene uygun olan verilerin yer aldığı dosya içeriği parametre olarak verilmiştir. Nitekim söz konusu desen tanımı ve bununla ilişkili başlangıç işlemleri zaten derlenmiş olan assembly içerisinde yer almaktadır. Çok doğal olarak derlenmiş bir assembly üzerinden yapılan çağrılar, instance ile yapılan çağrılara nazaran daha performanslı olacaktır.

Ama esneklik yönünden de derlenmiş assembly kullanımının bazı dez avantajları vardır. Örneğin dinamik olarak bir desen bildirilemez. Desen zaten önceden belirlenmiş ve assembly içerisine gömülmüştür. Hatta kodun ilerleyen kısımlarında Case Sensitive'liğin dikkate alınmaması veya alınması gereken bir durumda RegexCompilationInfo tipi ile ilgili seçeneklerinin yeniden düzenlenmesi gerekmektedir ki bu mümkün değildir.

![Exclamation](/assets/images/2011/Exclamation_thumb_10.gif)

Aslında mümkün değildir derken biraz kolay kaçtığımızı ifade edebiliriz. Nitekim Reflection yardımıyla başlangıç ayarlarının parametrik olarak verilmesi, yeniden derlenmesi ve dinamik olarak yüklenerek kullanılması mümkündür. Hatta bu noktada dynamic keyword'ünün de bir çok noktada işi kolaylaştıracağını ifade edebiliriz. Yine de işin içerisinde dinamik olarak çalışma zamanına yük getirecek ve performansı olumsuz yönde etkileyecek bir çalışma mekanizması söz konusudur.

Yani Assembly'ın yeniden üretilmesi ve referans edilmesi gerekmektedir. Açıkçası tek bir desen değil ama n sayıda desenin kullanıldığı ve başlangıçtaki konfigurasyon seçeneklerinin belli olduğu senaryolarda kullanılması daha doğru olabilir. foreach döngüsünün çalışmasına göre LoremIpsum dosyasında geçerli URL formatında olan tüm cümleler elde edilebilecektir.

[![blg222_RuntimeLast](/assets/images/2011/blg222_RuntimeLast_thumb.gif)](/assets/images/2011/blg222_RuntimeLast.gif)

Tabi buraya kadar bahsettiklerimizi göz önüne aldığımızda Regular Expression kontrollerinde hangi tekniği kullanacağımız yönünde kafamızda bir sürü soru oluşmuş olabilir. Aslında aynı Regular Expression nesnesinin defalarca kullanıldığı senaryolarda static üyelerin kullanılması daha cazip görünmektedir. Diğer yandan desenlerin başlangıçtaki opsiyonel seçeneklerinin belli olduğu durumlarda ise, derlenmiş versiyonlarını kullanmak daha mantıklı olabilir. İşin gerçeği son sözü söylemek için test sonuçlarına bakmak bence en doğrusudur

![Wink](/assets/images/2011/smiley-wink.gif)

Böylece geldik bir yazımızın daha sonuna. Tekrardan görüşünceye dek hepinize mutlu günler dilerim.

[RegExPerformanceTipsV2.rar (246,51 kb)](/assets/files/2011/RegExPerformanceTipsV2.rar) [Örnek Visual Studio 2010 Ultimate Sürümü Üzerinde Geliştirilmiş ve Test Edilmiştir]