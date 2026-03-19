---
layout: post
title: "LINQ Maceralarım"
date: 2008-04-10 06:00:00 +0300
description: "Language INtegrated Query(LINQ) mimarisi sayesinde CLR nesneleri(Common Language Runtime Objects) üzerinden SQL tarzı sorgu ifadeleri yazılabilmektedir. Hatta LINQ mimarisi, SQL veritabanı(LINQ to SQL) ve XML (LINQ to XML) kaynakları üzerindede kullanılabilmektedir. Özellikle IEnumerable<T> arayüzünü uyarlayan tiplere ait nesne örnekleri için, Select, Where, GroupBy, Sum, Avg, Distinct ve daha pek çok bilinen sorgulama metodu uygulanabilmektedir."
categories:
  - csharp-3-0
  - linq
tags:
  - csharp-3-0
  - linq
  - csharp
  - dotnet
  - ado-net
  - sql-server
  - windows-forms
  - xml
  - threading
  - reflection
  - generics
  - visual-studio
  - datatable
---
# LINQ Maceralarım
Değerli Okurlarım Merhabalar,

Language INtegrated Query (LINQ) mimarisi sayesinde CLR nesneleri (Common Language Runtime Objects) üzerinden SQL tarzı sorgu ifadeleri yazılabilmektedir. Hatta LINQ mimarisi, SQL veritabanı (LINQ to SQL) ve XML (LINQ to XML) kaynakları üzerindede kullanılabilmektedir. Özellikle IEnumerable arayüzünü uyarlayan tiplere ait nesne örnekleri için, Select, Where, GroupBy, Sum, Avg, Distinct ve daha pek çok bilinen sorgulama metodu uygulanabilmektedir.

LINQ içerisinde yer alan imkanlar göz önüne alındığında,.Net Framework 1.1, 2.0 ve 3.0 ile geliştirilmiş pek çok projenin.Net 3.5' e aktarılarak bu olanaklardan yararlanabilmeleri için gerekli geçiş hazırlıklarının ciddi anlamda düşünüldüğüde ortadadır. Üstelik Visual Studio 2008, getirdiği çoklu framework desteği sayesinde.Net Framework 2.0, 3.0 ve 3.5 arasındaki geçişlerin kolayca yapılabilmesini sağlamaktadır. Bu gibi konular göz önüne alındığında bir geliştirici olarak LINQ'in daha önceki kod parçalarında kullanılabileceği yeni yerlerde merak konusu haline gelmektedir. İşte bu makalemizde, LINQ sorgularını farklı kod parçalarında kullanmaya çalışıyor olacağız.

LINQ mimarisinin kullanılabileceği alanlar göz önüne alındığında, Reflection (Yansıma), IO (Dosya giriş/çıkış), Bağlantısız Katman (Disconnected Layer) sadece bir kaç basit alan olarak ön plana çıkmaktadır. Ancak bu alanlar pek çok uygulamada önemli görevler üstlenmektedir. Söz gelimi yansıma teknikleri ile IDE geliştirilmesi (Visual Studio benzeri), Plug-In tabanlı uygulamalar yazılması, nitelik (Attribute) bazlı olacak şekilde çalışma ortamının organize edilmesi (özellikle deklerafit programlama tekniklerinde) gibi işlevsellikler ön plana çıkmaktadır. Dosyalama işlemleri en basit anlamda resim işleme programlarından, XML ayrıştırma uygulamalarına kadar pek çok alanda kullanılmaktadır. Çok doğal olarak Ado.Net mimarisine göre geliştirilen pek çok uygulamada bağlantısız katman nesneleri görülebilmektedir. Sadece bu konular bile göz önüne alındığında bazı kod ihtiyaçları için diziler, döngüler ve koşullu ifadelerin çok sık kullanıldığıda göze çarpmaktadır. Ancak LINQ sorguları sayesinde bu işlemler çok daha basit bir şekilde gerçekleştirilebilir. Elbetteki genişletme metodlarının üstlendiği yük çerçevesinde söz konusu döngülerin, koşullu ifadelerin ortadan kalkması gibi bir durum mümkün değildir. Ancak kodun çok daha etkin bir şekilde ve bir sorgulama diline yatkın olaraktan geliştirilmesi önemli bir avantajdır.

