---
layout: post
title: "Using İfadesi ile Garanti Dispose"
date: 2007-10-27 21:37:00 +0300
categories:
  - csharp
tags:
  - csharp
  - using
  - garbage-collection
  - disposable-objects
  - idisposable
  - dispose
  - overriding
---
Bellek yönetimi özellikle büyük çaplı projelerde performans kazanımı açısından çok önemlidir. Günümüz sistemlerinin yüksek Ram kapasitesine sahip oldukları göz önüne alındığında bu durum çoğu zaman göz ardı edilmektedir. Ancak sayısız kullanıcının bağlandığı sunucu (server) sistemleri üzerinde çalışan windows veya web servisleri gibi uygulamalar göz önüne alındığında bellek sorunları ile her zaman için karşılaşılma olasılığı vardır.

Bildiğiniz gibi.Net ağırlıklı olarak yönetimli kodu (managed code) desteklemektedir. CLR (Common Language RunTime - Ortak Dil Çalışma Zamanı), yazmış olduğumuz uygulamarı çalıştırmaktan, kaynakları yönetmekten ve sonlandırmaktan sorumlu bir ortam olarak bu yönetim işini üstlenmiştir. CLR içerisinde istisna yönetimi, tip güvenliği gibi çeşitli kontrol ve yönetim mekanizmaları vardır. Bunlardan belkide en önemlisi (uygulama geliştiricileri özellikle C++' tan gelenleri bir nebze olsun rahatlatan) GC (Garbage Collector- Çöp Toplayıcı) mekanizmasıdır. GC, kapsama alanı (scope) dışına çıkan referans tiplerinin bellekten atılması görevini üstlenir. Oysaki C++ ile geliştirilen uygulamalarda, nesnelerin bellekten atılması elle yapılmak zorundadır. Dolayısıyla GC uygulama geliştiricilere büyük avantaj sağlar.

GC'nin sağlamış olduğu yönetim mekanizması her ne kadar avantajlı gözüksede uygulamaların performansı açısından dikkat edilmesi gereken bir nokta vardır. GC işleri biten referans türlerini bellekten anında atmaz. Bu atılma süresi genelde belirsizdir ve nesnenin kendisinin bağlı olduğu başka referanslara veya nesnenin içinde bulunduğu kapsama alanının (scope) yaşam süresine göre değişmektedir. Bahsi geçen süreler, özellikle birbirlerine bağlı referans türlerinin çok fazla sayıda kullanıldığı sunucu uygulamalarında belleğin gereksiz yere şismesine neden olmaktadır. (Hatta ben geliştirmiş olduğumuz bir projede OutOfMemoryException hatasını görebildiğimi söyleyebilirim.) Aslında bu durumu anlayabilmek için bellek üzerinde referans tiplerinin tutuluşunu ve GC tarafından sistemden kaldırılışını çok basit seviyede düşünmek gerekir. Aşağıdaki senaryoda basit olarak bu durum incelenmeye çalışılmaktadır.

Bir uygulamamız olduğunu ve uygulama kodlarının çalışma zamanında 3 farklı nesneyi oluşturduğunu düşünelim. Bu nesne örnekleri bizim kendi oluşturduğumuz tiplere ait olabileceği gibi FCL (Framework Class Library) içerisinde yer alan tiplerden de olabilir. Burada önemli olan bunların referans türünden olmaları ve new operatörü ile oluşturulmalarıdır. Bildiğiniz gibi referans türleri belleğin heap adı verilen bölgesinde tutulmaktadır. Bu durumda belleğin heap bölgesindeki yerleşim aşağıdakine benzer bir yapıda farzedilebilir.

![mk136_1.gif](/assets/images/2007/mk136_1.gif)

Burada nesnelerin arka arkaya dizildiğini görüyorsunuz. Çoğu zaman uygulamalarımızda nesneler ile işimiz bittiğinde (özellikle kendi yazdığımız sınıflara ait nesneler ile işimiz bittiğinde) bu nesneye ait kaynakları serbest bırakmak amacıyla null değer atamasına başvururuz. Nesne örneğine null değer atanması aslında o nesneye izleyen kod satırlarında erişilememesini garanti eden bir durumdur. Peki ya bu nesneye ait bellek kaynakları gerçekten ne zaman serbest kalacaktır. Senaryomuza geri dönelim. Null değer ataması yaptığımızda bellek görünümünde herhangibibir değişiklik olmaz. Sadece Y Nesnesi artık erişilebilir konumda değildir çünkü onu işaret eden adresleme artık mevcut değildir.

![mk136_2.gif](/assets/images/2007/mk136_2.gif)

Şu noktada yeni bir nesne örneğini daha oluşturduğumuzu düşünelim. Aslında oluşturmaya çalıştığımızı düşünürsek daha iyi olur. Örneğin Q nesnemizin aşağıdaki gibi bellekte konumlanacağını farzedelim.

![mk136_3.gif](/assets/images/2007/mk136_3.gif)

Burada Q nesnemizin boyutunun, belleğin sınırlarını aştığını temsil etmeye çalışıyoruz. Bu OutOfMemoryException'a yol açabilecek bir durumdur. Neyseki Garbage Collector (Artık Toplayıcı) durum bu noktaya gelmeden önce devreye girererek managed code içerisinde çalışmakta olan referans türlerini gözden geçirir ve uzun süredir kullanılmayan nesnelerin olup olmadığını devamlı suretle kontrol eder. Bu senaryoya göre ilk yapacağı iş null ataması yapılmış nesneyi bulmak ve bellekteki adreslemeleri yeniden düzenleyerek yer tahsislerini kaydırmaktır. Sonuç olarak belleğin yeni görünümü aşağıdaki gibi olacaktır.

![mk136_4.gif](/assets/images/2007/mk136_4.gif)

Bu durum kapsama alanından çıkan her nesne için geçerlidir. Kısacası,

> GC, kapsama alanı dışına çıkmış olan nesnelere ait kaynakları hemen serbest bırakmaz. Bu serbest bırakma süresi genellikle belli değildir. Sürenin belirsiz oluşu ve gereksiz nesneler toplanıncaya kadar bellekte kalarak adres alanı işgal ediyor olmaları sistemin yavaşlamasına neden olur.

Elbetteki GC'nin yaptığı işi küçümseyemeyiz. Bellekteki kullanım dışı kalmış nesnelerin bir mekanizma sayesinde otomatik olarak sistemden kaldırılması gerçekten işimizi kolaylaştırmaktadır. Ancak yukarıdaki gibi bir senaryoyu göz önüne aldığımızda bellek üzerindeki nesnelere ait yapılan yer değiştirmeler uygulamanın yavaşlamasına neden olacaktır. İşte bu yüzden kullandığımız nesneler ile ilgili işimiz bittiğinde onlara ait bellek kaynaklarının kesin olarak serbest bırakılmasını sağlamak isteyebiliriz. Bu noktada IDisposable arayüzünden türemiş nesnelerin Dispose metodlarını kullanabiliriz. Dispose metodunda gerekli kodlamaları yaparak ilgili nesnenin anında bellekten atılmasını sağlayabiliriz.

> Bir nesne üzerinde Dispose metodunu kullanabilmek için, bu nesnenin IDisposable arayüzünü mutlaka implemente etmesi gerekmektedir.

FCL içerisinde yer alan sayısız nesne IDisposable arayüzünü uygulamaktadır. Bu nesneleri using ifadesi ile veya try-catch-finally bloğunun uygulandığı desenler yarıdımıyla bellekten daha kısa sürede atabiliriz. İlk olarak try-catch-finally bloğu ile bu işi nasıl gerçekleştirebileceğimize bakalım. Aşağıdaki örnek kod parçası, basit olarak bir referans tipinin işi bittiğinde derhal bellekten atılmasının nasıl sağlanacağını göstermektedir.

```csharp
using System;
using System.Data;
using System.Data.SqlClient;

namespace InvestigateForDispose
{
    class AnaProgram
    {
        static void Main(string[] args)
        {
            SqlCommand cmd=null;
            SqlConnection con=null;
            try
            {
                con=new SqlConnection("data source=localhost;database=AdventureWorks2000;user id=sa;password=");
                cmd=new SqlCommand("SELECT TOP 1 * FROM Customer",con);
                con.Open(); 
                cmd.ExecuteNonQuery();
            }
            catch(SqlException exp)
            {
                Console.WriteLine(exp.Message);
            }
            finally
            {
                con.Dispose();
                cmd.Dispose();
            }
        }
    }
}
```

Finally blokları try bloğunda yer alan kodlarda istisna fırlatılmasına neden olacak hatalar olsa da olmasa da devreye girer. Bu sebeple serbest bırakılacak nesneler için Dispose metodlarını çağıracağımız en uygun yer finally bloklarıdır. Yukarıdaki örnekte yer alan SqlConnection ve SqlCommand, IDisposable arayüzünü implemente eden sınıflardır. Bu sebepten Dispose metodları mevcuttur. Yukarıdaki desenin sağladığı işlevselliğin aynısını using ifadesi ile de gerçekleştirebiliriz. Aşağıdaki kod parçası yukarıdaki örneğin using ifadeleri ile nasıl kodlandığını göstermektedir.

```csharp
using(SqlConnection con=new SqlConnection("data source=localhost;database=AdventureWorks2000;user id=sa;password="))
{
    using(SqlCommand cmd=new SqlCommand("SELECT TOP 1 * FROM Customer",con))
    {
        con.Open();
        cmd.ExecuteNonQuery();
    }
}
```

Using bloğunun yazımı try-catch-finally desenine göre daha kolaydır. Aslında Using bloğuda bir nevi try-finally bloğudur. Bu durumu daha iyi anlayabilmek için using ifadesinin MSIL (Microsoft Intermediate Language) koduna bakmakta fayda var. ILDASM aracı ile uygulamamızın Main metodundaki kodlara baktığımızda aslında Dispose işlemi için try-finally bloklarının uygulandığını görürüz. Bir başka deyişle using ifademiz IL kodu içerisinde try-finally bloğuna dönüştürülmüştür.

```text
.method private hidebysig static void Main(string[] args) cil managed
{
    .entrypoint
    // Code size 61 (0x3d)
    .maxstack 3
    .locals ([0] class [System.Data]System.Data.SqlClient.SqlConnection con,
    [1] class [System.Data]System.Data.SqlClient.SqlCommand cmd)
    IL_0000: ldstr "data source=localhost;database=AdventureWorks2000;"
    + "user id=sa;password="
    IL_0005: newobj instance void [System.Data]System.Data.SqlClient.SqlConnection::.ctor(string)
    IL_000a: stloc.0
    .try
    {
        IL_000b: ldstr "SELECT TOP 1 * FROM Customer"
        IL_0010: ldloc.0
        IL_0011: newobj instance void [System.Data]System.Data.SqlClient.SqlCommand::.ctor(string,
        class [System.Data]System.Data.SqlClient.SqlConnection)
        IL_0016: stloc.1
        .try
        {
            IL_0017: ldloc.0
            IL_0018: callvirt instance void [System.Data]System.Data.SqlClient.SqlConnection::Open()
            IL_001d: ldloc.1
            IL_001e: callvirt instance int32 [System.Data]System.Data.SqlClient.SqlCommand::ExecuteNonQuery()
            IL_0023: pop
            IL_0024: leave.s IL_0030
        } // end .try
        finally
        {
            IL_0026: ldloc.1
            IL_0027: brfalse.s IL_002f
            IL_0029: ldloc.1
            IL_002a: callvirt instance void [mscorlib]System.IDisposable::Dispose()
            IL_002f: endfinally
        } // end handler
        IL_0030: leave.s IL_003c
    } // end .try
    finally
    {
        IL_0032: ldloc.0
        IL_0033: brfalse.s IL_003b
        IL_0035: ldloc.0
        IL_0036: callvirt instance void [mscorlib]System.IDisposable::Dispose()
        IL_003b: endfinally
    } // end handler
    IL_003c: ret
} // end of method AnaProgram::Main
```

Biz uygulamamızda iç içe iki using bloğu kullandığımızdan, uygulamanın IL kodunda iç içe geçmiş iki try-finally bloğu yer almaktadır. Dikkat ettiyseniz finally bloklarında SqlConnection ve SqlCommand nesnelerine ait Dispose metodları çağırılmıştır. Eğer IDisposable arayüzünü implemente etmemiş bir nesne örneği için using veya try-catch-finally desenini kullanırsak derleme zamanı hatası alırız.

Using bloğu için aşağıdaki hata alınır.

![mk136_6.gif](/assets/images/2007/mk136_6.gif)

Try-catch-finally deseninde de zaten Nesne üzerinden erişebileceğimiz bir Dispose metodu yoktur.

![mk136_7.gif](/assets/images/2007/mk136_7.gif)

Aslında bu gibi durumlarda using bloğunda kullanılacak nesnenin IDisposable arayüzünü uygulayıp uygulamadığını bilmek ve buna göre davranmak oldukça etkili bir yaklaşım olabilir.

Peki kendi sınıflarımıza Dispose yeteneğini nasıl kazandırabiliriz? Bunun için sınıfımıza IDisposable arayüzünü uygularız. IDisposable arayüzü sadece Dispose metoduna ilişkin bir bildirim içerir. Sınıfın Dispose metodunda managed ve unmanaged kaynaklar için gerekli yoketme işlemlerini gerçekleştirebiliriz.

```csharp
class VeriYonetim:IDisposable
{
    public VeriYonetim()
    {}
    public void Baglan()
    {}

    #region IDisposable Members

    public void Dispose()
    {
        // Managed ve Unmanaged kaynakların serbest bırakılması.
    }
    
    #endregion
}
class AnaProgram
{
    static void Main(string[] args)
    {
        using(VeriYonetim vy=new VeriYonetim())
        {
            // Bir takım işlemler
        }
    }
}
```

Disposable nesnelerin kullanımında dikkat edilmesi gereken bir nokta daha vardır. Bazı durumlarda, nesne örneklerimiz unmanaged (yönetilmeyen) referanslara sahip olabilirler. Bu tip unmanaged referanslar söz konusu olduğunda bunların açık bir şekilde sonlandırılmaları gerekir. Bu bir nesnenin yaşam düzeni ile ilgilidir. Öyleki bir nesne heap bölgesinde geçirdiği zaman süresi boyunca aşağıdaki şekilde tasvir edilen evrelerden geçer.

![mk136_5.gif](/assets/images/2007/mk136_5.gif)

Her nesne önce oluşturulur. Çoğunlukla new operatörü ile gerçekleştirilen bu işlemin ardından nesne uygulama içerisinde kullanılır. Kullanım sona erdiğinde ve nesne scope (kapsama alanı) dışına çıktığında kullanılamaz hale gelir. Bu makalenin başında belirttiğimiz gibi nesneye ait kaynakların serbest bırakılması anlamına gelmez. Sadece referans yok edilmiştir. Daha sonra nesnemiz sonlandırılabilir hale gelir ve nesne ömrü sonlandırılır. Sonlandırma işleminde genellikle nesne içerisinde var olan unmanaged referansların bellekten atılması işlemi gerçekleştirilir. Nihayetinde sonlandırılmış nesneye ait bellek bölgeleri serbest bırakılır. İşte biz Dispose metodunu ve SuppressFinalize metodunu kullanarak sonlandırma aşamasını otomatik olarak atlayabilir ve nesneye ait kaynakların iade edilmesini sağlayabiliriz.

> Bir nesnenin referans ettiği unmanaged kaynakları yok etmek için ya Dispose metodunun içeriği kullanılır ya da sınıfa ait destructor (yokedici) metod içerisinde bu yok etme işlemi gerçekleştiriliz.

Peki böyle bir imkan var ise neden sonlandırma süreci söz konusudur? Managed olarak oluşturulmuş nesneler GC kontrolünde yok edilirler. Ancak içerisinde unmanaged referanslar var ise, bu referansların açıkça yok edilmeleri bir başka deyişle kaynaklarının serbest bırakılmaları gerekir. Oysaki GC bu işlemin nasıl yapılabileceğini tahmin edemez. Bu tarz durumlarda unmanaged referanslara ait kaynakları serbest bırakmak için nesnelerin destructor metodlarından faydalanılabilir.

Diğer yandan her nesne için GC tarafından yönetilen bir sonlandırma kuyruğu (finalizable queue) söz konusudur. Bu kuyruk finalize edilebilir nesneleri tutan bir koleksiyondur. GC oluşturulan nesneler için buraya giriş yapar ve nesne sonlandırma sürecine girdiğinde ilgili sonlandırma işlemini uygulayacağı nesne bilgilerini almak için yine bu kuyruğu kullanır. Eğer ki Dispose etmek istediğimiz nesnenin unmanaged referansları varsa bunlarıda dispose işlemi ile birlikte anında serbest bırakmak için GC sınıfının SuppressFinalize metodu kullanılabilir.

Bu koşullar göz önüne alındığında çoğunlukla kullanılan bir desen vardır. Bu desende hem IDisposable arayüzü implemente edilir hemde sınıfa ait desctructor (yokedici) metod kullanılır. Desctructor metodlar bildiğiniz gibi nesne GC tarafından yok edilmeden önce çalışan son metoddur. Burada çoğunlukla unmanaged referansların serbest bırakılma işlemleri ele alınır. Söz konusu desen aşağıdaki yapıya sahiptir.

```csharp
class VeriYonetim:IDisposable
{
    ~VeriYonetim()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposeDurumu)
    {
        if(disposeDurumu==true)
        {
            // Managed kaynaklar için Dispose metodu uygulanır.
        }
        // UnManaged kaynaklar temizlenir.
    }

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
```

Bu desendeki yaklaşım kısaca şudur; nesnemizi eğer Dispose metodunu kullanarak çağırırsak bu durumda overload ettiğimiz ve sadece bu sınıftan türeyen sınıflar tarafından erişilip override edilebilen Dispose metodu devreye girecektir. Bu metod içerisinde managed kodlarımız için gerekli temizleme işlemleri yapılır. Hemen ardından da unmanaged kaynaklar silinir. Peki desctructor metodu neden kullanıyoruz?

Nesnemizin illede açıkça Dispose edilmesi gibi biri durum söz konusu değildir. Pekala, nesne doğal yollardan scope dışına çıktığında GC tarafından serbest bırakılabilir. İşte bu serbest bırakma anından hemen önce desctructor metodu devreye girmektedir. Desctructor metodumuzdan yine overload ettiğimi virtual Dispose metodumuzu çağırırız. Ancak bu kez managed kaynaklar zaten GC tarafından serbest bırakılacağından durumu müdahale etmeyiz. Bu yüzden metoda parametre false olarak gönderilir. Ancak yine de unmanaged kaynaklar bizim tarafımızdan serbest bırakılacaktır. Son olarak Dipose metodumuzda kullandığımız GC.SuppressFinalize metod çağırımı ile o anki nesneye ait sonlandırma kuyruğundaki kaynaklarıda serbest bırakmış oluruz.

Bir başka makalemizde görüşünceye dek hepinize mutlu günler dilerim.