> Bilindiği gibi LINQ sorgularında yer alan anahtar kelimeler (keywords) aslında arka planda birer genişletme metoduna (Extension Methods) karşılık gelmektedir. Bu metodlar söz gelimi basit bir arama işlemi için gereken döngüsel veya koşullu ifadeleri kapsülleyerek geliştiricinin üzerinden almaktadır. Bu sayede geliştirici SQL diline yatkın bir şekilde sorgular yazabilmekte ve kodun daha etkili, ölçeklenebilir, anlaşılır bir şekilde geliştirilmesine odaklanabilmektedir.

Dilerseniz hiç vakit kaybetmeden örneklerimize başlayalım.

İlk olarak dosyalama işlemlerini göz önüne alarak ilerleyebiliriz. Söz gelimi, herhangibir klasör içerisinde yer alan Jpg uzantılı dosyalardan boyutu 1000 kb üzerinde olanların tespit edilmesini istediğimizi düşünelim. Bu işlemi VS 2008 tabanlı bir Console Uygulamasında aşağıdaki kod parçası ile gerçekleştirebiliriz.

```csharp
string klasorAdresi= @"C:\Documents and Settings\BurakSenyurt\My Documents\My Pictures\Google Pictures\";
DirectoryInfo dInfo = new DirectoryInfo(klasorAdresi);
var resimDosyalari = from fInfo in dInfo.GetFiles()
                                    where fInfo.Extension == ".jpg" && fInfo.Length >= 1000 * 1024
                                        select new
                                                    {
                                                        fInfo.Name
                                                        ,fInfo.Length
                                                        ,fInfo.CreationTime
                                                    };
foreach (var dosya in resimDosyalari)
    Console.WriteLine(dosya);
```

DirectoryInfo sınıfının GetFiles metodu FileInfo tipinden bir dizi döndürmektedir. Bu dizi bir Array tipi olduğu için LINQ ile birlikte gelen genişletme metodlarını (Extension Methods) kullanabilmektedir. Dolayısıyla LINQ ifadesi içerisinde from, select, where gibi anahtar kelimeler kolay bir şekilde ele alınabilmektedir. FileInfo dizisi üzerinden dosya uzantısı (Extension).jpg ve uzunluğu (Length) 1000 Kb üzerinde olanlar tespit edilirken aynı zamanda isimsiz bir tip (Anonymous Type) üretimide gerçekleştirilmekte ve ilgili dosya için ad (Name), uzunluk (Length) ve oluşturulma zamanı (CreationTime) bilgilerinin yer aldığı yeni bir nesne örneği oluşturulmaktadır. Program kodunun çıktısı örnek klasör için aşağıdaki gibidir.

![mk248_1.gif](/assets/images/2008/mk248_1.gif)

Eğlenceli değil mi? Öyleyse devam edelim. Diyelimki dosyalama işlemleri ile ilgili olaraktan şöyle bir ihtiyacımız oldu;Bir klasör içerisindeki dosyaları tiplerine göre gruplayıp, her grup içerisinde kaçar adet dosya bulunduğunu öğrenmek istiyoruz. Bu kodun LINQ ifadesini yazmadan önce, LINQ olmadan nasıl geliştirilebileceğini düşünmenizi öneririm. LINQ ile bu sorgu aşağıdaki kod parçasında olduğu gibi gerçekleştirilebilir.

```csharp
string adres = @"C:\Windows\";
DirectoryInfo dInfo = new DirectoryInfo(adres);
var dosyaGruplari = from fInfo in dInfo.GetFiles()
                                    group fInfo by fInfo.Extension into grp
                                        select new
                                                    {
                                                        Uzanti = grp.Key,
                                                        Toplam = grp.Count()
                                                    };

foreach (var dosyaGrubu in dosyaGruplari)
    Console.WriteLine(dosyaGrubu.ToString());
```

Bu seferki LINQ ifadesinde group by kullanımı söz konusudur. Group By sayesinde aynen SQL'de olduğu gibi gruplama işlemi nesneler üzerinde yapılabilmektedir. Örnekte Windows klasörü altındaki dosyalar FileInfo tipinin Extension özelliğine göre gruplanmaktadır. Sonrasında ise gruplanan koleksiyon üzerinden Count genişletme metodu kullanılmakta ve her bir tip grubu için kaçar dosya olduğu hesaplanmaktadır. İlk örnekte olduğu gibi yine isimsiz tip (Anonymous Type) kullanılarak dosya grubuna ait uzantı ve toplam dosya sayısı bilgileri elde edilmektedir. Sonuç olarak uygulamanın ekran çıktısı aşağıdaki gibi olacaktır.

![mk248_2.gif](/assets/images/2008/mk248_2.gif)

Şimdide herhangibir klasördeki jpg uzantılı dosyalardan L harfi ile başlayanları boyutlarına göre tersten sıralayarak elde etmek istediğimizi düşünelim. LINQ kullanmadığımız takdirde bize en çok sorun çıkartacak noktalardan biriside tersten sıralama işlemi olacaktır. Bunu sağlamak için doğal olarak FileInfo dizisi üzerinden ters sıralama algoritması uygulanması gerekir. Oysaki LINQ ifadeleri ile bu işlem için gerekli kod parçası aşağıdaki gibi kolayca geliştirilebilir.

```csharp
string klasorAdresi = @"C:\Documents and Settings\BurakSenyurt\My Documents\My Pictures\Google Pictures\";
DirectoryInfo dInfo = new DirectoryInfo(klasorAdresi);

var dosyalar=from fInfo in dInfo.GetFiles()
                        where fInfo.Extension==".jpg" && fInfo.Name[0]=='L'
                            orderby fInfo.Length descending
                                select new 
                                                {
                                                    fInfo.Name,
                                                    fInfo.Length
                                                };

foreach (var dosya in dosyalar)
    Console.WriteLine("{0} \t{1}",dosya.Length,dosya.Name);
```

Bu kez orderby anahtar kelimesi (ki bu arka planda OrderBy genişletme metoduna dönüştürülmektedir) kullanılarak dosyaların boyutlarına göre tersten sıralanması sağlanmıştır. Sonuç olarak kodun ekran çıktısı aşağıdakine benzer olacaktır.

![mk248_3.gif](/assets/images/2008/mk248_3.gif)

LINQ sorguları dosyalama işlemleri dışında özellikle reflection (yansıma) tarafındada etkili bir şekilde kullanılabilir. Yazımızın bundan sonraki kısmındada yansıma teknikleri içerisinde LINQ ifadelerini örnekler üzerinde ele almaya çalışacağız. Öncelikli olarak Process'lerden başlamak taraftarıyım. Bilindiği üzere.Net uygulamaları sistem üzerinde açılan Process'ler içerisinde ayrı uygulama alanları (Application Domains) altına dahil edilirler. Hatta bu uygulama alanları kendi içlerinde, birden fazla (en az bir tane olmak üzere) Thread'ede sahip olabilirler. Sistem üzerinde çalışan Process'lerin yada o anda çalışmakta olan güncel Process'in bilgilerini almak için Process sınıfının farklı metodları bulunmaktadır. Bizimde aklımıza gelen soru şudur; acaba sistem üzerinde çalışmakta olan Process'ler içerisinde sadece tek bir Thread'e sahip olanlar hangileridir. Nitekim bilindiği üzere bazı Process'ler kendi içlerinde birden fazla Thread içermektedir. Bu amaçla aşağıdaki gibi bir kod parçası geliştirilebilir.

```csharp
var processes = from prc in Process.GetProcesses()
                            where prc.Threads.Count == 1
                                orderby prc.ProcessName descending
                                    select new
                                                {
                                                    prc.ProcessName
                                                    , prc.PagedMemorySize64
                                                };
foreach (var process in processes)
    Console.WriteLine(process.ToString());
```

Process sınıfının static GetProcesses metodu ile o anda sistemde çalışmakta olan Process'ler elde edilmektedir. Sonrasında where anahtar kelimesi ile Threads özelliği üzerinden Count değeri kontrol edilir. 1 olanlar adlarına (ProcessName) göre orderby anahtar kelimesinden yararlanılarak tersten sıralanacak şekilde yeni bir isimsiz tip içerisinde toplanırlar. Bu isimsiz tip (Anonymous Type) örnek olarak Process'in adı (ProcessName) ve sayfalanmış bellek boyutu (PagedMemorySize64) değerlerini içermektedir. Sonuç olarak kodun çıktısı, çalışılan sistem üzerinde aşağıdaki gibi olmuştur.

![mk248_4.gif](/assets/images/2008/mk248_4.gif)

Reflection ile başlamışken hızımızı kesmeyelim ve yeni bir sorgu ile devam edelim. Bu kez şöyle bir ihtiyacımız var; bir assembly'ın referans ettiği assmebly'lar içerisinden versiyonu.Net Framework 2.0 olmayanları bulmak istiyoruz. Bu tip bir durumda var olan Assembly'ın yüklenmesi ve referans ettiği Assembly'ların GetReferencedAssemblies metodu ile çekilmesi gerekir. Ne tesadüftürki GetReferencedAssemblies metodu AssemblyName tipinden bir dizi döndürmektedir. Dolayısıyla bu dizi üzerinden LINQ ifadeleri kullanılabilmesi olasıdır. Söz konusu ihtiyaç için aşağıdaki gibi bir kod parçası düşünülebilir.

```csharp
AssemblyName[] result1 = Assembly.LoadFrom(@"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.EnterpriseServices.dll")
.GetReferencedAssemblies();
var framework2Olmayanlar = from asmb in result1
                                                where asmb.Version != new Version(2, 0, 0, 0)
                                                    select new
                                                                {
                                                                    AssemblyAdi = asmb.FullName
                                                                    ,IslemciMimarisi = asmb.ProcessorArchitecture
                                                                    ,HashAlgoritması = asmb.HashAlgorithm
                                                                };

foreach (var a in framework2Olmayanlar)
    Console.WriteLine(a.ToString());
```

Örnek olarak System.EnterpriseServices.dll assembly'ı kullanılmaktadır. Sorgu içerisinde dikkat edilecek olursa GetReferencedAssemblies metodu ile elde edilen sonuç kümesi üzerinden çekilen her bir AssemblyName nesnesinin Version özelliğine bakılmaktadır. Sonrasında ise yine bir isimsiz tip kullanılarak sadece Assembly'ın adı (FullName), işlemci mimarisi (ProcessorArchitecture) ve hash algoritması (HashAlgorithm) değerleri toplanmaktadır. Örneğin ekran çıktısı aşağıdaki gibi olacaktır.

![mk248_5.gif](/assets/images/2008/mk248_5.gif)

LINQ sorguları içerisinde bazı yerlerde harici metodlarında çağırılması mümkündür. Söz gelimi bir koşul kontrolü için iterasyonun o andaki nesnesinin denetlenmesi gerektiği durumlarda harici metod çağrıları gerekebilir. Örneğin dll uzantılı dosyalar ile dolu bir klasör içerisinde.Net Assembly'ı olarak yüklenebilenlerin tespit edilmesini istediğimiz düşünelim. Böyle bir senaryoda Assembly sınıfının static LoadFrom metodu oldukça işe yarayacaktır. Nitekim söz konusu dll herhangibir nedenle yüklenebilen bir Assembly değilse çalışma zamanı istisnası (Runtime Exception) oluşacaktır. Aşağıdaki kod parçası bu durumu analiz etmek için geliştirilmiştir.

```csharp
class Program
{
    static void Main(string[] args)
    { 
        string path = @"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727";
        DirectoryInfo klasor = new DirectoryInfo(path);
        var assemblyOlanlar = from dosya in klasor.GetFiles("*.dll")
                                            where Yuklenebildinmi(dosya.FullName)
                                                select dosya;

        foreach (var asmb in assemblyOlanlar)
            Console.WriteLine(asmb.FullName);
    }

    static bool Yuklenebildinmi(string assemblyAdresi)
    { 
        try
        {
            Assembly asmbly = Assembly.LoadFrom(assemblyAdresi);
            return true;
        }
        catch
        {
            return false;
        }
    } 
}
```

GetFiles metodu ile dll uzantılı FileInfo dizisi elde edildikten sonra her bir eleman için Yuklenebildimi isimli bir metod ile denetleme işlemi gerçekleştirilmektedir. Yuklenebildimi isimli fonksiyon, Assembly.LoadFrom metodu işe yarıyorsa true değerini, yaramıyorsa false değerini döndürmektedir. Buna göre true değeri dönen dosyaların yüklenebilen assembly'lar olduğu sonucuna varılmaktadır. Uygulamanın çalışma zamanındaki görüntüsü aşağıdakine benzer olacaktır.

![mk248_6.gif](/assets/images/2008/mk248_6.gif)

Yine assembly'lar üzerinden LINQ sorguları yazmaya devam edelim. Örneğin bir assembly içerisinden dışarıya sunulan harici tipler göz önüne alınsın. Burada işin içerisine gruplama fonksiyonelliğinide katarak, hangi isim alanı (namespace) içerisinden kaç adet tipin dışarıya sunulduğu bilgiside elde edilebilir. Bu işi gerçekleştirmek için örnek olarak aşağıdaki gibi bir kod parçası göz önüne alınabilir.

```csharp
Assembly systemAsmb = Assembly.LoadFrom(@"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Web.dll"); 
var hariciTipler = from t in systemAsmb.GetExportedTypes()
                            group t by t.Namespace into ng
                                orderby ng.Key descending
                                    select new
                                                {
                                                    IsimAlaniAdi = ng.Key,
                                                    TipSayisi = ng.Count()
                                                };
foreach (var hariciTip in hariciTipler)
    Console.WriteLine("{0} isim alanından {1} tip vardır", hariciTip.IsimAlaniAdi, hariciTip.TipSayisi.ToString());
```

Bu LINQ sorgusunda GetExportedTypes metodu yardımıyla örnek olarak System.Web.dll assembly'ı içerisinden dışarıya sunulmakta olan harici tiplerin listesi Type türünden bir dizi olarak elde edilmektedir. Sonrasında ise her tip, Namespace özelliğinin değerine göre group anahtar kelimesi yardımıyla ng isimli değişken altında gruplanmaktadır. Gruplanan veriler sonucu elde edilen liste Namespace adlarına göre tersten (descending) sıralanmaktadır. Bu noktada devreye orderby anahtar kelimesi girmektedir. Elde edilen listeden isim alanı adları Key özelliği ile ve tip sayılarıda Count genişletme metodu ile çekilerek yeni bir isimsiz tip altında toplanmaktadır. Kod parçasının çalışmasının sonucu oluşan örnek ekran çıktısı ise aşağıdaki gibidir.

![mk248_7.gif](/assets/images/2008/mk248_7.gif)

Peki herhangibir assembly içerisinde kaç farklı isim alanı olduğunu bulmak istersek. Normal şartlarda bu işlem için isim alanı adlarını çektikten sonra bir fonksiyonellik geliştirilmesi gerekmektedir. Oysaki LINQ ile birlikte genen Distinct genişletme metodu sayesinde söz konusu işlem aşağıdaki kod parçasında olduğu gibi kolayca gerçekleştirilebilir.

```csharp
Assembly systemAsmb = Assembly.LoadFrom(@"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.Xml.dll"); 
var isimAlanlari = (from t in systemAsmb.GetTypes() 
                                select t.Namespace).Distinct();
Console.WriteLine("\n{0} assembly' ı içerisinde {1} farklı isim alanı adı vardır", systemAsmb.FullName,isimAlanlari.Count()-1);
foreach (var isimAlani in isimAlanlari)
    Console.WriteLine(isimAlani);
```

Burada dikkat edilmesi gereken noktalardan biriside Distinct işlevselliğinin bir metod olarak select sorgusunun arkasından kullanılmasıdır. Buna ek olarak Count genişletme metodu ilede farklı isim alanlarının sayısı çekilmektedir. Örnekte yer alan System.Xml.dll assembly'ı için ilgili sonuçlar aşağıdaki gibi olacaktır.

![mk248_8.gif](/assets/images/2008/mk248_8.gif)

Reflection ile ilişkili olarak LINQ sorgularını kullanacağımız son bir örnek ile devam edelim. Bu sefer bir assembly içerisinde yer alan tiplerin toplam sayılarını türedikleri base type'lara göre gruplayarak elde etmeye çalışıyor olacağız. Bu amaçla, Type sınıfının BaseType özelliği gruplama işleminde kullanılabilir. Söz gelimi System.dll assembly'ı içerisindeki tipleri BaseType özelliklerinin değerlerine göre gruplamak istersek aşağıdaki kod parçası yeterli olacaktır.

```csharp
Assembly systemAsmb = Assembly.LoadFrom(@"C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727\System.dll");
var tipler = from m in systemAsmb.GetTypes()
                    group m by m.BaseType into grp
                        select new
                                    {
                                        grp.Key
                                        ,Toplam = grp.Count()
                                    };
foreach (var tip in tipler)
    Console.WriteLine("{0} \t{1}", tip.Key, tip.Toplam.ToString());
```

Bir önceki örnektekine benzer olacak şekilde yine group by fonksiyonu kullanılmaktadır. Sonuç olarak üretilen isimsiz tip içerisinde BaseType adı ve toplam tip sayısı değerleri yer almaktadır. Örnek kodun çalışma zamanındaki ekran çıktısı aşağıdakine benzer olacaktır.

![mk248_9.gif](/assets/images/2008/mk248_9.gif)

LINQ (Language INtegrated Query) ifadeleri pek çok dizi tipi ve koleksiyon üzerinde etkin bir şekilde kullanılabildiğinden, akla gelen konulardan biriside görsel uygulamalarda yer alan Controls koleksiyonlarıdır. Bir windows uygulamasında yada web uygulamasında Container görevi üstlenen ve bu sebepten Controls koleksiyonuna sahip olan nesnel topluluklar üzerinde de LINQ sorguları çalıştırılabilir. Bunu basit bir örnek üzerinden inceleyebiliriz. Söz gelimi aşağıdaki ekran görüntüsüde yer alan bir Windows Formumuz olduğunu düşünelim.

![mk248_10.gif](/assets/images/2008/mk248_10.gif)

Amacımız şimdilik bu form üzerinde hangi tipte kontroller bulunduğunu göstermek. Bu amaçla basit olarak aşağıdaki gibi bir kod parçası yeterli olacaktır.

```csharp
private void button2_Click(object sender, EventArgs e)
{
    lstSonuclar.Items.Clear();

    IEnumerable<Control> kontroller=Controls.Cast<Control>();

    var farkliTipler = (from kontrol in kontroller
                                select kontrol.GetType()).Distinct().OrderBy(k => k.Name);

    foreach (Type farkliTip in farkliTipler)
        lstSonuclar.Items.Add(farkliTip.Name);
}
```

Bu kod parçasıda belkide en önemli noktalardan biriside Controls özelliği üzerinden kullanılan Cast genişletme metodudur. Cast metodu kullanılmadığı takdirde Controls özelliği üzerinden Select, Where, GroupBy gibi LINQ sorgularında önem arz eden fonksiyonelliklere erişilemediği görülür. Cast metodunun buradaki görevi Controls koleksiyonu içerisindeki bileşenleri, parametre olarak verilen generic tipe dönüştürerek IEnumerable arayüzünün taşıyabileceği bir nesne topluluğu referansı halinde üretmektir. Böylece LINQ sorguları için gerekli fonksiyonellikler elde edilebilmektedir.

Windows formu üzerindeki görsel bileşenler Control sınıfından türemektedir. Bu sebepten Cast metodunun generic parametresi Control tipindendir. Bu dönüştürme işleminin ardından Distinct ve OrderBy genişletme metodlarınında yer aldığı bir LINQ sorgusu çalıştırılması mümkün olmaktadır. Select sorgusunda, GetType metodunun kullanılmasının sebebi tiplerin benzersiz şekilde ele alınmak istemesidir. Sonuç olarak uygulamanın çalışma zamanındaki ekran çıktısı aşağıdakine benzer olacaktır.

![mk248_11.gif](/assets/images/2008/mk248_11.gif)

Görüldüğü gibi form üzerinde hangi tipten kontrollerin var olduğu listelenmektedir. Yeni bir sorgu ile devam edelim. Bu sefer form üzerindeki kontrolleri tiplerine göre gruplayıp her bir tipten kaç adet olduğunu bulmak istediğimizi düşünelim. Bu basit gruplama işleminin kodu aşağıdaki gibi geliştirilebilir.

```csharp
private void button2_Click(object sender, EventArgs e)
{
    lstSonuclar.Items.Clear();

    IEnumerable<Control> kontroller=Controls.Cast<Control>();

    var farkliTipler = from kontrol in kontroller
                                group kontrol by kontrol.GetType() into grp
                                    select new
                                                {
                                                    KontrolAdi=grp.Key
                                                    ,Toplam=grp.Count()
                                                };

    foreach (var farkliTip in farkliTipler)
        lstSonuclar.Items.Add(String.Format("{0} : {1}",farkliTip.KontrolAdi,farkliTip.Toplam.ToString()));
}
```

Bu sefer gruplama işlemi Control tipinin GetType metoduna göre yapılmaktadır. Uygulama kodunun ekran çıktısı aşağıdaki gibi olacaktır.

![mk248_12.gif](/assets/images/2008/mk248_12.gif)

Cast metodu doğrudan LINQ genişletme metodlarının kullanılamadığı pek çok senaryoda ele alınabilir. Söz gelimi aşağıdaki kod parçası çok basit olarak Application Log altındaki girişlerden programın çalıştırıldığı gün içerisinde üretilenlerin çekilmesini sağlamaktadır.

```csharp
EventLog logs = new EventLog("Application", ".", "");
IEnumerable<EventLogEntry> entries = logs.Entries.Cast<EventLogEntry>();

var girisler = from entry in entries
                    where entry.TimeGenerated.Day == DateTime.Now.Day
                        select new
                                    {
                                        entry.Category,
                                        entry.CategoryNumber,
                                        entry.EntryType,
                                        entry.TimeGenerated
                                    };

foreach (var giris in girisler)
    Console.WriteLine(giris.ToString());
```

Bu kod parçasında kullanılan Cast genişletme metodu geriye, IEnumerable arayüzü (interface) tarafından taşınacak bir nesne topluluğu referansı döndürmektedir. IEnumerable arayüzüne ulaşıldığı içinde LINQ sorgusu kolay bir şekilde ele alınmış ve aşağıdaki ekran çıktısının üretilmesi sağlanmıştır.

![mk248_13.gif](/assets/images/2008/mk248_13.gif)

Cast metodu ile benzer özelliğe sahip bir diğer önemli metodda OfType genişletme metodudur. Bu metod bir nesne topluluğu üzerinde, generic parametre tipine göre filtreleme yapılabilmesini ve geriye LINQ sorgularının uygulanabileceği bir IEnumerable referansı döndürülmesini sağlamaktadır.

> Cast metodu ile OfType metodu benzer işlevselliğe sahip görünmekle birlikte arada önemli farklar vardır. OfType metodu temel olarak generic parametre tipine göre bir filtreleme yapmakta iken, Cast metodu generic parametre tipine dönüştürme yapmaktadır. Bu sebepten dönüştürme yapılamayacağı durumlarda Cast metodu, çalışma zamanında InvalidCastException istisnası üretilmesine neden olur. Oysaki OfType metodu bu durumu tamamen görmezden gelir ve diğer nesneden devam eder. OfType kendi içerisinde is anahtar kelimesini kullanarak tip kontrolü yapmaktayken, Cast doğrudan dönüştürme adımını uygular.

Şimdi OfType metodunu örnek bir senaryo üzerinden ele almaya çalışalım. Örneğin uygulamamızda kullandığımız.Net Framework 2.0 ile geliştirilmiş bir kütüphane olsun. Bu kütüphane içerisinde yer alan metodlardan bazılarınında ArrayList gibi tür güvenli olmayan koleksiyonlar döndürdüğünü düşünelim. Referansta bulunan uygulamanın.Net 3.5 tabanlı olduğu düşünülecek olursa, gelen koleksiyon nesneleri üzerinden LINQ sorguları çalıştırılması istenebilir. Bu noktada OfType metodu oldukça işe yarayacaktır. Söz konusu senaryoyo ele almak için aşağıdaki tipi içeren bir sınıf kütüphanesi (Class Library) olduğunu düşünelim.

```csharp
public class Yardimci
{
    public ArrayList ListeyiAl()
    {
        ArrayList liste = new ArrayList();
        liste.Add("Burak");
        liste.Add("Bili");
        liste.Add("Behçet");
        liste.Add("Necdet");
        liste.Add("Kerim");
        liste.Add("Mayk");
        liste.Add(19.90);
        liste.Add(10);
        liste.Add(true);
        liste.Add(false);
        liste.Add('C');
        return liste;
    }
}
```

Kod parçasında kasıtlı olarak ArrayList içerisine farklı tipte veriler atılmıştır. Eğerki LINQ sorgusunda bu metoddan dönen değerler içerisinden sadece string tabanlı olanları ele almak istiyorsak, OfType metodunu aşağıdaki kod parçasında olduğu gibi kullanabiliriz.

```csharp
GenelIslemler.Yardimci yrdm = new GenelIslemler.Yardimci();

var besHarfliler = from nesne in yrdm.ListeyiAl().OfType<string>()
                            where nesne.Length == 5
                                select nesne;

foreach (string nesne in besHarfliler)
    Console.WriteLine(nesne);
```

OfType metodu buradaki kullanıma göre ListeyiAl fonksiyonundan gelen ArrayList içerisindeki tüm nesnelerde, is kontrolünü yaparak sadece String olanları geriye döndürmektedir. Sonrasında nesnelerin karakter uzunluğu kıyaslanarak 5 ise çekilmektedir. Program kodunun çıktısı aşağıdaki gibi olacaktır.

![mk248_14.gif](/assets/images/2008/mk248_14.gif)

Yazımızda son olarak.Net Framework 2.0 ile yazılmış ve DataTable nesnelerini kullanan bir uygulamayı.Net 3.5' e taşıyarak basit LINQ sorgularını nasıl ele alabileceğimizi incelemeye çalışacağız. (LINQ sorgularının işlevselliğinin ön plana çıktığı vakalarda, var olan.Net uygulamaları.Net 3.5 versiyonuna terfi edilmek durumdan kalabilir.) Bu amaçla ilk olarak.Net Framework 2.0 ile geliştirilmiş ve test amacıyla aşağıdaki kodlara sahip bir Console uygulamamız olduğunu düşünelim.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace Net20DataTable
{
    class Program
    {
        static void Main(string[] args)
        {
            DataTable tbl = null;
    
            using (SqlConnection conn = new SqlConnection("data source=.;database=AdventureWorks;integrated security=SSPI"))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("Select ProductId,Name,ListPrice,Class,SellStartDate,ProductSubCategoryId From Production.Product", conn);
                tbl = new DataTable("Products");
                adapter.Fill(tbl);
            }
        }
    }
}
```

Kod, AdventureWorks isimli SQL Server 2005 veritabanına bağlanmakta ve Production şemasındaki Product tablosundan bir kaç alanı çekmektedir. Çekilen veri kümesi işlenilmek üzere bir DataTable nesnesi içerisinde toplanmaktadır. Çok doğal olarak uygulama.Net Framework 2.0 tabanlı olduğundan, LINQ ifadelerinin DataTable üzerinden uygulanması (veya başka bağlantısız katman nesneleri üzerinden) mümkün değildir. Eğer elimizde Visual Studio 2008 var ise yapılması gerekenler çok basittir. Öncelikli olarak proje özelliklerinden (Properties) Application sekmesine geçilmeli ve Target Framework seçeneği.Net Framework 3.5 olarak değiştirilmelidir.

![mk248_15.gif](/assets/images/2008/mk248_15.gif)

Bu işlemin ardından uygulamanın bir kere daha derlenmesinde yarar vardır. (Söz konusu adımların ardından System.Core.dll assembly'ının projeye hemen referans edildiğide görülebilir.) Artık LINQ sorgularının yazılmasına başlanabilir. DataTable için bu sorguların uygulanabilmesi için AsEnumerable metodunun erişilebilir olması gerekmektedir. Ancak bu genişletme metoduna şu anda erişilemediği görülmektedir. Bunun sebebi System.Data.DataSetExtensions.dll assembly'ının projeye referans edilmemiş olmasıdır. Dolayısıyla öncelikle bu assembly'ın referans edilmesi gerekmektedir.

![mk248_16.gif](/assets/images/2008/mk248_16.gif)

Artık uygulamada yer alan DataTable üzerinde LINQ sorguları çalıştırılabilir. İşte bir örnek;

```csharp
var altKategorisi4OlanUrunler = from row in tbl.AsEnumerable()
                                                    where row["ProductSubCategoryId"].ToString() == "4"
                                                        select new
                                                                    {
                                                                        Id = Convert.ToInt16(row["ProductId"]),
                                                                        Ad = row["Name"].ToString(),
                                                                        Fiyat = Convert.ToDouble(row["ListPrice"])
                                                                    };

foreach (var urun in altKategorisi4OlanUrunler)
    Console.WriteLine(urun.ToString());
```

Bu kod parçasında görülen LINQ sorgusuna göre DataTable içerisinden ProductSubCategoryId alanının değeri 4 olanların ProductId,Name,ListPrice kolonlarının verilerinden oluşan yeni bir isimsiz tip (Anonymous Type) topluluğu elde edilmektedir. Kodun ekran çıktısı aşağıdakine benzer olacaktır.

![mk248_17.gif](/assets/images/2008/mk248_17.gif)

Görüldüğü gibi LINQ sorguları.Net Framework içerisinde pek çok farklı alanda uygulanabilmektedir. Reflection, IO, Windows Forms Controls, Application Log, DataTable, eski bir uygulamadan gelen ArrayList bu yazıda ele alınan basit bir kaç alandır. LINQ sorguları Office ürünlerinde dahi kullanılabilmektedir. Söz gelimi Outlook içerisindeki kontaklar LINQ sorguları ile filtrelenebilir. Örnekleri arttırmak ve yaymak mümkündür. Ancak unutulmaması gereken noktalardan biriside bu işlemlerin yapılması için LINQ sorgularının olmasının zorunlu olmadığıdır.

Öyleki LINQ sorgularıda özünde,.Net Framework 3.5 ile gelen genişletme metodlarını (Extension Methods) yoğun bir şekilde ele almaktadır. Bir başka deyişle LINQ olmadanda metodlar yardımıyla bu istekler karşılanabilir. Diğer taraftan LINQ sorgularının getirdiği dil esnekliği, kullanım kolaylığı, anlaşılabilirlik göz ardı edilmemelidir. Her geliştirici kullandığı programlama dili yardımıyla nesneler üzerinden SQL benzeri sorgu ifadeleri yazabilmek ister. LINQ bu imkanı sağlayarak önemli bir açığı kapatmaktadır. Böylece geldik bir makalemizin daha sonuna. Bir sonraki makalemizde görüşünceye dek hepinize mutlu günler dilerim.

[Örnek Uygulama için Tıklayın](/assets/files/2008/LINQveReflectionArastirma.rar)